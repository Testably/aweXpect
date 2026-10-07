using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
using static aweXpect.Core.Constraints.Outcome;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class CombinedResultTests
{
	[Test]
	public async Task FailureCause_ShouldIgnoreTheCauseOfAPartThatIsMet()
	{
		ConstraintResult sut = new JunctionResult(new FailureCauseResult(Success, new MyException()),
			new DummyConstraintResult(Failure), true, " and ", FurtherProcessingStrategy.Continue);

		await That(sut.FailureCause).IsNull()
			.Because("only the failed right part explains why the combination failed");
	}

	[Test]
	[Arguments(true, FailureBothWays, Success, FailureBothWays, FailureBothWays)]
	[Arguments(true, FailureBothWays, Failure, Failure, Success)]
	[Arguments(true, FailureBothWays, Undecided, Failure, Undecided)]
	[Arguments(true, FailureBothWays, FailureBothWays, FailureBothWays, FailureBothWays)]
	[Arguments(false, FailureBothWays, Failure, FailureBothWays, FailureBothWays)]
	[Arguments(false, FailureBothWays, Success, Success, Failure)]
	[Arguments(false, FailureBothWays, Undecided, Undecided, Failure)]
	[Arguments(false, FailureBothWays, FailureBothWays, FailureBothWays, FailureBothWays)]
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

	[Test]
	[Arguments(Success, FailureBothWays)]
	[Arguments(Failure, Success)]
	public async Task Outcome_WhenMemberFailsBothWays_ShouldDependOnTheParentUnderNegation(Outcome parentOutcome,
		Outcome expectedWhenNegated)
	{
		ConstraintResult sut = new MappingResult(new DummyConstraintResult(parentOutcome),
			new DummyConstraintResult(FailureBothWays), sb => sb.Append(" whose member "), "member");

		sut.Negate();

		await That(sut.Outcome).IsEqualTo(expectedWhenNegated)
			.Because("a failed parent meets the negation of the mapping, even when the member could not be answered");
	}

	[Test]
	public async Task TryGetStoredValue_WhenBothOperandsStoreAValue_ShouldReturnTheValueOfTheRightOperand()
	{
		object left = new();
		object right = new();
		ConstraintResult sut = new JunctionResult(new DummyConstraintResult(left, Success),
			new DummyConstraintResult(right, Success), true, " and ", FurtherProcessingStrategy.Continue);

		bool result = sut.TryGetStoredValue(out object? value);

		await That(result).IsTrue();
		await That(value).IsSameAs(right)
			.Because("the right-most expectation determines the type of the result");
	}

	[Test]
	public async Task TryGetStoredValue_WhenOnlyTheLeftOperandStoresAValue_ShouldReturnIt()
	{
		object left = new();
		ConstraintResult sut = new JunctionResult(new DummyConstraintResult(left, Success),
			new DummyConstraintResult(Success), true, " and ", FurtherProcessingStrategy.Continue);

		bool result = sut.TryGetStoredValue(out object? value);

		await That(result).IsTrue();
		await That(value).IsSameAs(left);
	}

	[Test]
	public async Task TryGetStoredValue_WhenTheRightOperandWasSkipped_ShouldReturnTheValueOfTheLeftOperand()
	{
		object left = new();
		object right = new();
		ConstraintResult sut = new JunctionResult(new DummyConstraintResult(left, Success),
			new DummyConstraintResult(right, Success).AsExpectationOnly(), false, " or ",
			FurtherProcessingStrategy.Continue);

		bool result = sut.TryGetStoredValue(out object? value);

		await That(result).IsTrue();
		await That(value).IsSameAs(left)
			.Because("a skipped operand was not evaluated");
	}

	private sealed class FailureCauseResult(Outcome outcome, Exception failureCause)
		: ConstraintResult(FurtherProcessingStrategy.Continue)
	{
		public override Outcome Outcome { get; protected set; } = outcome;

		public override Exception? FailureCause => failureCause;

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate() => this;
	}
}
