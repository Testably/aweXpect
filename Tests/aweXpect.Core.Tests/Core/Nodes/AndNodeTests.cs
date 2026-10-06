using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class AndNodeTests
{
	[Test]
	public async Task AddAsyncMapping_ShouldUseCurrentNode()
	{
		MemberAccessor<string, Task<int>> memberAccessor =
			MemberAccessor<string, Task<int>>.FromExpression(x => Task.FromResult(x.Length));
		DummyNode first = new("foo");
		DummyNode second = new("bar");
		AndNode node = new(first);

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
		AndNode node = new(first);
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
		AndNode node = new(first);

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
		AndNode node = new(first);
		node.AddNode(second);

		node.AddMapping(memberAccessor);

		await That(first.MappingMemberAccessor).IsNull();
		await That(second.MappingMemberAccessor).IsSameAs(memberAccessor);
	}

	[Test]
	public async Task AppendExpectation_WithAdditionalNodes_ShouldUseAllNodes()
	{
		AndNode node = new(new DummyNode("foo"));
		node.AddNode(new DummyNode("bar"));
		node.AddNode(new DummyNode("baz"));
		StringBuilder sb = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo and bar and baz");
	}

	[Test]
	public async Task AppendExpectation_WithCustomSeparators_ShouldUseThem()
	{
		AndNode node = new(new DummyNode("foo"));
		node.AddNode(new DummyNode("bar"), " my ");
		node.AddNode(new DummyNode("baz"), " is ");
		StringBuilder sb = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo my bar is baz");
	}

	[Test]
	public async Task AppendExpectation_WithoutAdditionalNodes_ShouldUseFirstNode()
	{
		AndNode node = new(new DummyNode("foo"));
		StringBuilder sb = new();

		node.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo");
	}

	[Test]
	public async Task Equals_IfCurrentNodeIsDifferent_ShouldBeFalse()
	{
		DummyNode innerNode1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode innerNode2 = new("2", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		AndNode node1 = new(innerNode1);
		AndNode node2 = new(innerNode2);

		bool result = node1.Equals(node2);

		await That(result).IsFalse();
		await That(node1.GetHashCode()).IsNotEqualTo(node2.GetHashCode());
	}

	[Test]
	public async Task Equals_IfCurrentNodeIsTheSame_ShouldBeTrue()
	{
		DummyNode innerNode1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode innerNode2 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		AndNode node1 = new(innerNode1);
		AndNode node2 = new(innerNode2);

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
		AndNode node1 = new(innerNode0);
		node1.AddNode(innerNode1);
		node1.AddNode(currentNode);
		AndNode node2 = new(innerNode0);
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
		AndNode node1 = new(innerNode1);
		node1.AddNode(innerNode3);
		node1.AddNode(currentNode);
		AndNode node2 = new(innerNode2);
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
		AndNode node = new(inner);
		object other = new OrNode(inner);

		bool result = node.Equals(other);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_WhenOtherIsNull_ShouldBeFalse()
	{
		AndNode node = new(new DummyNode("foo"));

		bool result = node.Equals(null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task FailureCause_WhenLeftFailedDueToException_ShouldForwardException()
	{
		Exception exception = new("foo");
		AndNode node = new(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure, "left"), exception, "it")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "right")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.FailureCause).IsSameAs(exception);
	}

	[Test]
	public async Task FailureCause_WhenRightFailedDueToException_ShouldForwardException()
	{
		Exception exception = new("foo");
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "left")));
		node.AddNode(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure, "right"), exception, "it")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.FailureCause).IsSameAs(exception);
	}

	[Test]
	public async Task FailureCause_WhenSuccessful_ShouldBeNull()
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "left")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "right")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.FailureCause).IsNull();
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Success)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Success)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task NegatedOutcome_ShouldBeExpected(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(node1)));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2)));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.FailureBothWays)]
	[Arguments(Outcome.Failure, Outcome.Success)]
	public async Task NegatedOutcome_WhenOperandStaysFailedUnderNegation_ShouldOnlySucceedIfOtherOperandSucceeds(
		Outcome other, Outcome expectedOutcome)
	{
		AndNode node = new(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure), new Exception("foo"), "it")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(other)));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	public async Task NegatedResult_ShouldUseOrAsSeparator()
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "foo")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "bar")));
		StringBuilder sb1 = new();
		StringBuilder sb2 = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.AppendExpectation(sb1);

		result.Negate();

		result.AppendExpectation(sb2);
		await That(sb1.ToString()).IsEqualTo("foo and bar");
		await That(sb2.ToString()).IsEqualTo("foo or bar");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, "l and r")]
	[Arguments(Outcome.Success, Outcome.Undecided, "l and r")]
	[Arguments(Outcome.Undecided, Outcome.Success, "l and r")]
	[Arguments(Outcome.Undecided, Outcome.Undecided, "l and r")]
	public async Task NegatedResultText_ShouldBeExpected(Outcome node1, Outcome node2, string expectedResultText)
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(node1, "left", "l")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2, "right", "r")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);
		result.Negate();

		await That(result.GetResultText()).IsEqualTo(expectedResultText)
			.Because("an undecided operand explains why the combination is undecided");
	}

	[Test]
	[Arguments(Outcome.Success, Outcome.Success, Outcome.Success)]
	[Arguments(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[Arguments(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[Arguments(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task Outcome_ShouldBeExpected(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(node1)));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2)));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Test]
	[Arguments(" that ", "whose bar", "foo whose bar")]
	[Arguments(" that ", "is bar", "foo that is bar")]
	[Arguments(" that ", "whosever bar", "foo that whosever bar")]
	[Arguments(" and ", "whose bar", "foo and whose bar")]
	public async Task Result_WithSeparator_ShouldOnlyDropWhichBeforeWhose(
		string separator, string rightExpectation, string expectedExpectation)
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "foo")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, rightExpectation)),
			separator);

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.GetExpectationText()).IsEqualTo(expectedExpectation);
	}

	[Test]
	[Arguments(Outcome.Failure, Outcome.Success, "l")]
	[Arguments(Outcome.Success, Outcome.Failure, "r")]
	[Arguments(Outcome.Failure, Outcome.Failure, "l and r")]
	[Arguments(Outcome.Failure, Outcome.Undecided, "l")]
	[Arguments(Outcome.Undecided, Outcome.Failure, "r")]
	[Arguments(Outcome.Success, Outcome.Undecided, "r")]
	[Arguments(Outcome.Undecided, Outcome.Success, "l")]
	[Arguments(Outcome.Undecided, Outcome.Undecided, "l and r")]
	public async Task ResultText_ShouldBeExpected(Outcome node1, Outcome node2, string expectedResultText)
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(node1, "left", "l")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(node2, "right", "r")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo(expectedResultText)
			.Because("an undecided operand only explains the combination when no operand failed");
	}

	[Test]
	public async Task ResultText_WhenLeftSpansMultipleLines_ShouldStartRightOnItsOwnLine()
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "left", "l1\nl2")));
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
		AndNode node = new(new DummyNode("",
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
	public async Task TryGetValue_WhenBothHaveValue_ShouldReturnRightValue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 1, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 2, ""));
		AndNode andNode = new(node1);
		andNode.AddNode(node2);
		ConstraintResult constraintResult = await andNode.IsMetBy(0, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int value);

		await That(result).IsTrue();
		await That(value).IsEqualTo(2);
	}

	[Test]
	public async Task TryGetValue_WhenNoneHasValue_ShouldReturnFalse()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		AndNode andNode = new(node1);
		andNode.AddNode(node2);
		ConstraintResult constraintResult = await andNode.IsMetBy(0, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int? value);

		await That(result).IsFalse();
		await That(value).IsNull();
	}

	[Test]
	public async Task TryGetValue_WhenOnlyRightHasValue_ShouldReturnRightValue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<int>(Outcome.Success, 2, ""));
		AndNode andNode = new(node1);
		andNode.AddNode(node2);
		ConstraintResult constraintResult = await andNode.IsMetBy(0, null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int value);

		await That(result).IsTrue();
		await That(value).IsEqualTo(2);
	}

	[Test]
	public async Task WithCustomSeparator_ShouldUseItInsteadOfOr()
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "foo", "-")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "bar", "-")), " my ");
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo my bar");
	}

	[Test]
	public async Task WithCustomSeparators_ShouldUseItInsteadOfOr()
	{
		AndNode node = new(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "foo", "-")));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "bar", "-")), " my ");
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "baz", "-")), " is ");
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo my bar is baz");
	}

	[Test]
	public async Task WithFirstFailedTests_ShouldIncludeSingleFailureInMessage()
	{
		async Task Act()
			=> await That(true).IsFalse().And.IsTrue();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that true
			             is False and is True,
			             but it was True
			             """);
	}

	[Test]
	public async Task WithMultiLineFailures_ShouldStartTheSecondResultOnItsOwnLine()
	{
		async Task Act()
			=> await That("foo").IsEqualTo("bar").And.IsEqualTo("baz");

		await That(Act).Throws()
			.WithMessage("""
			             Expected that "foo"
			             is equal to "bar" and is equal to "baz",
			             but it was "foo", which differs at index 0:
			                ↓ (actual)
			               "foo"
			               "bar"
			                ↑ (expected)
			             and it was "foo", which differs at index 0:
			                ↓ (actual)
			               "foo"
			               "baz"
			                ↑ (expected)
			             """);
	}

	[Test]
	public async Task WithMultipleFailedTests_ShouldIncludeAllFailuresInMessage()
	{
		async Task Act()
			=> await That(true).IsFalse().And.IsFalse().And.Implies(false);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that true
			             is False and is False and implies False,
			             but it was True and did not
			             """);
	}

	[Test]
	public async Task WithPartNotStartingWithSubjectInBetween_ShouldRepeatSubjectAfterIt()
	{
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).HasItem(9).AtIndex(0).And.All().AreEqualTo(9).And.HasItem(8).AtIndex(1);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that subject
			             has an item equal to 9 at index 0 and is equal to 9 for all items and has an item equal to 8 at index 1,
			             but it had item 1 at index 0 and none of 3 were and it had item 2 at index 1

			             Not matching items:
			             [1, 2, 3]

			             Collection:
			             [1, 2, 3]
			             """);
	}

	[Test]
	public async Task WithProcessingStrategy_ShouldConsiderIgnoreCompletely()
	{
		AndNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "foo")));
		node.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "bar", null, FurtherProcessingStrategy.IgnoreCompletely)));
		node.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "baz", "-")));
		StringBuilder sb = new();

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(sb.ToString()).IsEqualTo("foo and bar");
		await That(result.Outcome).IsEqualTo(Outcome.Success);
	}

	[Test]
	public async Task WithProcessingStrategy_WhenFailedOperandIgnoresResult_ShouldOnlyUseExpectationOfFollowingOperands()
	{
		AndNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "foo", "l", FurtherProcessingStrategy.IgnoreResult)));
		node.AddNode(new DummyNode("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure, "bar", "r"), new Exception("baz"), "it")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetExpectationText()).IsEqualTo("foo and bar");
		await That(result.GetResultText()).IsEqualTo("l");
		await That(result.FailureCause).IsNull()
			.Because("the following operands are only evaluated for their expectation text");
	}

	[Test]
	public async Task WithProcessingStrategy_WhenSucceededOperandIgnoresResult_ShouldEvaluateFollowingOperands()
	{
		AndNode node = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "foo", "l", FurtherProcessingStrategy.IgnoreResult)));
		node.AddNode(new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "bar", "r")));

		ConstraintResult result = await node.IsMetBy(0, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Failure);
		await That(result.GetResultText()).IsEqualTo("r");
	}

	[Test]
	public async Task WithSecondFailedTests_ShouldIncludeSingleFailureInMessage()
	{
		async Task Act()
			=> await That(true).IsTrue().And.IsFalse();

		await That(Act).Throws()
			.WithMessage("""
			             Expected that true
			             is True and is False,
			             but it was True
			             """);
	}

	[Test]
	public async Task WithTwoSuccessfulTests_ShouldNotThrow()
	{
		async Task Act()
			=> await That(true).IsTrue().And.IsNotEqualTo(false);

		await That(Act).DoesNotThrow();
	}
}
