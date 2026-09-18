using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public class AsyncMappingNodeTests
{
	[Fact]
	public async Task Equals_IfMemberAccessorsAreDifferent_ShouldBeFalse()
	{
		AsyncMappingNode<string, int> node1 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length1 "));
		AsyncMappingNode<string, int> node2 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length2 "));

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Fact]
	public async Task Equals_IfMemberAccessorsAreSame_ShouldBeTrue()
	{
		AsyncMappingNode<string, int> node1 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
		AsyncMappingNode<string, int> node2 = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));

		bool result = node1.Equals(node2);

		await That(result).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Fact]
	public async Task Equals_WhenOtherIsDifferentNode_ShouldBeFalse()
	{
		AsyncMappingNode<string, int> node = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
		object other = new AsyncMappingNode<int, int>(
			MemberAccessor<int, Task<int>>.FromFunc(s => Task.FromResult(s * 2), " duplicate "));

		bool result = node.Equals(other);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		AsyncMappingNode<string, int> node = new(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));

		bool result = node.Equals(null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsMetBy_ShouldUseInnerConstraintWithOuterValue()
	{
		AsyncMappingNode<string, int> node =
			new(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
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

	[Fact]
	public async Task IsMetBy_WithInvalidType_ShouldThrowInvalidOperationException()
	{
		AsyncMappingNode<string, int> node =
			new(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length "));
		node.AddConstraint(
			new DummyValueConstraint<int?>(v => new DummyConstraintResult<int?>(Outcome.Success, v, "yeah!")));
		async Task Act() => await node.IsMetBy(42, null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("""
			             The member type for the actual value in the which node did not match.
			             Expected: string,
			                Found: int
			             """);
	}

	[Fact]
	public async Task IsMetBy_WithNullDelegate_ShouldReturnNullFailure()
	{
		DelegateValue<string?> value = new("foo", null, 10.Milliseconds(), true);
		AsyncMappingNode<string?, int?> node =
			new(MemberAccessor<string?, Task<int?>>.FromFunc(s => Task.FromResult(s?.Length), " length "));
		node.AddConstraint(
			new DummyValueConstraint<int?>(v => new DummyConstraintResult<int?>(Outcome.Success, v, "yeah!")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(value, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("yeah!");
		await That(result.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task IsMetBy_WithNullValue_ShouldReturnNullFailure()
	{
		AsyncMappingNode<string?, int?> node =
			new(MemberAccessor<string?, Task<int?>>.FromFunc(s => Task.FromResult(s?.Length), " length "));
		node.AddConstraint(
			new DummyValueConstraint<int?>(v => new DummyConstraintResult<int?>(Outcome.Success, v, "yeah!")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("yeah!");
		await That(result.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task IsMetBy_WithNullValue_WhenNegated_ShouldStillFail()
	{
		AsyncMappingNode<string?, int?> node =
			new(MemberAccessor<string?, Task<int?>>.FromFunc(s => Task.FromResult(s?.Length), " length "));
		node.AddConstraint(
			new DummyValueConstraint<int?>(v => new DummyConstraintResult<int?>(Outcome.Failure, v, "yeah!")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("yeah!");
		await That(negated.GetResultText()).IsEqualTo("it was <null>");
	}
}
