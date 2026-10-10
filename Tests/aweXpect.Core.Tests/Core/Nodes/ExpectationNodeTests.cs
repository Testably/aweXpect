using System.Collections.Generic;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public class ExpectationNodeTests
{
	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Success)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
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

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Undecided, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Success, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task AddAsyncMapping_ShouldUseAndCombination(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(node2, "foo2", "bar2")));
		StringBuilder expectationSb = new();
		StringBuilder resultSb = new();
		string expectedResult = (node1, node2) switch
		{
			(Outcome.Failure, Outcome.Failure) or (Outcome.Undecided, Outcome.Undecided) => "bar1 and bar2",
			(Outcome.Failure, _) or (Outcome.Undecided, Outcome.Success) => "bar1",
			(_, Outcome.Failure) or (Outcome.Success, Outcome.Undecided) => "bar2",
			(_, _) => "",
		};

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.AppendExpectation(expectationSb);
		result.AppendResult(resultSb);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(expectationSb.ToString()).IsEqualTo("foo1 length: foo2");
		await That(resultSb.ToString()).IsEqualTo(expectedResult);
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task AddAsyncMapping_WhenNestedMemberIsNull_NegatedResult_ShouldOnlySucceedIfConstraintFails(
		Outcome node1, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<string, Task<string?>>.FromFunc(_ => Task.FromResult<string?>(null), " inner: "))
			.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new NotEvaluatedConstraint<int>("foo2", "not foo2"));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		if (expectedOutcome == Outcome.Failure)
		{
			await That(result.GetResultText()).IsEqualTo("bar1 and it was <null>");
		}
	}

	[Test]
	[Arguments(false, Outcome.Failure)]
	[Arguments(true, Outcome.Success)]
	public async Task AddAsyncMapping_WhenValueHasOtherType_ShouldOnlyUseTheConstraint(bool negate,
		Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(Outcome.Failure, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<int, Task<int>>.FromFunc(i => Task.FromResult(i * 2), " doubled: "))
			.AddConstraint(new NotEvaluatedConstraint<int>("foo2", "not foo2"));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		if (negate)
		{
			result.Negate();
		}

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(result.GetExpectationText()).IsEqualTo("foo1 doubled: foo2");
		await That(result.GetResultText()).IsEqualTo(negate ? "" : "bar1");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task AddAsyncMapping_WhenSubjectIsNull_NegatedResult_ShouldOnlySucceedIfConstraintFails(
		Outcome node1, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddAsyncMapping(MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(Outcome.Success, "foo2", "bar2")));

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
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

	[Test]
	public async Task AddConstraint_Twice_ShouldThrowInvalidOperationException()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));

		void Act() => node.AddConstraint(new DummyConstraint("bar"));

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage(
				"You have to specify how to combine the expectations. Use `And()` or `Or()` in between adding expectations.");
	}

	[Test]
	public async Task AddConstraint_WithAsyncMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddAsyncMapping(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo with length bar");
	}


	[Test]
	public async Task AddConstraint_WithAsyncNarrowingMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddAsyncNarrowingMapping<string, object?, int>(
			MemberAccessor<string, Task<object?>>.FromFunc(s => Task.FromResult<object?>(s.Length), " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo with length bar");
	}

	[Test]
	public async Task AddConstraint_WithMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo with length bar");
	}

	[Test]
	public async Task AddConstraint_WithNarrowingMapping_ShouldForwardToInnerNode()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddNarrowingMapping<string, object?, int>(
			MemberAccessor<string, object?>.FromFunc(s => s.Length, " with length "));
		StringBuilder sb = new();

		node.AddConstraint(new DummyConstraint("bar"));

		node.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo with length bar");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Success)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
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

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Undecided, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Success, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task AddMapping_ShouldUseAndCombination(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(node2, "foo2", "bar2")));
		StringBuilder expectationSb = new();
		StringBuilder resultSb = new();
		string expectedResult = (node1, node2) switch
		{
			(Outcome.Failure, Outcome.Failure) or (Outcome.Undecided, Outcome.Undecided) => "bar1 and bar2",
			(Outcome.Failure, _) or (Outcome.Undecided, Outcome.Success) => "bar1",
			(_, Outcome.Failure) or (Outcome.Success, Outcome.Undecided) => "bar2",
			(_, _) => "",
		};

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.AppendExpectation(expectationSb);
		result.AppendResult(resultSb);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(expectationSb.ToString()).IsEqualTo("foo1 length: foo2");
		await That(resultSb.ToString()).IsEqualTo(expectedResult);
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	[Arguments(" that ", "whose bar", "foo whose bar")]
	[Arguments(" that ", "is bar", "foo that is bar")]
	[Arguments(" whose ", "whose bar", "foo whose whose bar")]
	public async Task AddMapping_WhenSeparatorEndsWithThat_ShouldOnlyDropItBeforeWhose(
		string separator, string rightExpectation, string expectedExpectation)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(Outcome.Failure, "foo")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, separator))
			.AddConstraint(new DummyValueConstraint<int>(_
				=> new DummyConstraintResult(Outcome.Failure, rightExpectation)));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);

		await That(result.GetExpectationText()).IsEqualTo(expectedExpectation);
	}

	[Test]
	public async Task AddMapping_WhenSeparatorEndsWithThat_ShouldRenderRightExpectationAfterIt()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(Outcome.Failure, "foo")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " that "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new PrecedingTextConstraintResult()));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);

		await That(result.GetExpectationText()).IsEqualTo("foo that follows \" that \"");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task AddMapping_WhenNestedMemberIsNull_NegatedResult_ShouldOnlySucceedIfConstraintFails(
		Outcome node1, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, string?>.FromFunc(_ => null, " inner: "))
			.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new NotEvaluatedConstraint<int>("foo2", "not foo2"));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		if (expectedOutcome == Outcome.Failure)
		{
			await That(result.GetResultText()).IsEqualTo("bar1 and it was <null>");
		}
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task AddMapping_WithExpectationOnlyMember_NegatedResult_ShouldOnlyNegateTheConstraint(
		Outcome node1, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_
				=> new ConstraintResult.ExpectationOnly<int>(ExpectationGrammars.None, "foo2", "not foo2")));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(result.GetExpectationText()).IsEqualTo("foo1 length: foo2");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task AddMapping_WhenMemberResultIsAndWithOperandThatStaysFailed_NegatedResult_ShouldFollowTheAnd(
		Outcome other, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(Outcome.Success, "foo1")));
		AndNode andNode = new(new DummyNode("", () => NullSubjectResult.Create(
			new DummyConstraintResult(Outcome.Success, "foo2"), "")));
		andNode.AddNode(new DummyNode("", () => new DummyConstraintResult(other, "foo3")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: ")).AddNode(andNode);

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task
		AddMapping_WhenMemberResultIsFailedOrWithOperandThatStaysFailed_NegatedResult_ShouldOnlySucceedIfConstraintFails(
			Outcome left, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(left, "foo1")));
		OrNode orNode = new(new DummyNode("", () => NullSubjectResult.Create(
			new DummyConstraintResult(Outcome.Success, "foo2"), "")));
		orNode.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "foo3")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: ")).AddNode(orNode);

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task
		AddMapping_WhenUserCodeOfConstraintThrows_NegatedResult_ShouldOnlySucceedIfMemberExpectationFails(
			Outcome member, Outcome expectedOutcome)
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new UserCodeConstraint<string>(() => throw exception, "foo1", "not foo1"));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(member, "foo2", "bar2")));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		if (expectedOutcome == Outcome.Failure)
		{
			await That(result.FailureCause).IsSameAs(exception);
		}
	}

	[Test]
	[Arguments(false, Outcome.Failure)]
	[Arguments(true, Outcome.Success)]
	public async Task AddMapping_WhenValueHasOtherType_ShouldOnlyUseTheConstraint(bool negate, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(Outcome.Failure, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<int, int>.FromFunc(i => i * 2, " doubled: "))
			.AddConstraint(new NotEvaluatedConstraint<int>("foo2", "not foo2"));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);
		if (negate)
		{
			result.Negate();
		}

		await That(result.Outcome).IsEqualTo(expectedOutcome);
		await That(result.GetExpectationText()).IsEqualTo("foo1 doubled: foo2");
		await That(result.GetResultText()).IsEqualTo(negate ? "" : "bar1");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task AddMapping_WhenSubjectIsNull_NegatedResult_ShouldOnlySucceedIfConstraintFails(
		Outcome node1, Outcome expectedOutcome)
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => new DummyConstraintResult(node1, "foo1", "bar1")));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, " length: "))
			.AddConstraint(new DummyValueConstraint<int>(_ => new DummyConstraintResult(Outcome.Success, "foo2", "bar2")));

		ConstraintResult result = await node.IsMetBy<string?>(null, null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
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

	[Test]
	public async Task AddNode_ShouldThrowNotSupportedException()
	{
		ExpectationNode node = new();

		void Act() => node.AddNode(new DummyNode("foo"));

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Don't specify the inner node for Expectation nodes directly. Use AddMapping() instead.");
	}

	[Test]
	public async Task AppendExpectation_Empty_ShouldReturnEmptyText()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("");
	}

	[Test]
	public async Task AppendExpectation_WithAsyncConstraintAndWithMapping_ShouldReturnBoth()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddConstraint(new DummyConstraint("foo"));
		node.AddAsyncMapping(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foowith length: ");
	}

	[Test]
	public async Task AppendExpectation_WithAsyncMapping_ShouldReturnMapping()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddAsyncMapping(
			MemberAccessor<string, Task<int>>.FromFunc(s => Task.FromResult(s.Length), "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("with length: ");
	}

	[Test]
	public async Task AppendExpectation_WithConstraint_ShouldReturnConstraint()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddConstraint(new DummyConstraint("foo"));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo");
	}

	[Test]
	public async Task AppendExpectation_WithConstraintAndWithMapping_ShouldReturnBoth()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddConstraint(new DummyConstraint("foo"));
		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foowith length: ");
	}

	[Test]
	public async Task AppendExpectation_WithMapping_ShouldReturnMapping()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();

		node.AddMapping(MemberAccessor<string, int>.FromFunc(s => s.Length, "with length: "));

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("with length: ");
	}

	[Test]
	public async Task AppendExpectation_WithReasons_ShouldAppendThemAfterTheConstraint()
	{
		StringBuilder sb = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));
		node.AddReasons([new BecauseReason("bar"),]);

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo, because bar");
	}

	[Test]
	public async Task Equals_IfConstraintIsDifferent_ShouldBeFalse()
	{
		ExpectationNode node1 = new();
		node1.AddConstraint(new DummyConstraint("foo"));
		ExpectationNode node2 = new();
		node2.AddConstraint(new DummyConstraint("bar"));

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
	}

	[Test]
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

	[Test]
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

	[Test]
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

	[Test]
	[Arguments(false)]
	[Arguments(true)]
	public async Task Equals_IfOnlyOneHasAnInnerNode_ShouldBeFalseBothWays(bool withConstraint)
	{
		ExpectationNode node1 = new();
		ExpectationNode node2 = new();
		if (withConstraint)
		{
			node1.AddConstraint(new DummyConstraint("foo"));
			node2.AddConstraint(new DummyConstraint("foo"));
		}

		node2.AddMapping(new MappingNode<string, int, int>(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " with length ")));

		await That(node1.Equals(node2)).IsFalse();
		await That(node2.Equals(node1)).IsFalse();
	}

	[Test]
	public async Task Equals_WhenOtherIsDifferentNode_ShouldBeFalse()
	{
		ExpectationNode node = new();
		object other = new DummyNode("");

		bool result = node.Equals(other);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_WhenOtherIsMappingNode_ShouldBeFalseBothWays()
	{
		ExpectationNode node = new();
		object other = new MappingNode<string, int, int>(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " with length "));

		await That(node.Equals(other)).IsFalse();
		await That(other.Equals(node)).IsFalse();
	}

	[Test]
	public async Task Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		ExpectationNode node = new();

		bool result = node.Equals(null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task GetHashCode_IfConstraintsAndInnerNodesAreSame_ShouldBeEqual()
	{
		ExpectationNode node1 = new();
		node1.AddConstraint(new DummyConstraint("foo"));
		node1.AddMapping(new MappingNode<string, int, int>(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " with length ")));
		ExpectationNode node2 = new();
		node2.AddConstraint(new DummyConstraint("foo"));
		node2.AddMapping(new MappingNode<string, int, int>(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " with length ")));

		await That(node1.Equals(node2)).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task GetHashCode_IfConstraintsAreSameButInnerNodesAreDifferent_ShouldNotBeEqual()
	{
		ExpectationNode node1 = new();
		node1.AddConstraint(new DummyConstraint("foo"));
		node1.AddMapping(new MappingNode<string, int, int>(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " with length1 ")));
		ExpectationNode node2 = new();
		node2.AddConstraint(new DummyConstraint("foo"));
		node2.AddMapping(new MappingNode<string, int, int>(
			MemberAccessor<string, int>.FromFunc(s => s.Length, " with length2 ")));

		await That(node1.Equals(node2)).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task GetHashCode_IfConstraintsOfDifferentTypesHaveTheSameExpectation_ShouldBeEqual()
	{
		ExpectationNode node1 = new();
		node1.AddConstraint(new DummyConstraint("foo"));
		ExpectationNode node2 = new();
		node2.AddConstraint(new DummyConstraint<int>(_ => true, "foo"));

		await That(node1.Equals(node2)).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task IsMetBy_WhenAsyncConstraintThrowsException_ShouldThrowTheException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncConstraint<int>(_ => Task.FromException<ConstraintResult>(exception)));

		async Task Act() =>
			await node.IsMetBy(44, null!, CancellationToken.None);

		await That(Act).Throws<MyException>()
			.WithMessage("IsMetBy_WhenAsyncConstraintThrowsException_ShouldThrowTheException")
			.Because("only an exception from the code of the caller fails the expectation");
	}

	[Test]
	public async Task IsMetBy_WhenAsyncContextConstraintThrowsException_ShouldThrowTheException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncContextConstraint<int>(_ => Task.FromException<ConstraintResult>(exception)));

		async Task Act() =>
			await node.IsMetBy(45, null!, CancellationToken.None);

		await That(Act).Throws<MyException>()
			.WithMessage("IsMetBy_WhenAsyncContextConstraintThrowsException_ShouldThrowTheException")
			.Because("only an exception from the code of the caller fails the expectation");
	}

	[Test]
	public async Task IsMetBy_WhenAsyncUserCodeIsCancelledWithTheEvaluation_ShouldThrowTheCancellation()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncConstraint<int>(async _ =>
		{
			await Task.Yield();
			return UserCode.Invoke<ConstraintResult>(() => throw new OperationCanceledException("canceled", cts.Token));
		}));

		async Task Act() =>
			await node.IsMetBy(1, null!, cts.Token);

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage("canceled")
			.Because("a requested cancellation aborts the evaluation instead of failing it");
	}

	[Test]
	[Arguments("it", "a value was \"foo\"")]
	[Arguments("Headers", "a value of Headers was \"foo\"")]
	public async Task IsMetBy_WhenConstraintCannotCompareAValueOfADictionary_ShouldNameTheValueInTheReason(string it,
		string expectedResult)
	{
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingConstraint<Dictionary<int, string>>(
			new UserCodeException(new NotComparableException("it was \"foo\"", null))), it);

		ConstraintResult result = await node.IsMetBy(new Dictionary<int, string> { [1] = "foo", }, null!,
			CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo(expectedResult)
			.Because("the strings of a dictionary that a constraint compares are its values");
	}

	[Test]
	[Arguments("it", "an item was \"foo\"")]
	[Arguments("Tags", "an item of Tags was \"foo\"")]
	public async Task IsMetBy_WhenConstraintCannotCompareAnItem_ShouldNameTheItemInTheReason(string it,
		string expectedResult)
	{
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingConstraint<string[]>(
			new UserCodeException(new NotComparableException("it was \"foo\"", null))), it);

		ConstraintResult result = await node.IsMetBy(new[] { "foo", }, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo(expectedResult)
			.Because("the compared string is an item of the collection, not the collection itself");
	}

	[Test]
	[Arguments(false)]
	[Arguments(true)]
	public async Task IsMetBy_WhenConstraintCannotCompareTheSubject_ShouldFailBothWaysWithTheReason(bool hasCause)
	{
		FormatException? cause = hasCause ? new FormatException("no number") : null;
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingConstraint<int>(
			new UserCodeException(new NotComparableException("it was \"foo\",\nwhich is no number", cause))));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);

		result.AppendResult(sb, "  ");
		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(result.Negate().Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(result.GetExpectationText()).IsEqualTo("throws");
		await That(result.FailureCause).IsSameAs(cause)
			.Because("only the cause that the match type gave explains the failure");
		await That(sb.ToString()).IsEqualTo("it was \"foo\",\n  which is no number")
			.Because("the reason is the result and is indented like any other multi-line result");
	}

	[Test]
	[Arguments("item \"foo\" is no number")]
	[Arguments("its value \"foo\" is no number")]
	[Arguments("It was \"foo\", which is no number")]
	[Arguments("\"foo\" is no number")]
	public async Task IsMetBy_WhenConstraintCannotCompareTheSubject_ShouldKeepAReasonThatDoesNotStartWithIt(
		string reason)
	{
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingConstraint<string>(
			new UserCodeException(new NotComparableException(reason, null))), "Json");

		ConstraintResult result = await node.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo(reason)
			.Because("only a leading \"it\" stands for the compared string");
	}

	[Test]
	public async Task IsMetBy_WhenConstraintCannotCompareTheSubject_ShouldNameTheMemberInTheReason()
	{
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingConstraint<string>(
			new UserCodeException(new NotComparableException("it was \"foo\", which is no number", null))), "Json");

		ConstraintResult result = await node.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("Json was \"foo\", which is no number")
			.Because("the reason names the compared string like the constraint does");
	}

	[Test]
	public async Task IsMetBy_WhenConstraintDoesNotAnswerAnItem_ShouldFailWithTheItemResult()
	{
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingConstraint<int>(new UnansweredItemException(
			new DummyConstraintResult(Outcome.FailureBothWays, "is valid", "it was broken"), "foo")));

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(result.GetExpectationText()).IsEqualTo("throws");
		await That(result.GetResultText()).IsEqualTo("for item \"foo\", it was broken")
			.Because("an item without a known index is named by its value");
	}

	[Test]
	public async Task IsMetBy_WhenContextConstraintThrowsException_ShouldThrowTheException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyContextConstraint<int>(_ => throw exception));

		async Task Act() =>
			await node.IsMetBy(43, null!, CancellationToken.None);

		await That(Act).Throws<MyException>()
			.WithMessage("IsMetBy_WhenContextConstraintThrowsException_ShouldThrowTheException")
			.Because("only an exception from the code of the caller fails the expectation");
	}

	[Test]
	public async Task IsMetBy_WhenExpectationTextConstraintDoesNotAnswerAnItem_ShouldUseItsExpectationResult()
	{
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingExpectationTextConstraint<int>(new UnansweredItemException(
			new DummyConstraintResult(Outcome.FailureBothWays, "is valid", "it was broken"), "foo", 2)));

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(result.GetExpectationText()).IsEqualTo("the expectation result")
			.Because("the constraint provides its expectation as a separate result");
		await That(result.GetResultText()).IsEqualTo("for the item at index 2, it was broken");
	}

	[Test]
	public async Task IsMetBy_WhenMemberThrows_ShouldNotUseTheDescriptionOfTheMemberConstraint()
	{
		ExpectationNode node = new();
		node.AddMapping(MemberAccessor<string, int>.FromFunc(_ => throw new MyException(), " length: "))
			.AddConstraint(new DescribingConstraint<int>("the member"));

		ConstraintResult result = await node.IsMetBy("foobar", null!, CancellationToken.None);

		await That(result.TryGetValue(out IDescribableSubject? _)).IsFalse()
			.Because("the constraint on the member was not evaluated, so it cannot describe the subject");
	}

	[Test]
	public async Task IsMetBy_WhenNoConstraintSupportsAFaultedDelegateValue_ShouldFailWithTheException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new UserCodeConstraint<string>(() => true, "yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(new DelegateValue(exception, TimeSpan.Zero), null!,
			CancellationToken.None);

		result.AppendExpectation(sb);
		sb.Append(", but ");
		result.AppendResult(sb);
		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays)
			.Because("the subject threw instead of providing a value the constraint could verify");
		await That(result.FailureCause).IsSameAs(exception);
		await That(sb.ToString()).IsEqualTo("""
		                                    yeah!, but it did throw a MyException:
		                                      IsMetBy_WhenNoConstraintSupportsAFaultedDelegateValue_ShouldFailWithTheException
		                                    """);
	}

	[Test]
	public async Task IsMetBy_WhenOnlyTheExpectationTextIsEvaluated_ShouldHaveNoResultText()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyConstraint("foo"));

		ConstraintResult result = await node.IsMetBy(1, ExpectationTextEvaluationContext.For(null),
			CancellationToken.None);

		await That(result.GetExpectationText()).IsEqualTo("foo");
		await That(result.GetResultText()).IsEqualTo("")
			.Because("the constraint was not evaluated");
	}

	[Test]
	public async Task IsMetBy_WhenUserCodeIsCancelledWithTheEvaluation_ShouldThrowTheCancellation()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		ExpectationNode node = new();
		node.AddConstraint(new UserCodeConstraint<int>(() => throw new OperationCanceledException("canceled", cts.Token),
			"yeah!", "not yeah!"));

		async Task Act() =>
			await node.IsMetBy(1, null!, cts.Token);

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage("canceled")
			.Because("a requested cancellation aborts the evaluation instead of failing it");
	}

	[Test]
	public async Task IsMetBy_WhenUserCodeThrows_ShouldFailWithTheException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new UserCodeConstraint<int>(() => throw exception, "yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		sb.Append(", but ");
		result.AppendResult(sb);
		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(result.FailureCause).IsSameAs(exception);
		await That(sb.ToString()).IsEqualTo("""
		                                    yeah!, but it did throw a MyException:
		                                      IsMetBy_WhenUserCodeThrows_ShouldFailWithTheException
		                                    """);
	}

	[Test]
	public async Task IsMetBy_WhenUserCodeThrows_WhenNegated_ShouldNegateExpectationAndStillFail()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new UserCodeConstraint<int>(() => throw exception, "yeah!", "not yeah!"));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.FailureBothWays)
			.Because("code that threw answered nothing, so the negation fails as well");
		await That(negated.FailureCause).IsSameAs(exception);
		await That(sb.ToString()).IsEqualTo("not yeah!");
	}

	[Test]
	public async Task IsMetBy_WhenValueConstraintThrowsException_ShouldThrowTheException()
	{
		MyException exception = new();
		ExpectationNode node = new();
		node.AddConstraint(new DummyValueConstraint<string>(_ => throw exception));

		async Task Act() =>
			await node.IsMetBy("42", null!, CancellationToken.None);

		await That(Act).Throws<MyException>()
			.WithMessage("IsMetBy_WhenValueConstraintThrowsException_ShouldThrowTheException")
			.Because("only an exception from the code of the caller fails the expectation");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task IsMetBy_WithAsyncMapping_WhenConstraintFailureSpansMultipleLines_ShouldStartInnerFailureOnItsOwnLine()
	{
		ExpectationNode node = new();
		node.AddConstraint(
			new DummyValueConstraint<int>(v
				=> new DummyConstraintResult<int>(Outcome.Failure, v, "foo", "outer\nfailure")));
		node.AddAsyncMapping(MemberAccessor<int, Task<int>>.FromFunc(s => Task.FromResult(s), " with mapping "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Failure, 2 * v, "bar", "inner failure")));

		ConstraintResult result = await node.IsMetBy(42, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("""
		                                             outer
		                                             failure
		                                             and inner failure
		                                             """).IgnoringNewlineStyle()
			.Because("the inner failure must not be glued onto the last line of the outer failure");
	}

	[Test]
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

	[Test]
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

	[Test]
	public async Task IsMetBy_WithMapping_WhenConstraintFailureSpansMultipleLines_ShouldStartInnerFailureOnItsOwnLine()
	{
		ExpectationNode node = new();
		node.AddConstraint(
			new DummyValueConstraint<int>(v
				=> new DummyConstraintResult<int>(Outcome.Failure, v, "foo", "outer\nfailure")));
		node.AddMapping(MemberAccessor<int, int>.FromFunc(s => s, " with mapping "));
		node.AddConstraint(new DummyValueConstraint<int>(v
			=> new DummyConstraintResult<int>(Outcome.Failure, 2 * v, "bar", "inner failure")));

		ConstraintResult result = await node.IsMetBy(42, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("""
		                                             outer
		                                             failure
		                                             and inner failure
		                                             """).IgnoringNewlineStyle()
			.Because("the inner failure must not be glued onto the last line of the outer failure");
	}


	[Test]
	public async Task IsMetBy_WithReason_WhenAsyncConstraintFails_ShouldAppendTheReason()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncConstraint<int>(_
			=> Task.FromResult<ConstraintResult>(new DummyConstraintResult(Outcome.Failure, "foo", "bar"))));
		node.AddReasons([new AsyncBecauseReason(Task.FromResult<string?>("baz")),]);

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText()).IsEqualTo("foo, because baz");
	}

	[Test]
	public async Task IsMetBy_WithReason_WhenAsyncContextConstraintFails_ShouldAppendTheReason()
	{
		ExpectationNode node = new();
		node.AddConstraint(new DummyAsyncContextConstraint<int>(_
			=> Task.FromResult<ConstraintResult>(new DummyConstraintResult(Outcome.Failure, "foo", "bar"))));
		node.AddReasons([new BecauseReason("baz"),]);

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText()).IsEqualTo("foo, because baz");
	}

	[Test]
	public async Task IsMetBy_WithReason_WhenConstraintDoesNotAnswerAnItem_ShouldFailWithTheItemResult()
	{
		ExpectationNode node = new();
		node.AddConstraint(new ThrowingConstraint<int>(new UnansweredItemException(
			new DummyConstraintResult(Outcome.FailureBothWays, "is valid", "it was broken"), "foo", 3)));
		node.AddReasons([new BecauseReason("baz"),]);

		ConstraintResult result = await node.IsMetBy(1, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(result.GetExpectationText()).IsEqualTo("throws, because baz");
		await That(result.GetResultText()).IsEqualTo("for the item at index 3, it was broken");
	}

	[Test]
	public async Task IsMetBy_WithReason_WhenUserCodeIsCancelledWithTheEvaluation_ShouldThrowTheCancellation()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		ExpectationNode node = new();
		node.AddConstraint(new UserCodeConstraint<int>(() => throw new OperationCanceledException("canceled", cts.Token),
			"yeah!", "not yeah!"));
		node.AddReasons([new BecauseReason("baz"),]);

		async Task Act() =>
			await node.IsMetBy(1, null!, cts.Token);

		await That(Act).Throws<OperationCanceledException>()
			.WithMessage("canceled")
			.Because("a requested cancellation aborts the evaluation instead of failing it");
	}

	[Test]
	public async Task IsMetBy_WithUnsupportedConstraint_ShouldThrowInvalidOperationException()
	{
		ExpectationNode node = new();
		node.AddConstraint(new UnsupportedConstraint());

		async Task Act() =>
			await node.IsMetBy("42", null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("The expectation node does not support string with value \"42\".");
	}

	[Test]
	[Arguments(FurtherProcessingStrategy.Continue, "failure1 and failure2")]
	[Arguments(FurtherProcessingStrategy.IgnoreResult, "failure1")]
	[Arguments(FurtherProcessingStrategy.IgnoreCompletely, "failure1")]
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

	[Test]
	[Arguments(FurtherProcessingStrategy.Continue, "failure1 and failure2")]
	[Arguments(FurtherProcessingStrategy.IgnoreResult, "failure1")]
	[Arguments(FurtherProcessingStrategy.IgnoreCompletely, "failure1")]
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

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
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

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
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

	[Test]
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

	[Test]
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

	[Test]
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

	private sealed class PrecedingTextConstraintResult() : ConstraintResult(FurtherProcessingStrategy.Continue)
	{
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			string text = stringBuilder.ToString();
			int start = Math.Max(0, text.Length - 6);
			stringBuilder.Append("follows \"").Append(text, start, text.Length - start).Append('"');
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null) { }

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate() => this;
	}

	private sealed class ThrowingConstraint<T>(Exception exception) : IValueConstraint<T>
	{
		public ConstraintResult IsMetBy(T actual) => throw exception;

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("throws");
	}

	private sealed class ThrowingExpectationTextConstraint<T>(Exception exception)
		: IValueConstraint<T>, IExpectationTextConstraint
	{
		public ValueTask<ConstraintResult> GetExpectationResult(IEvaluationContext context,
			CancellationToken cancellationToken)
			=> new(new ConstraintResult.ExpectationOnly<T>(ExpectationGrammars.None, "the expectation result",
				"not the expectation result"));

		public ConstraintResult IsMetBy(T actual) => throw exception;

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("the constraint itself");
	}

	private sealed class UnsupportedConstraint : IConstraint
	{
		/// <inheritdoc />
		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
	}
}
