using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

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

	[Fact]
	public async Task WhenNegated_WithValidation_AndFailingMemberExpectation_ShouldSucceed()
	{
		string subject = "foo";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasLength(it, length => length.IsEqualTo(4)));

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenNegated_WithValidation_ShouldNegateValidation()
	{
		string subject = "foo";

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => HasLength(it, length => length.IsEqualTo(3)));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             does not have a length which is equal to 3,
			             but it had
			             """);
	}

	private static AndOrResult<string?, IThat<string?>> HasLength(
		IThat<string?> subject,
		Action<IThat<int>> expectations)
		=> new(subject.Get().ExpectationBuilder
				.ForAsyncMember(
					MemberAccessor<string?, Task<int>>.FromFunc(s => Task.FromResult(s!.Length), " which "),
					replaceIt: false)
				.Validate((it, grammars) => new HasLengthConstraint(it, grammars))
				.AddExpectations(e => expectations(new ThatSubject<int>(e))),
			subject);

	private sealed class HasLengthConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = actual is null ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a length");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was <null>");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have a length");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
