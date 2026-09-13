using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public class ExpectationNodeTests
{
	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Success)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task AddAsyncMapping_NegatedResult_ShouldHaveExpectedOutcome(
		Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(node2, "foo2", "bar2")));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task AddAsyncMapping_ShouldUseAndCombination(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(node2, "foo2", "bar2")));
		StringBuilder expectationSb = new();
		StringBuilder resultSb = new();
		string expectedResult = (node1 == Outcome.Failure, node2 == Outcome.Failure) switch
		{
			(true, true) => "bar1 and bar2",
			(true, _) => "bar1",
			(_, true) => "bar2",
			(_, _) => "",
		};

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.AppendExpectation(expectationSb);
		result.AppendResult(resultSb);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(expectationSb.ToString()).IsEqualTo("foo1 length: foo2");
		await That(resultSb.ToString()).IsEqualTo(expectedResult);
	}

	[Fact]
	public async Task AddAsyncMapping_TryGetValue_ShouldGetValueFromLeftNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_
			=> new DummyConstraintResult("foo", Outcome.Undecided, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_
				=> new DummyConstraintResult(1, Outcome.Undecided, "foo2", "bar2")));

		ConstraintResult constraintResult = await node.IsMetBy("foobar", null!, CancellationToken.None);
		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo("foo");
	}

	[Fact]
	public async Task AddAsyncMapping_TryGetValue_ShouldGetValueFromRightNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_
			=> new DummyConstraintResult(Outcome.Undecided, "foo1", "bar1")));
		node.AddAsyncMapping(
				MemberAccessor<string, Task<string>>.FromFunc(s => Task.FromResult(s.Substring(1)), " substring: "))
			.AddConstraint(new DummyValueConstraint<string>(_
				=> new DummyConstraintResult(Outcome.Undecided, "foo2", "bar2")));

		ConstraintResult constraintResult = await node.IsMetBy("foobar", null!, CancellationToken.None);
		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo("foobar");
	}

	[Fact]
	public async Task AddAsyncMapping_TryGetValue_WhenTypeDoesNotMatchAnyNode_ShouldReturnFalse()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_
			=> new DummyConstraintResult("foo", Outcome.Undecided, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_
				=> new DummyConstraintResult(42, Outcome.Undecided, "foo2", "bar2")));

		ConstraintResult constraintResult = await node.IsMetBy("foobar", null!, CancellationToken.None);
		bool result = constraintResult.TryGetValue(out DateTime? value);

		await That(result).IsFalse();
		await That(value).IsNull();
	}

	[Fact]
	public async Task AddAsyncMapping_WithCustomExpectationTextGenerator_ShouldUseIt()
	{
		ExpectationNode node = new();
		node.AddAsyncMapping(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length: "),
				(m, sb) => sb.Append("my custom generator:").Append(m))
			.AddConstraint(new DummyConstraint<int>(_ => true, "yeah"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("my custom generator: length: yeah");
	}

	[Fact]
	public async Task AddConstraint_Twice_ShouldThrowInvalidOperationException()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));

		void Act() => node.AddConstraint(new DummyConstraint("bar"));

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"You have to specify how to combine the expectations! Use `And()` or `Or()` in between adding expectations.");
	}

	[Fact]
	public async Task AddConstraint_WithAsyncMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddAsyncMapping(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foobar");
	}


	[Fact]
	public async Task AddConstraint_WithAsyncNarrowingMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddAsyncNarrowingMapping<string, object?, int>(
			MemberAccessor<string, Task<object?>>.FromFunc(s => Task.FromResult<object?>(s.Length), " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foobar");
	}

	[Fact]
	public async Task AddConstraint_WithMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foobar");
	}

	[Fact]
	public async Task AddConstraint_WithNarrowingMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddNarrowingMapping<string, object?, int>(
			MemberAccessor<string, object?>.FromFunc(s => s.Length, " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foobar");
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Success)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task AddMapping_NegatedResult_ShouldHaveExpectedOutcome(
		Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(node2, "foo2", "bar2")));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task AddMapping_ShouldUseAndCombination(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(node2, "foo2", "bar2")));
		StringBuilder expectationSb = new();
		StringBuilder resultSb = new();
		string expectedResult = (node1 == Outcome.Failure, node2 == Outcome.Failure) switch
		{
			(true, true) => "bar1 and bar2",
			(true, _) => "bar1",
			(_, true) => "bar2",
			(_, _) => "",
		};

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.AppendExpectation(expectationSb);
		result.AppendResult(resultSb);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(expectationSb.ToString()).IsEqualTo("foo1 length: foo2");
		await That(resultSb.ToString()).IsEqualTo(expectedResult);
	}

	[Fact]
	public async Task AddMapping_TryGetValue_ShouldGetValueFromLeftNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_
			=> new DummyConstraintResult("foo", Outcome.Undecided, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_
				=> new DummyConstraintResult(1, Outcome.Undecided, "foo2", "bar2")));

		ConstraintResult constraintResult = await node.IsMetBy("foobar", null!, CancellationToken.None);
		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo("foo");
	}

	[Fact]
	public async Task AddMapping_TryGetValue_ShouldGetValueFromRightNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_
			=> new DummyConstraintResult(Outcome.Undecided, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, string>.FromFunc(s => s.Substring(1), " substring: "))
			.AddConstraint(new DummyValueConstraint<string>(_
				=> new DummyConstraintResult(Outcome.Undecided, "foo2", "bar2")));

		ConstraintResult constraintResult = await node.IsMetBy("foobar", null!, CancellationToken.None);
		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo("foobar");
	}

	[Fact]
	public async Task AddMapping_TryGetValue_WhenTypeDoesNotMatchAnyNode_ShouldReturnFalse()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_
			=> new DummyConstraintResult("foo", Outcome.Undecided, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_
				=> new DummyConstraintResult(42, Outcome.Undecided, "foo2", "bar2")));

		ConstraintResult constraintResult = await node.IsMetBy("foobar", null!, CancellationToken.None);
		bool result = constraintResult.TryGetValue(out DateTime? value);

		await That(result).IsFalse();
		await That(value).IsNull();
	}

	[Fact]
	public async Task AddMapping_WithCustomExpectationTextGenerator_ShouldUseIt()
	{
		ExpectationNode node = new();
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "),
				(m, sb) => sb.Append("my custom generator:").Append(m))
			.AddConstraint(new DummyConstraint<int>(_ => true, "yeah"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("my custom generator: length: yeah");
	}

	[Fact]
	public async Task AddNode_ShouldThrowNotSupportedException()
	{
		ExpectationNode node = new();

		void Act() => node.AddNode(new DummyNode("foo"));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Don't specify the inner node for Expectation nodes directly. Use AddMapping() instead!");
	}

	[Fact]
	public async Task AppendExpectation_Empty_ShouldReturnEmptyText()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("");
	}

	[Fact]
	public async Task AppendExpectation_WithAsyncConstraintAndWithMapping_ShouldReturnBoth()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddConstraint(new DummyConstraint("foo"));
		node.AddAsyncMapping(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo");
	}

	[Fact]
	public async Task AppendExpectation_WithAsyncMapping_ShouldReturnMapping()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddAsyncMapping(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("");
	}

	[Fact]
	public async Task AppendExpectation_WithConstraint_ShouldReturnConstraint()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddConstraint(new DummyConstraint("foo"));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo");
	}

	[Fact]
	public async Task AppendExpectation_WithConstraintAndWithMapping_ShouldReturnBoth()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddConstraint(new DummyConstraint("foo"));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo");
	}

	[Fact]
	public async Task AppendExpectation_WithMapping_ShouldReturnMapping()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("");
	}

	[Fact]
	public async Task Equals_IfConstraintIsDifferent_ShouldBeFalse()
	{
		ExpectationNode node1 = new();
		node1.AddConstraint(new DummyConstraint("foo"));
		ExpectationNode node2 = new();
		node2.AddConstraint(new DummyConstraint("bar"));

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_IfConstraintIsTheSame_ShouldBeTrue()
	{
		ExpectationNode node1 = new();
		node1.AddConstraint(new DummyConstraint("foo"));
		ExpectationNode node2 = new();
		node2.AddConstraint(new DummyConstraint("foo"));

		bool result = node1.Equals(node2);

		await That(result).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Fact]
	public async Task Equals_IfInnerNodesAreDifferent_ShouldBeFalse()
	{
		ExpectationNode node1 = new();
		node1
			.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " with length1 "));
		ExpectationNode node2 = new();
		node2
			.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " with length2 "));

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Fact]
	public async Task Equals_IfInnerNodesAreSame_ShouldBeTrue()
	{
		ExpectationNode node1 = new();
		node1
			.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " with length "));
		ExpectationNode node2 = new();
		node2
			.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " with length "));

		bool result = node1.Equals(node2);

		await That(result).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Fact]
	public async Task Equals_WhenOtherIsDifferentNode_ShouldBeFalse()
	{
		ExpectationNode node = new();
		object other = new DummyNode("");

		bool result = node.Equals(other);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		ExpectationNode node = new();

		bool result = node.Equals(null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsMetBy_WhenAsyncConstraintReturns_ShouldApplyBecauseReason()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncConstraint<int>(_
			=> Task.FromResult<ConstraintResult>(new DummyConstraintResult(Outcome.Success, "foo"))));
		node.SetReason(new BecauseReason("my reason"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(44, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo, because my reason");
	}

	[Fact]
	public async Task IsMetBy_WhenAsyncConstraintThrowsException_ShouldThrowInvalidOperationException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncConstraint<int>(_ => Task.FromException<ConstraintResult>(exception)));

		async Task Act() =>
			await node.IsMetBy(44, null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithInner<MyException>(x => x.HasMessage(exception.Message)).And
			.WithMessage("""
			             Error evaluating DummyAsyncConstraint<int> constraint with value 44: IsMetBy_WhenAsyncConstraintThrowsException_ShouldThrowInvalidOperationException
			             """);
	}

	[Fact]
	public async Task IsMetBy_WhenAsyncContextConstraintReturns_ShouldApplyBecauseReason()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncContextConstraint<int>(_
			=> Task.FromResult<ConstraintResult>(new DummyConstraintResult(Outcome.Success, "foo"))));
		node.SetReason(new BecauseReason("my reason"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(44, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo, because my reason");
	}

	[Fact]
	public async Task IsMetBy_WhenAsyncContextConstraintThrowsException_ShouldThrowInvalidOperationException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncContextConstraint<int>(_ => Task.FromException<ConstraintResult>(exception)));

		async Task Act() =>
			await node.IsMetBy(45, null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithInner<MyException>(x => x.HasMessage(exception.Message)).And
			.WithMessage("""
			             Error evaluating DummyAsyncContextConstraint<int> constraint with value 45: IsMetBy_WhenAsyncContextConstraintThrowsException_ShouldThrowInvalidOperationException
			             """);
	}

	[Fact]
	public async Task IsMetBy_WhenContextConstraintReturns_ShouldApplyBecauseReason()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyContextConstraint<int>(_ => new DummyConstraintResult(Outcome.Success, "foo")));
		node.SetReason(new BecauseReason("my reason"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(44, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo, because my reason");
	}

	[Fact]
	public async Task IsMetBy_WhenContextConstraintThrowsException_ShouldThrowInvalidOperationException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyContextConstraint<int>(_ => throw exception));

		async Task Act() =>
			await node.IsMetBy(43, null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithInner<MyException>(x => x.HasMessage(exception.Message)).And
			.WithMessage("""
			             Error evaluating DummyContextConstraint<int> constraint with value 43: IsMetBy_WhenContextConstraintThrowsException_ShouldThrowInvalidOperationException
			             """);
	}

	[Fact]
	public async Task IsMetBy_WhenValueConstraintReturns_ShouldApplyBecauseReason()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(Outcome.Success, "foo")));
		node.SetReason(new BecauseReason("my reason"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(44, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo, because my reason");
	}

	[Fact]
	public async Task IsMetBy_WhenValueConstraintThrowsException_ShouldThrowInvalidOperationException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => throw exception));

		async Task Act() =>
			await node.IsMetBy("42", null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithInner<MyException>(x => x.HasMessage(exception.Message)).And
			.WithMessage("""
			             Error evaluating DummyValueConstraint<string> constraint with value "42": IsMetBy_WhenValueConstraintThrowsException_ShouldThrowInvalidOperationException
			             """);
	}

	[Fact]
	public async Task IsMetBy_WithAsyncMapping_WhenConstraintAndInnerFail_ShouldCombineFailureMessage()
	{
		ExpectationNode node = new();
		node.AddConstraint(
			new DummyValueConstraint<int>(v
				=> new DummyConstraintResult<int>(Outcome.Failure, v, "foo", "outer failure")));
		node.AddAsyncMapping(MemberAccessor<int, Task<int>>.FromFunc(s => Task.FromResult(s), " with mapping "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Failure, 2 * v, "bar", "inner failure")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(42, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.TryGetValue(out int value)).IsTrue();
		await That(value).IsEqualTo(42);
		await That(sb.ToString()).IsEqualTo("foo with mapping bar");
		await That(result.GetResultText()).IsEqualTo("outer failure and inner failure");
	}

	[Fact]
	public async Task
		IsMetBy_WithAsyncMapping_WhenConstraintAndInnerFailAndWhenBothFailureMessagesAreIdentical_ShouldOnlyPrintOnce()
	{
		ExpectationNode node = new();
		node.AddConstraint(
			new DummyValueConstraint<int>(v
				=> new DummyConstraintResult<int>(Outcome.Failure, v, "foo", "same failure")));
		node.AddAsyncMapping(MemberAccessor<int, Task<int>>.FromFunc(s => Task.FromResult(s), " with mapping "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Failure, 2 * v, "bar", "same failure")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(42, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.TryGetValue(out int value)).IsTrue();
		await That(value).IsEqualTo(42);
		await That(sb.ToString()).IsEqualTo("foo with mapping bar");
		await That(result.GetResultText()).IsEqualTo("same failure");
	}

	[Fact]
	public async Task IsMetBy_WithMapping_WhenConstraintAndInnerFail_ShouldCombineFailureMessage()
	{
		ExpectationNode node = new();
		node.AddConstraint(
			new DummyValueConstraint<int>(v
				=> new DummyConstraintResult<int>(Outcome.Failure, v, "foo", "outer failure")));
		node.AddMapping(MemberAccessor<int, int>.FromFunc(s => s, " with mapping "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Failure, 2 * v, "bar", "inner failure")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(42, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.TryGetValue(out int value)).IsTrue();
		await That(value).IsEqualTo(42);
		await That(sb.ToString()).IsEqualTo("foo with mapping bar");
		await That(result.GetResultText()).IsEqualTo("outer failure and inner failure");
	}

	[Fact]
	public async Task
		IsMetBy_WithMapping_WhenConstraintAndInnerFailAndWhenBothFailureMessagesAreIdentical_ShouldOnlyPrintOnce()
	{
		ExpectationNode node = new();
		node.AddConstraint(
			new DummyValueConstraint<int>(v
				=> new DummyConstraintResult<int>(Outcome.Failure, v, "foo", "same failure")));
		node.AddMapping(MemberAccessor<int, int>.FromFunc(s => s, " with mapping "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Failure, 2 * v, "bar", "same failure")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(42, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.TryGetValue(out int value)).IsTrue();
		await That(value).IsEqualTo(42);
		await That(sb.ToString()).IsEqualTo("foo with mapping bar");
		await That(result.GetResultText()).IsEqualTo("same failure");
	}


	[Fact]
	public async Task IsMetBy_WithUnsupportedConstraint_ShouldThrowInvalidOperationException()
	{
		ExpectationNode node = new();
		node.AddConstraint(new UnsupportedConstraint());

		async Task Act() =>
			await node.IsMetBy("42", null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("The expectation node does not support string with value \"42\"");
	}

	[Theory]
	[InlineData(FurtherProcessingStrategy.Continue, "failure1 and failure2")]
	[InlineData(FurtherProcessingStrategy.IgnoreResult, "failure1")]
	[InlineData(FurtherProcessingStrategy.IgnoreCompletely, "failure1")]
	public async Task MultipleFailures_WithAsyncMapping_ShouldIncludeBothOnlyWhenFurtherProcessingStrategyIsContinue(
		FurtherProcessingStrategy furtherProcessingStrategy, string expectedFailureText)
	{
		DummyConstraint constraint1 = new("",
			() => new DummyConstraintResult(Outcome.Failure, failureText: "failure1",
				furtherProcessingStrategy: furtherProcessingStrategy));
		DummyConstraint constraint2 =
			new("", () => new DummyConstraintResult(Outcome.Failure, failureText: "failure2"));
		ExpectationNode node = new();
		node.AddConstraint(constraint1);
		node.AddAsyncMapping(MemberAccessor<int, Task<int>>.FromFunc(_ => Task.FromResult(0), "length"));
		node.AddConstraint(constraint2);

		ConstraintResult result = await node.IsMetBy(3, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo(expectedFailureText);
	}

	[Theory]
	[InlineData(FurtherProcessingStrategy.Continue, "failure1 and failure2")]
	[InlineData(FurtherProcessingStrategy.IgnoreResult, "failure1")]
	[InlineData(FurtherProcessingStrategy.IgnoreCompletely, "failure1")]
	public async Task MultipleFailures_WithMapping_ShouldIncludeBothOnlyWhenFurtherProcessingStrategyIsContinue(
		FurtherProcessingStrategy furtherProcessingStrategy, string expectedFailureText)
	{
		DummyConstraint constraint1 = new("",
			() => new DummyConstraintResult(Outcome.Failure, failureText: "failure1",
				furtherProcessingStrategy: furtherProcessingStrategy));
		DummyConstraint constraint2 =
			new("", () => new DummyConstraintResult(Outcome.Failure, failureText: "failure2"));
		ExpectationNode node = new();
		node.AddConstraint(constraint1);
		node.AddMapping(MemberAccessor<int, int>.FromFunc(_ => 0, "length"));
		node.AddConstraint(constraint2);

		ConstraintResult result = await node.IsMetBy(3, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo(expectedFailureText);
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task Outcome_WithAsyncMapping_ShouldBeExpected(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		DummyConstraint constraint1 = new("", () => new DummyConstraintResult(node1));
		DummyConstraint constraint2 = new("", () => new DummyConstraintResult(node2));
		ExpectationNode node = new();
		node.AddConstraint(constraint1);
		node.AddAsyncMapping(MemberAccessor<int, Task<int>>.FromFunc(_ => Task.FromResult(0), "length"));
		node.AddConstraint(constraint2);

		ConstraintResult result = await node.IsMetBy(3, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task Outcome_WithMapping_ShouldBeExpected(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		DummyConstraint constraint1 = new("", () => new DummyConstraintResult(node1));
		DummyConstraint constraint2 = new("", () => new DummyConstraintResult(node2));
		ExpectationNode node = new();
		node.AddConstraint(constraint1);
		node.AddMapping(MemberAccessor<int, int>.FromFunc(_ => 0, "length"));
		node.AddConstraint(constraint2);

		ConstraintResult result = await node.IsMetBy(3, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Fact]
	public async Task TryGetValue_WhenLeftHasNoValue_ShouldUseDefaultValue()
	{
		DummyConstraint constraint1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyConstraint constraint2 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 2, ""));
		ExpectationNode node = new();
		node.AddConstraint(constraint1);
		node.AddMapping(MemberAccessor<int, int>.FromFunc(_ => 0, "length"));
		node.AddConstraint(constraint2);
		ConstraintResult constraintResult = await node.IsMetBy(3, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo(3);
	}

	[Fact]
	public async Task TryGetValue_WhenLeftHasValue_ShouldReturnFalse()
	{
		DummyConstraint constraint1 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 1, ""));
		DummyConstraint constraint2 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 2, ""));
		ExpectationNode node = new();
		node.AddConstraint(constraint1);
		node.AddMapping(MemberAccessor<int, int>.FromFunc(_ => 0, "length"));
		node.AddConstraint(constraint2);
		ConstraintResult constraintResult = await node.IsMetBy(3, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo(1);
	}

	[Fact]
	public async Task TryGetValue_WhenNeitherLeftNorRightHasValue_ShouldReturnFalse()
	{
		DummyConstraint constraint1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyConstraint constraint2 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		ExpectationNode node = new();
		node.AddConstraint(constraint1);
		node.AddMapping(MemberAccessor<int, int>.FromFunc(_ => 0, "length"));
		node.AddConstraint(constraint2);
		ConstraintResult constraintResult = await node.IsMetBy(3, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsFalse();
		await That(value).IsNull();
	}

	private sealed class UnsupportedConstraint : IConstraint
	{
		/// <inheritdoc />
		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
	}
}
