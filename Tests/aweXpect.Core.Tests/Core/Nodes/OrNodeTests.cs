using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class OrNodeTests
{
	[Test]
	public async Task AddAsyncMapping_ShouldUseCurrentNode()
	{
		MemberAccessor<string, Task<int>> memberAccessor =
			MemberAccessor<string, Task<int>>.FromExpression(x => Task.FromResult(x.Length));
		DummyNode first = new("foo");
		DummyNode second = new("bar");
		OrNode node = new(first);

		node.AddAsyncMapping(memberAccessor);
		node.AddNode(second);

		await That(first.MappingMemberAccessor).IsSameAs(memberAccessor);
		await That(second.MappingMemberAccessor).IsNull();
	}

	[Test]
	public async Task AddAsyncMapping_ShouldUseSecondNode()
	{
		MemberAccessor<string, Task<int>> memberAccessor =
			MemberAccessor<string, Task<int>>.FromExpression(x => Task.FromResult(x.Length));
		DummyNode first = new("foo");
		DummyNode second = new("bar");
		OrNode node = new(first);
		node.AddNode(second);

		node.AddAsyncMapping(memberAccessor);

		await That(first.MappingMemberAccessor).IsNull();
		await That(second.MappingMemberAccessor).IsSameAs(memberAccessor);
	}

	[Test]
	public async Task AddMapping_ShouldUseCurrentNode()
	{
		MemberAccessor<string, int> memberAccessor = MemberAccessor<string, int>.FromExpression(x => x.Length);
		DummyNode first = new("foo");
		DummyNode second = new("bar");
		OrNode node = new(first);

		node.AddMapping(memberAccessor);
		node.AddNode(second);

		await That(first.MappingMemberAccessor).IsSameAs(memberAccessor);
		await That(second.MappingMemberAccessor).IsNull();
	}

	[Test]
	public async Task AddMapping_ShouldUseSecondNode()
	{
		MemberAccessor<string, int> memberAccessor = MemberAccessor<string, int>.FromExpression(x => x.Length);
		DummyNode first = new("foo");
		DummyNode second = new("bar");
		OrNode node = new(first);
		node.AddNode(second);

		node.AddMapping(memberAccessor);

		await That(first.MappingMemberAccessor).IsNull();
		await That(second.MappingMemberAccessor).IsSameAs(memberAccessor);
	}

	[Test]
	public async Task AppendExpectation_WithAdditionalNodes_ShouldUseAllNodes()
	{
		OrNode node = new(new DummyNode("foo"));
		node.AddNode(new DummyNode("bar"));
		node.AddNode(new DummyNode("baz"));
		StringBuilder sb = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo or bar or baz");
	}

	[Test]
	public async Task AppendExpectation_WithCustomSeparators_ShouldUseThem()
	{
		OrNode node = new(new DummyNode("foo"));
		node.AddNode(new DummyNode("bar"), " my ");
		node.AddNode(new DummyNode("baz"), " is ");
		StringBuilder sb = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo my bar is baz");
	}

	[Test]
	public async Task AppendExpectation_WithoutAdditionalNodes_ShouldUseFirstNode()
	{
		OrNode node = new(new DummyNode("foo"));
		StringBuilder sb = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo");
	}

	[Test]
	public async Task Equals_IfCurrentNodeIsDifferent_ShouldBeFalse()
	{
		DummyNode innerNode1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode innerNode2 = new("2", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		OrNode node1 = new(innerNode1);
		OrNode node2 = new(innerNode2);

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_IfCurrentNodeIsTheSame_ShouldBeTrue()
	{
		DummyNode innerNode1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode innerNode2 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		OrNode node1 = new(innerNode1);
		OrNode node2 = new(innerNode2);

		bool result = node1.Equals(node2);

		await That(result).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_IfInnerNodesAreDifferent_ShouldBeFalse()
	{
		DummyNode innerNode0 = new("0", () => new DummyConstraintResult<string?>(Outcome.Success, "0", ""));
		DummyNode innerNode1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode innerNode2 = new("2", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		DummyNode currentNode = new("3", () => new DummyConstraintResult<string?>(Outcome.Success, "3", ""));
		OrNode node1 = new(innerNode0);
		node1.AddNode(innerNode1);
		node1.AddNode(currentNode);
		OrNode node2 = new(innerNode0);
		node2.AddNode(innerNode2);
		node2.AddNode(currentNode);

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_IfInnerNodesAreSame_ShouldBeTrue()
	{
		DummyNode innerNode1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode innerNode2 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode innerNode3 = new("2", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		DummyNode innerNode4 = new("2", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		DummyNode currentNode = new("3", () => new DummyConstraintResult<string?>(Outcome.Success, "3", ""));
		OrNode node1 = new(innerNode1);
		node1.AddNode(innerNode3);
		node1.AddNode(currentNode);
		OrNode node2 = new(innerNode2);
		node2.AddNode(innerNode4);
		node2.AddNode(currentNode);

		bool result = node1.Equals(node2);

		await That(result).IsTrue();
		await That(node1.GetHashCode()).IsEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_WhenOtherIsDifferentNode_ShouldBeFalse()
	{
		DummyNode inner = new("foo");
		OrNode node = new(inner);
		object other = new AndNode(inner);

		bool result = node.Equals(other);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		OrNode node = new(new DummyNode("foo"));

		bool result = node.Equals(null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task FailureCause_WhenBothFailedButOnlyRightDueToException_ShouldForwardException()
	{
		Exception exception = new("foo");
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "left", "-")));
		node.AddNode(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure, "right"), exception, "it")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.FailureCause).IsSameAs(exception);
	}

	[Test]
	public async Task FailureCause_WhenLeftFailedDueToException_ShouldForwardException()
	{
		Exception exception = new("foo");
		OrNode node = new(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure, "left"), exception, "it")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "right", "-")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.FailureCause).IsSameAs(exception);
	}

	[Test]
	public async Task FailureCause_WhenSuccessful_ShouldBeNull()
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "left")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "right")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.FailureCause).IsNull();
	}

	[Test]
	public async Task NegatedExpectation_WhenFirstNodeSucceeds_ShouldIncludeAllExpectationsInMessage()
	{
		async Task Act()
			=> await That(1).DoesNotComplyWith(it => it.IsEqualTo(1).Or.IsEqualTo(2));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that 1
			             is not equal to 1 and is not equal to 2,
			             but it was 1
			             """);
	}

	[Test]
	public async Task NegatedExpectation_WhenFirstNodeSucceeds_ShouldNotEvaluateSecondNode()
	{
		bool isEvaluated = false;

		async Task Act()
			=> await That(1).DoesNotComplyWith(it => it.IsEqualTo(1).Or.Satisfies(_ => isEvaluated = true));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that 1
			             is not equal to 1 and does not satisfy _ => isEvaluated = true,
			             but it was 1
			             """);
		await That(isEvaluated).IsFalse()
			.Because("the negation of a succeeded branch already fails the combination");
	}

	[Test]
	public async Task NegatedExpectation_WhenFirstNodeSucceeds_ShouldOnlyIncludeItsResult()
	{
		async Task Act()
			=> await That("a").DoesNotComplyWith(it => it.IsEqualTo("a").Or.StartsWith("b"));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that "a"
			             is not equal to "a" and does not start with "b",
			             but it was "a"
			             """)
			.Because("the second node was not evaluated, so it has no result");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Success, Outcome.Undecided, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task NegatedOutcome_ShouldBeExpected(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(node1)));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2)));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.FailureBothWays)]
	public async Task NegatedOutcome_WhenOperandStaysFailedUnderNegation_ShouldFail(Outcome other,
		Outcome expectedOutcome)
	{
		OrNode node = new(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure), new Exception("foo"), "it")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(other)));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome)
			.Because("the negated other operand decides whether the negation could be answered");
	}

	[Test]
	public async Task NegatedResult_ShouldUseAndAsSeparator()
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "foo")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "bar")));
		StringBuilder sb1 = new();
		StringBuilder sb2 = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.AppendExpectation(sb1);

		result.Negate();

		result.AppendExpectation(sb2);
		await That(sb1.ToString()).IsEqualTo("foo or bar");
		await That(sb2.ToString()).IsEqualTo("foo and bar");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, "l")]
	[Arguments(Outcome.Failure, Outcome.Success, "r")]
	[Arguments(Outcome.Success, Outcome.Failure, "l")]
	[Arguments(Outcome.Success, Outcome.Undecided, "l")]
	[Arguments(Outcome.Undecided, Outcome.Success, "r")]
	[Arguments(Outcome.Failure, Outcome.Undecided, "r")]
	[Arguments(Outcome.Undecided, Outcome.Failure, "l")]
	[Arguments(Outcome.Undecided, Outcome.Undecided, "l and r")]
	public async Task NegatedResultText_ShouldBeExpected(Outcome node1, Outcome node2, string expectedResultText)
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(node1, "left", "l")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2, "right", "r")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.GetResultText()).IsEqualTo(expectedResultText)
			.Because("an undecided operand only explains the combination when no operand failed");
	}

	[Test]
	[Arguments(Outcome.Success, FurtherProcessingStrategy.Continue)]
	[Arguments(Outcome.Success, FurtherProcessingStrategy.IgnoreResult)]
	[Arguments(Outcome.Undecided, FurtherProcessingStrategy.Continue)]
	[Arguments(Outcome.Undecided, FurtherProcessingStrategy.IgnoreResult)]
	public async Task NegatedResultText_WhenLeftSucceedsUnderNegation_ShouldIncludeRightResultText(
		Outcome right, FurtherProcessingStrategy leftStrategy)
	{
		OrNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "left", "l", leftStrategy)));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(right, "right", "r")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.GetResultText()).IsEqualTo("r")
			.Because("the strategy of the left operand only suppresses the right result after the left result");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Undecided, Outcome.Success)]
	[Arguments(Outcome.Undecided, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Undecided)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task Outcome_ShouldBeExpected(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(node1)));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2)));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(Outcome.Failure, Outcome.Failure, "l and r")]
	[Arguments(Outcome.Failure, Outcome.Undecided, "l and r")]
	[Arguments(Outcome.Undecided, Outcome.Failure, "l and r")]
	[Arguments(Outcome.Undecided, Outcome.Undecided, "l and r")]
	public async Task ResultText_ShouldBeExpected(Outcome node1, Outcome node2, string expectedResultText)
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(node1, "left", "l")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2, "right", "r")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo(expectedResultText)
			.Because("an undecided operand explains why the combination is undecided");
	}

	[Test]
	public async Task ResultText_WhenLeftSpansMultipleLines_ShouldStartRightOnItsOwnLine()
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "left", "l1\nl2")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "right", "r")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.AppendResult(sb, "  ");

		await That(sb.ToString()).IsEqualTo("""
		                                    l1
		                                    l2
		                                      and r
		                                    """).IgnoringNewlineStyle()
			.Because("the right result must not be glued onto the last line of the left result");
	}

	[Test]
	public async Task ShouldConsiderFurtherProcessingStrategy()
	{
		OrNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "foo", "-")));
		node.AddNode(new DummyNode("",
				() => new DummyConstraintResult(Outcome.Failure, "bar", "-",
					FurtherProcessingStrategy.IgnoreCompletely)),
			" my ");
		node.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "baz", "-")), " is ");
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo my bar");
	}

	[Test]
	public async Task ShouldEvaluateSecondNodeWhenFirstNodeFails()
	{
		bool isEvaluated = false;

		async Task Act()
			=> await That(1).IsEqualTo(2).Or.Satisfies(_ => isEvaluated = true);

		await That(Act).DoesNotThrow();
		await That(isEvaluated).IsTrue()
			.Because("the result is still undecided after the first branch failed");
	}

	[Test]
	public async Task ShouldNotAccessMemberOfSecondNodeWhenFirstNodeSucceeds()
	{
		int accessCount = 0;

		async Task Act()
			=> await That(1).IsEqualTo(1).Or.Whose(x =>
			{
				accessCount++;
				return x;
			}, p => p.IsEqualTo(2));

		await That(Act).DoesNotThrow();
		await That(accessCount).IsEqualTo(0)
			.Because("a branch after a successful one must not access its member");
	}

	[Test]
	public async Task ShouldNotEvaluateSecondNodeWhenFirstNodeSucceeds()
	{
		async Task Act()
			=> await That(1).IsEqualTo(1).Or.Satisfies<int>(_ => throw new MyException());

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task TryGetValue_WhenLeftHasValue_ShouldReturnLeftValue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 1, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 2, ""));
		OrNode orNode = new(node1);
		orNode.AddNode(node2);
		ConstraintResult constraintResult = await orNode.IsMetBy(0, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int value);

		await That(result).IsTrue();
		await That(value).IsEqualTo(1);
	}

	[Test]
	public async Task TryGetValue_WhenNoneHasValue_ShouldReturnFalse()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		OrNode orNode = new(node1);
		orNode.AddNode(node2);
		ConstraintResult constraintResult = await orNode.IsMetBy(0, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int? value);

		await That(result).IsFalse();
		await That(value).IsNull();
	}

	[Test]
	public async Task TryGetValue_WhenOnlyRightHasValue_ShouldReturnRightValue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 2, ""));
		OrNode orNode = new(node1);
		orNode.AddNode(node2);
		ConstraintResult constraintResult = await orNode.IsMetBy(0, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int value);

		await That(result).IsTrue();
		await That(value).IsEqualTo(2);
	}

	[Test]
	public async Task WhenBothAreSuccess_ShouldHaveEmptyResultText()
	{
		OrNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "foo")));
		node.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "bar")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEmpty();
	}

	[Test]
	public async Task WhenLeftIsFailureAndHasIgnoreResultFurtherProcessingStrategy_ShouldExcludeRightResultText()
	{
		OrNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "foo", "r1", FurtherProcessingStrategy.IgnoreResult)));
		node.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "bar", "r2", FurtherProcessingStrategy.IgnoreResult)));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("r1");
	}

	[Test]
	public async Task WhenLeftIsSuccess_ShouldOnlyUseTheExpectationOfTheRightOperand()
	{
		OrNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "foo", "r1")));
		node.AddNode(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure, "bar", "r2"), new Exception("baz"), "it")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText()).IsEqualTo("foo and bar");
		await That(result.GetResultText()).IsEqualTo("r1")
			.Because("the right operand is not evaluated after a successful left operand, so it has no result");
		await That(result.FailureCause).IsNull();
	}

	[Test]
	public async Task WithCustomSeparator_ShouldUseItInsteadOfOr()
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "foo", "-")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "bar", "-")), " my ");
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo my bar");
	}

	[Test]
	public async Task WithCustomSeparators_ShouldUseItInsteadOfOr()
	{
		OrNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "foo", "-")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "bar", "-")), " my ");
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "baz", "-")), " is ");
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo my bar is baz");
	}

	[Test]
	public async Task WithFirstFailedTests_ShouldNotThrow()
	{
		async Task Act()
			=> await That(true).IsFalse().Or.IsTrue();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithMultiLineFailures_ShouldStartTheSecondResultOnItsOwnLine()
	{
		async Task Act()
			=> await That("foo").StartsWith("b").Or.EndsWith("x");

		await That(Act).Throws()
			.WithMessage("""
			             Expected that "foo"
			             starts with "b" or ends with "x",
			             but it was "foo", which differs at index 0:
			                ↓ (actual)
			               "foo"
			               "b"
			                ↑ (expected prefix)
			             and it was "foo", which differs at index 2:
			                  ↓ (actual)
			               "foo"
			                 "x"
			                  ↑ (expected suffix)
			             """);
	}

	[Test]
	public async Task WithMultipleFailedTests_ShouldIncludeAllFailuresInMessage()
	{
		async Task Act()
			=> await That(true).IsFalse().Or.IsFalse().Or.Implies(false);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that true
			             is False or is False or implies False,
			             but it was True and did not
			             """);
	}

	[Test]
	public async Task WithProcessingStrategy_ShouldConsiderIgnoreCompletely()
	{
		OrNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "foo", "r1")));
		node.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "bar", "r2", FurtherProcessingStrategy.IgnoreCompletely)));
		node.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "baz")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo or bar");
		await That(result.GetResultText()).IsEqualTo("r1 and r2");
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
	}

	[Test]
	public async Task WithSecondFailedTests_ShouldNotThrow()
	{
		async Task Act()
			=> await That(true).IsTrue().Or.IsFalse();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WithTwoSuccessfulTests_ShouldNotThrow()
	{
		async Task Act()
			=> await That(true).IsTrue().Or.IsNotEqualTo(false);

		await That(Act).DoesNotThrow();
	}
}
