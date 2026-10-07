using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public class MappingNodeTests
{
	[Test]
	public async Task Equals_IfMemberAccessorsAreDifferent_ShouldBeFalse()
	{
		MappingNode<string, int, int> node1 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length1 "));
		MappingNode<string, int, int> node2 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length2 "));

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_IfMemberAccessorsAreSame_ShouldBeTrue()
	{
		MappingNode<string, int, int> node1 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		MappingNode<string, int, int> node2 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));

		bool result = node1.Equals(node2);

		await That(result).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_IfMemberExpectationsAreDifferent_ShouldBeFalse()
	{
		MappingNode<string, int, int> node1 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node1.AddConstraint(new DummyConstraint("is 1"));
		MappingNode<string, int, int> node2 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node2.AddConstraint(new DummyConstraint("is 2"));

		await That(node1.Equals(node2)).IsFalse();
		await That(node2.Equals(node1)).IsFalse();
	}

	[Test]
	public async Task Equals_IfMemberExpectationsAreSame_ShouldBeTrue()
	{
		MappingNode<string, int, int> node1 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node1.AddConstraint(new DummyConstraint("is 1"));
		MappingNode<string, int, int> node2 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node2.AddConstraint(new DummyConstraint("is 1"));

		await That(node1.Equals(node2)).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_IfNodesWithMemberExpectationsAreDifferent_ShouldBeFalse()
	{
		MappingNode<string, int, int> node1 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node1.AddNode(new DummyNode("is 1"));
		MappingNode<string, int, int> node2 = new(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node2.AddNode(new DummyNode("is 2"));

		await That(node1.Equals(node2)).IsFalse();
		await That(node2.Equals(node1)).IsFalse();
	}

	[Test]
	public async Task Equals_WhenOtherIsDifferentNode_ShouldBeFalse()
	{
		MappingNode<string, int, int> node = new(MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		object other = new MappingNode<int, int, int>(MemberAccessor<int, int>.FromFunc(s => s * 2, " duplicate "));

		bool result = node.Equals(other);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		MappingNode<string, int, int> node = new(MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));

		bool result = node.Equals(null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsMetBy_ShouldUseInnerConstraintWithOuterValue()
	{
		MappingNode<string, int, int> node = new(MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Success, v, $"yeah: {v}")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("yeah: 6");
		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(result.TryGetValue(out string? value)).IsTrue();
		await That(value).IsEqualTo("foobar");
	}

	[Test]
	public async Task IsMetBy_WhenMemberThrows_ShouldFailWithoutEvaluatingMemberConstraints()
	{
		NotSupportedException exception = new("foo");
		MappingNode<string, int, int> node = new(MemberAccessor<string, int>.FromFunc(_ => throw exception, " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foo", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(result.FailureCause).IsSameAs(exception);
		await That(sb.ToString()).IsEqualTo("yeah!");
	}

	[Test]
	public async Task IsMetBy_WhenMemberThrows_ShouldUseTheExpectationResultOfExpectationTextConstraints()
	{
		MappingNode<string, int, int> node = new(
			MemberAccessor<string, int>.FromFunc(_ => throw new NotSupportedException("foo"), " length "));
		node.AddConstraint(new ExpectationTextConstraint<int>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foo", null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(sb.ToString()).IsEqualTo("not yeah!");
	}

	[Test]
	public async Task IsMetBy_WhenMemberThrows_WhenNegated_ShouldNegateExpectationAndStillFail()
	{
		MappingNode<string, int, int> node = new(
			MemberAccessor<string, int>.FromFunc(_ => throw new NotSupportedException("foo"), " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foo", null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(sb.ToString()).IsEqualTo("not yeah!");
	}

	[Test]
	public async Task IsMetBy_WithInvalidType_ShouldNotApplyTheMemberExpectations()
	{
		MappingNode<string, int, int> node = new(MemberAccessor<string, int>.FromFunc(s => s.Length, " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int>("yeah!", "not yeah!"));

		ConstraintResult result = await node.IsMetBy(42, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Undecided);
		await That(result.GetExpectationText()).IsEqualTo("yeah!");
		await That(result.GetResultText()).IsEmpty();
	}

	[Test]
	public async Task IsMetBy_WithNullDelegate_ShouldReturnNullFailure()
	{
		DelegateValue<string?> value = new("foo", null, 10.Milliseconds(), true);
		MappingNode<string?, int?, int?> node = new(MemberAccessor<string?, int?>.FromFunc(s => s?.Length, " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int?>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(value, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(sb.ToString()).IsEqualTo("yeah!");
		await That(result.GetResultText()).IsEqualTo("it was <null>");
	}

	[Test]
	public async Task IsMetBy_WithNullValue_ShouldReturnNullFailure()
	{
		MappingNode<string?, int?, int?> node = new(MemberAccessor<string?, int?>.FromFunc(s => s?.Length, " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int?>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(sb.ToString()).IsEqualTo("yeah!");
		await That(result.GetResultText()).IsEqualTo("it was <null>");
	}

	[Test]
	public async Task IsMetBy_WithNullValue_WhenNegated_ShouldNegateExpectationAndStillFail()
	{
		MappingNode<string?, int?, int?> node = new(MemberAccessor<string?, int?>.FromFunc(s => s?.Length, " length "));
		node.AddConstraint(new NotEvaluatedConstraint<int?>("yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(sb.ToString()).IsEqualTo("not yeah!");
		await That(negated.GetResultText()).IsEqualTo("it was <null>");
	}
}
