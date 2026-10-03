using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
using static aweXpect.Core.Constraints.Outcome;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class CombinedResultTests
{
	[Theory]
	[InlineData(true, FailureBothWays, Success, FailureBothWays, FailureBothWays)]
	[InlineData(true, FailureBothWays, Failure, Failure, Success)]
	[InlineData(true, FailureBothWays, Undecided, Failure, Undecided)]
	[InlineData(true, FailureBothWays, FailureBothWays, FailureBothWays, FailureBothWays)]
	[InlineData(false, FailureBothWays, Failure, FailureBothWays, FailureBothWays)]
	[InlineData(false, FailureBothWays, Success, Success, Failure)]
	[InlineData(false, FailureBothWays, Undecided, Undecided, Failure)]
	[InlineData(false, FailureBothWays, FailureBothWays, FailureBothWays, FailureBothWays)]
	public async Task Outcome_WithFailureBothWaysPart_ShouldStayFailureBothWaysOnlyWhenTheNegationFailsAsWell(
		bool isAnd, Outcome left, Outcome right, Outcome expected, Outcome expectedWhenNegated)
	{
		ConstraintResult sut = new JunctionResult(new DummyConstraintResult(left), new DummyConstraintResult(right),
			isAnd, isAnd ? " and " : " or ", FurtherProcessingStrategy.Continue);

		Outcome outcome = sut.Outcome;
		sut.Negate();

		await That(outcome).IsEqualTo(expected);
		await That(sut.Outcome).IsEqualTo(expectedWhenNegated);
	}

	[Theory]
	[InlineData(Outcome.Success, Outcome.FailureBothWays)]
	[InlineData(Outcome.Failure, Outcome.Success)]
	public async Task Outcome_WhenMemberFailsBothWays_ShouldDependOnTheParentUnderNegation(Outcome parentOutcome,
		Outcome expectedWhenNegated)
	{
		ConstraintResult sut = new MappingResult(new DummyConstraintResult(parentOutcome),
			new DummyConstraintResult(Outcome.FailureBothWays), sb => sb.Append(" whose member "), "member");

		sut.Negate();

		await That(sut.Outcome).IsEqualTo(expectedWhenNegated)
			.Because("a failed parent meets the negation of the mapping, even when the member could not be answered");
	}
}
