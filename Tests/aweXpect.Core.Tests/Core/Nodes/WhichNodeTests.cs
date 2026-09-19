using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class WhichNodeTests
{
	[Fact]
	public async Task AddAsyncMapping_WithInnerNode_ShouldAddMappingToInnerNode()
	{
		MemberAccessor<string, Task<int>> memberAccessor =
			MemberAccessor<string, Task<int>>.FromExpression(s => Task.FromResult(s.Length));
		DummyNode innerNode = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "inner", ""));
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(innerNode);

		whichNode.AddAsyncMapping(memberAccessor);

		await That(innerNode.MappingMemberAccessor).IsSameAs(memberAccessor);
	}

	[Fact]
	public async Task AddAsyncMapping_WithoutInnerNode_ShouldNotThrow()
	{
		MemberAccessor<string, Task<int>> memberAccessor =
			MemberAccessor<string, Task<int>>.FromExpression(s => Task.FromResult(s.Length));
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);

		void Act() => whichNode.AddAsyncMapping(memberAccessor);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task AddConstraint_WithoutInnerNode_ShouldNotThrow()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);

		void Act() => whichNode.AddConstraint(
			new DummyConstraint("c2", () => new DummyConstraintResult<int>(Outcome.Success, 4, "e2")));

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task AddMapping_WithInnerNode_ShouldAddMappingToInnerNode()
	{
		MemberAccessor<string, int> memberAccessor = MemberAccessor<string, int>.FromExpression(s => s.Length);
		DummyNode innerNode = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "inner", ""));
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(innerNode);

		whichNode.AddMapping(memberAccessor);

		await That(innerNode.MappingMemberAccessor).IsSameAs(memberAccessor);
	}

	[Fact]
	public async Task AddMapping_WithoutInnerNode_ShouldNotThrow()
	{
		MemberAccessor<string, int> memberAccessor = MemberAccessor<string, int>.FromExpression(s => s.Length);
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);

		void Act() => whichNode.AddMapping(memberAccessor);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task AppendExpectation_WithInnerNode_ShouldAppendSeparatorAndInnerExpectation()
	{
		DummyNode innerNode = new("inner-node", () => new DummyConstraintResult<string?>(Outcome.Success, "inner", ""));
		WhichNode<string, int> whichNode = new(null, s => s.Length, "foo-separator ");
		whichNode.AddNode(innerNode);
		StringBuilder sb = new();

		whichNode.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo-separator inner-node");
	}

	[Fact]
	public async Task AppendExpectation_WithoutInnerNode_ShouldAppendSeparator()
	{
		WhichNode<string, int> whichNode = new(null, s => s.Length, "foo-separator");
		StringBuilder sb = new();

		whichNode.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("foo-separator");
	}

	[Fact]
	public async Task AppendExpectation_WithoutSeparator_WithInnerNode_ShouldOnlyAppendInnerExpectation()
	{
		DummyNode innerNode = new("inner-node", () => new DummyConstraintResult<string?>(Outcome.Success, "inner", ""));
		WhichNode<string, int> whichNode = new(null, s => s.Length);
		whichNode.AddNode(innerNode);
		StringBuilder sb = new();

		whichNode.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("inner-node");
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Success)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Success)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task CombinedResult_ShouldBeNegatable(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		WhichNode<string, int> whichNode = new(new DummyNode("", () => new DummyConstraintResult(node1)),
			s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("", () => new DummyConstraintResult(node2)));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);
		result.Negate();

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Fact]
	public async Task Equals_IfInnerAreDifferent_ShouldBeFalse()
	{
		DummyNode node1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode node2 = new("2", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		WhichNode<string, int> whichNode1 = new(null, s => s.Length);
		whichNode1.AddNode(node1);
		WhichNode<string, int> whichNode2 = new(null, s => s.Length);
		whichNode2.AddNode(node2);

		bool result = whichNode1.Equals(whichNode2);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_IfInnerAreSame_ShouldBeTrue()
	{
		DummyNode node1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode1 = new(null, s => s.Length);
		whichNode1.AddNode(node1);
		WhichNode<string, int> whichNode2 = new(null, s => s.Length);
		whichNode2.AddNode(node1);

		bool result = whichNode1.Equals(whichNode2);

		await That(result).IsTrue();
		await That(whichNode1.GetHashCode()).IsEqualTo(whichNode2.GetHashCode());
	}

	[Fact]
	public async Task Equals_IfParentsAreBothNull_ShouldBeTrue()
	{
		WhichNode<string, int> whichNode1 = new(null, s => s.Length);
		WhichNode<string, int> whichNode2 = new(null, s => s.Length);

		bool result = whichNode1.Equals(whichNode2);

		await That(result).IsTrue();
	}

	[Fact]
	public async Task Equals_IfParentsAreDifferent_ShouldBeFalse()
	{
		DummyNode node1 = new("1", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode node2 = new("2", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		WhichNode<string, int> whichNode1 = new(node1, s => s.Length);
		WhichNode<string, int> whichNode2 = new(node2, s => s.Length);

		bool result = whichNode1.Equals(whichNode2);

		await That(result).IsFalse();
		await That(whichNode1.GetHashCode()).IsNotEqualTo(whichNode2.GetHashCode());
	}

	[Fact]
	public async Task Equals_IfParentsAreSame_ShouldBeTrue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode1 = new(node1, s => s.Length);
		WhichNode<string, int> whichNode2 = new(node1, s => s.Length);

		bool result = whichNode1.Equals(whichNode2);

		await That(result).IsTrue();
		await That(whichNode1.GetHashCode()).IsEqualTo(whichNode2.GetHashCode());
	}

	[Fact]
	public async Task Equals_IfTypesAreDifferent_ShouldBeFalse()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		WhichNode<string, int> whichNode1 = new(node1, s => s.Length);
		object whichNode2 = new WhichNode<string, string>(node2, s => s.Substring(0, 1));

		bool result = whichNode1.Equals(whichNode2);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task FailureCause_WhenLeftFailedDueToException_ShouldForwardException()
	{
		Exception exception = new("foo");
		DummyNode node1 = new("", () => new ConstraintResult.FromException(
			new DummyConstraintResult(Outcome.Failure, "1"), exception));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Success, "2"));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.FailureCause).IsSameAs(exception);
	}

	[Fact]
	public async Task FailureCause_WhenSuccessful_ShouldBeNull()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success, "1"));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Success, "2"));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.FailureCause).IsNull();
	}

	[Fact]
	public async Task GetResult_WhenBothFailed_ShouldUseOnlyFirst()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Failure, "1", "r1"));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Failure, "2", "r2"));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);
		ConstraintResult constraintResult = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(constraintResult.GetResultText()).IsEqualTo("r1");
	}

	[Fact]
	public async Task GetResult_WhenBothHaveSameFailureText_ShouldOnlyIncludeOnce()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Failure, "1", "same result"));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Failure, "2", "same result"));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);
		ConstraintResult constraintResult = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(constraintResult.GetResultText()).IsEqualTo("same result");
	}

	[Fact]
	public async Task GetResult_WithIgnoreResultFurtherProcessingStrategy_ShouldOnlyIncludeFirstFailure()
	{
		DummyNode node1 = new("",
			() => new DummyConstraintResult(Outcome.Failure, "1", "r1", FurtherProcessingStrategy.IgnoreResult));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Failure, "2", "r2"));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);
		ConstraintResult constraintResult = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(constraintResult.GetResultText()).IsEqualTo("r1");
	}

	[Fact]
	public async Task IsMetBy_EmptyExpectationNode_ShouldThrowInvalidOperationException()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		Task<ConstraintResult> Act() => whichNode.IsMetBy("foo", null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("The expectation node does not support int with value 1");
	}

	[Fact]
	public async Task IsMetBy_ExpectationNodeWithConstraint_ShouldApplyConstraint()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", "e1"));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("c2",
			() => new DummyConstraintResult<int>(Outcome.Success, 4, "e2")));
		StringBuilder sb = new();

		ConstraintResult result = await whichNode.IsMetBy("foo", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(sb.ToString()).IsEqualTo("e1e2");
	}

	[Fact]
	public async Task IsMetBy_WhenOuterTypeDoesNotMatchButParentExposesProjectedValue_ShouldFallBackToParentProjection()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "abcd", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		int? observed = null;
		whichNode.AddConstraint(new DummyConstraint<int>(i =>
		{
			observed = i;
			return true;
		}, "is anything"));

		ConstraintResult result = await whichNode.IsMetBy(DateTime.Now, null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(observed).IsEqualTo(4);
	}

	[Fact]
	public async Task IsMetBy_WhenParentChainProjectsThroughThreeLevels_ShouldPropagateInnermostValue()
	{
		WhichNode<string, char> level1 = new(null, s => s![0]);
		level1.AddNode(new ExpectationNode());
		level1.AddConstraint(new DummyConstraint<char>(_ => true, "first"));

		WhichNode<char, int> level2 = new(level1, c => c);
		level2.AddNode(new ExpectationNode());
		level2.AddConstraint(new DummyConstraint<int>(_ => true, "code"));

		WhichNode<int, bool> level3 = new(level2, i => i % 2 == 0);
		level3.AddNode(new ExpectationNode());
		bool? observed = null;
		level3.AddConstraint(new DummyConstraint<bool>(b =>
		{
			observed = b;
			return b;
		}, "is true"));

		ConstraintResult result = await level3.IsMetBy("foo", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(observed).IsEqualTo(true);
	}

	[Fact]
	public async Task IsMetBy_WhenParentWhichNodeProjectsToInnerSource_ShouldEvaluateAgainstProjectedValue()
	{
		WhichNode<string, int> outerWhich = new(null, s => s!.Length);
		outerWhich.AddNode(new ExpectationNode());
		outerWhich.AddConstraint(new DummyConstraint<int>(_ => true, "length"));

		bool? observed = null;
		WhichNode<int, bool> innerWhich = new(outerWhich, i => i % 2 == 0);
		innerWhich.AddNode(new ExpectationNode());
		innerWhich.AddConstraint(new DummyConstraint<bool>(b =>
		{
			observed = b;
			return b;
		}, "is true"));

		ConstraintResult result = await innerWhich.IsMetBy("food", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(observed).IsEqualTo(true);
	}

	[Fact]
	public async Task IsMetBy_WhenTypeDoesNotMatch_ShouldThrowInvalidOperationException()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("c2",
			() => new DummyConstraintResult<int>(Outcome.Success, 4, "e2")));

		async Task Act()
			=> await whichNode.IsMetBy(DateTime.Now, null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("""
			             The member type for the actual value in the which node did not match.
			                  Found: DateTime
			               Expected: string
			             """);
	}

	[Theory]
	[InlineData(Outcome.Success)]
	[InlineData(Outcome.Failure)]
	public async Task IsMetBy_WithNullValue_ShouldFailAlsoWhenNegated(Outcome parentOutcome)
	{
		WhichNode<string, int> whichNode = new(new DummyNode("", () => new DummyConstraintResult(parentOutcome)),
			s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("", () => new DummyConstraintResult(Outcome.Success)));

		ConstraintResult result = await whichNode.IsMetBy<string?>(null, null!, CancellationToken.None);
		Outcome outcome = result.Outcome;
		result.Negate();

		await That(outcome).IsEqualTo(Outcome.Failure);
		await That(result.Outcome).IsEqualTo(Outcome.Failure);
	}

	[Fact]
	public async Task IsMetBy_WithNullValueAndWithoutParent_ShouldFailAlsoWhenNegated()
	{
		WhichNode<string, int> whichNode = new(null, s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new NotEvaluatedConstraint<int>("e2", "not e2"));
		StringBuilder sb = new();

		ConstraintResult result = await whichNode.IsMetBy<string?>(null, null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("not e2");
		await That(negated.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task Negate_ShouldNegateTheWholeExpectation()
	{
		WhichNode<string, int> whichNode = new(
			new DummyNode("", () => new NegatableConstraintResult(Outcome.Success, "1")), s => s.Length, " which ");
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("", () => new NegatableConstraintResult(Outcome.Success)));
		StringBuilder sb = new();

		ConstraintResult result = await whichNode.IsMetBy("foo", null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("not e1 which e2");
		await That(negated.GetResultText()).IsEqualTo("not r1");
	}

	[Fact]
	public async Task Negate_WithNullValue_ShouldNegateTheWholeExpectationAndStayFailed()
	{
		WhichNode<string, int> whichNode = new(
			new DummyNode("", () => new NegatableConstraintResult(Outcome.Failure, "1")), s => s.Length, " which ");
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new NotEvaluatedConstraint<int>("e2", "not e2"));
		StringBuilder sb = new();

		ConstraintResult result = await whichNode.IsMetBy<string?>(null, null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("not e1 which e2");
		await That(negated.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task Negate_WithNegateMemberOnly_ShouldNegateTheContinuedExpectation()
	{
		WhichNode<string, int> whichNode = new(
			new DummyNode("", () => new DummyConstraintResult(Outcome.Success, "e1")), s => s.Length, " which ",
			true);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("", () => new NegatableConstraintResult(Outcome.Success)));
		StringBuilder sb = new();

		ConstraintResult result = await whichNode.IsMetBy("foo", null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("e1 which not e2");
		await That(negated.GetResultText()).IsEqualTo("not r2");
	}

	[Fact]
	public async Task Negate_WithNegateMemberOnlyAndNullValue_ShouldNegateTheContinuedExpectationAndStayFailed()
	{
		WhichNode<string, int> whichNode = new(
			new DummyNode("", () => new DummyConstraintResult(Outcome.Failure, "e1", "it was <null>")), s => s.Length,
			" which ", true);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new NotEvaluatedConstraint<int>("e2", "not e2"));
		StringBuilder sb = new();

		ConstraintResult result = await whichNode.IsMetBy<string?>(null, null!, CancellationToken.None);
		ConstraintResult negated = result.Negate();

		negated.AppendExpectation(sb);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure);
		await That(sb.ToString()).IsEqualTo("e1 which not e2");
		await That(negated.GetResultText()).IsEqualTo("it was <null>");
	}

	[Fact]
	public async Task IsMetBy_WithoutInnerNode_ShouldThrowInvalidOperationException()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		Task<ConstraintResult> Act() => whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("No inner node specified for the which node.");
	}

	[Fact]
	public async Task IsMetBy_WithoutParent_ShouldUseNodeResult()
	{
		WhichNode<string, int> whichNode = new(null, s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("c2",
			() => new DummyConstraintResult<int>(Outcome.Success, 4, "e2")));
		StringBuilder sb = new();

		ConstraintResult result = await whichNode.IsMetBy("foo", null!, CancellationToken.None);

		result.AppendExpectation(sb);
		await That(result.Outcome).IsEqualTo(Outcome.Success);
		await That(sb.ToString()).IsEqualTo("e2");
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.Success, Outcome.Success)]
	[InlineData(Outcome.Failure, Outcome.Success, Outcome.Failure)]
	[InlineData(Outcome.Success, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Failure, Outcome.Undecided, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Failure, Outcome.Failure)]
	[InlineData(Outcome.Undecided, Outcome.Undecided, Outcome.Undecided)]
	public async Task Outcome_ShouldBeExpected(Outcome node1, Outcome node2, Outcome expectedOutcome)
	{
		WhichNode<string, int> whichNode = new(new DummyNode("", () => new DummyConstraintResult(node1)),
			s => s.Length);
		whichNode.AddNode(new ExpectationNode());
		whichNode.AddConstraint(new DummyConstraint("", () => new DummyConstraintResult(node2)));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.Outcome).IsEqualTo(expectedOutcome);
	}

	[Fact]
	public async Task TryGetValue_WhenLeftHasValue_ShouldReturnLeftValue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "1", ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<string?>(Outcome.Success, "2", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);
		ConstraintResult constraintResult = await whichNode.IsMetBy("", null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo("1");
	}

	[Fact]
	public async Task TryGetValue_WhenMemberAccessorHasCorrectValue_ShouldReturnMemberValue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult<string>(Outcome.Success, "1", ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<string>(Outcome.Success, "2", ""));
		WhichNode<string, int> whichNode = new(node1, _ => 3);
		whichNode.AddNode(node2);
		ConstraintResult constraintResult = await whichNode.IsMetBy("", null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out int value);

		await That(result).IsTrue();
		await That(value).IsEqualTo(3);
	}

	[Fact]
	public async Task TryGetValue_WhenNoneHasValue_ShouldReturnFalse()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);
		ConstraintResult constraintResult = await whichNode.IsMetBy("", null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsFalse();
		await That(value).IsNull();
	}

	[Fact]
	public async Task TryGetValue_WhenOnlyRightHasValue_ShouldReturnRightValue()
	{
		DummyNode node1 = new("", () => new DummyConstraintResult(Outcome.Success, ""));
		DummyNode node2 = new("", () => new DummyConstraintResult<string>(Outcome.Success, "2", ""));
		WhichNode<string, int> whichNode = new(node1, s => s.Length);
		whichNode.AddNode(node2);
		ConstraintResult constraintResult = await whichNode.IsMetBy("", null!, CancellationToken.None);

		bool result = constraintResult.TryGetValue(out string? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo("2");
	}

	[Fact]
	public async Task WhenBothAreFailure_ShouldOnlyIncludeLeftResult()
	{
		WhichNode<string, int> whichNode = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "foo", "r1")), _ => 3);
		whichNode.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "bar", "r2")));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("r1");
	}

	[Fact]
	public async Task WhenBothAreSuccess_ShouldHaveEmptyResultText()
	{
		WhichNode<string, int> whichNode = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "foo")), _ => 3);
		whichNode.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "bar")));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.GetResultText()).IsEmpty();
	}

	[Fact]
	public async Task WhenLeftIsFailureAndHasIgnoreResultFurtherProcessingStrategy_ShouldExcludeRightResultText()
	{
		WhichNode<string, int> whichNode = new(new DummyNode("",
				() => new DummyConstraintResult(Outcome.Failure, "foo", "r1", FurtherProcessingStrategy.IgnoreResult)),
			_ => 3);
		whichNode.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "bar", "r2", FurtherProcessingStrategy.IgnoreResult)));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("r1");
	}

	[Fact]
	public async Task WhenLeftIsSuccessAndHasIgnoreResultFurtherProcessingStrategy_ShouldStillIncludeRightResultText()
	{
		WhichNode<string, int> whichNode = new(new DummyNode("",
				() => new DummyConstraintResult(Outcome.Success, "foo", null, FurtherProcessingStrategy.IgnoreResult)),
			_ => 3);
		whichNode.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "bar", "r2", FurtherProcessingStrategy.IgnoreResult)));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("r2");
	}

	[Fact]
	public async Task WhenOnlyRightHasFailure_ShouldIncludeRightResultText()
	{
		WhichNode<string, int> whichNode = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Success, "foo")), _ => 3);
		whichNode.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "bar", "r2")));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.GetResultText()).IsEqualTo("r2");
	}

	[Theory]
	[InlineData(" which ", "whose bar", "foo whose bar")]
	[InlineData(" which ", "is bar", "foo which is bar")]
	[InlineData(" whose value ", "whose bar", "foo whose value whose bar")]
	public async Task WhenSeparatorEndsWithWhich_ShouldOnlyDropItBeforeWhose(
		string separator, string rightExpectation, string expectedExpectation)
	{
		WhichNode<string, int> whichNode = new(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, "foo")), _ => 3, separator);
		whichNode.AddNode(new DummyNode("",
			() => new DummyConstraintResult(Outcome.Failure, rightExpectation)));

		ConstraintResult result = await whichNode.IsMetBy("", null!, CancellationToken.None);

		await That(result.GetExpectationText()).IsEqualTo(expectedExpectation);
	}

	[Fact]
	public async Task WhichCreatesGoodMessage()
	{
		Dummy subject = new()
		{
			Inner = new Dummy.Nested
			{
				Id = 1,
			},
			Value = "foo",
		};

		async Task Act()
			=> await That(subject).Is<Dummy>()
				.Whose(p => p.Value, e => e.IsEqualTo("bar"));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is type WhichNodeTests.Dummy whose Value is equal to "bar",
			             but Value was "foo" which differs at index 0:
			                ↓ (actual)
			               "foo"
			               "bar"
			                ↑ (expected)
			             """);
	}

	[Fact]
	public async Task WhichWithWhose_ShouldNotRepeatConnector()
	{
		string[] subject = ["foo",];

		async Task Act()
			=> await That(subject).HasSingle().Which.Whose(x => x.Length, l => l.IsEqualTo(4));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             has a single item whose Length is equal to 4,
			             but Length was 3 which differs by -1
			             """);
	}

	private sealed class NegatableConstraintResult(Outcome outcome, string id = "2")
		: ConstraintResult(FurtherProcessingStrategy.Continue)
	{
		private bool _isNegated;

		public override Outcome Outcome { get; protected set; } = outcome;

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_isNegated ? "not e" : "e").Append(id);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_isNegated ? "not r" : "r").Append(id);

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}

	private sealed class Dummy
	{
		public Nested? Inner { get; set; }
		public string? Value { get; set; }

		public class Nested
		{
#pragma warning disable CS0649
			public int Field;
#pragma warning restore CS0649
			public int Id { get; set; }

			public int Method() => Id + 1;
		}
	}
}
