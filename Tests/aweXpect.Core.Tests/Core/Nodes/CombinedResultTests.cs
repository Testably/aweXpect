using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
using static aweXpect.Core.Constraints.Outcome;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class CombinedResultTests
{
	[Test]
	public async Task AppendResult_WhenTheRightResultDoesNotStartWithItsLeadingSubject_ShouldKeepTheWholeResult()
	{
		ConstraintResult sut = new JunctionResult(new SubjectResult(Failure, "it", "it was 1"),
			new SubjectResult(Failure, "it", "found 2"), true, " and ", FurtherProcessingStrategy.Continue);

		string result = sut.GetResultText();

		await That(result).IsEqualTo("it was 1 and found 2")
			.Because("only a repeated subject is omitted");
	}

	[Test]
	public async Task FailureCause_ShouldIgnoreTheCauseOfAPartThatIsMet()
	{
		ConstraintResult sut = new JunctionResult(new FailureCauseResult(Success, new MyException()),
			new DummyConstraintResult(Failure), true, " and ", FurtherProcessingStrategy.Continue);

		await That(sut.FailureCause).IsNull()
			.Because("only the failed right part explains why the combination failed");
	}

	[Test]
	[Arguments(Failure, Failure, "left", "right")]
	[Arguments(Failure, Success, "left", "left")]
	[Arguments(Success, Failure, "right", "right")]
	[Arguments(Success, Success, null, null)]
	public async Task LeadingAndTrailingSubject_ShouldBeTheOnesOfTheRenderedOperands(Outcome left, Outcome right,
		string? expectedLeadingSubject, string? expectedTrailingSubject)
	{
		ConstraintResult sut = new JunctionResult(new SubjectResult(left, "left", "left result"),
			new SubjectResult(right, "right", "right result"), true, " and ", FurtherProcessingStrategy.Continue);

		await That(sut.LeadingSubject).IsEqualTo(expectedLeadingSubject);
		await That(sut.TrailingSubject).IsEqualTo(expectedTrailingSubject);
	}

	[Test]
	[Arguments(Success, Success, Success)]
	[Arguments(Success, Failure, Failure)]
	[Arguments(Success, Undecided, Undecided)]
	[Arguments(Success, FailureBothWays, FailureBothWays)]
	[Arguments(Failure, Success, Failure)]
	[Arguments(Failure, Failure, Failure)]
	[Arguments(Failure, Undecided, Failure)]
	[Arguments(Failure, FailureBothWays, Failure)]
	[Arguments(Undecided, Success, Undecided)]
	[Arguments(Undecided, Failure, Failure)]
	[Arguments(Undecided, Undecided, Undecided)]
	[Arguments(Undecided, FailureBothWays, Failure)]
	[Arguments(FailureBothWays, Success, FailureBothWays)]
	[Arguments(FailureBothWays, Failure, Failure)]
	[Arguments(FailureBothWays, Undecided, Failure)]
	[Arguments(FailureBothWays, FailureBothWays, FailureBothWays)]
	public async Task Outcome_WithAnd_ShouldCombineTheOutcomesOfBothParts(Outcome left, Outcome right,
		Outcome expected)
	{
		ConstraintResult sut = new JunctionResult(new DummyConstraintResult(left), new DummyConstraintResult(right),
			true, " and ", FurtherProcessingStrategy.Continue);

		await That(sut.Outcome).IsEqualTo(expected);
	}

	[Test]
	[Arguments(Success, Success, Success)]
	[Arguments(Success, Failure, Success)]
	[Arguments(Success, Undecided, Success)]
	[Arguments(Success, FailureBothWays, Success)]
	[Arguments(Failure, Success, Success)]
	[Arguments(Failure, Failure, Failure)]
	[Arguments(Failure, Undecided, Undecided)]
	[Arguments(Failure, FailureBothWays, FailureBothWays)]
	[Arguments(Undecided, Success, Success)]
	[Arguments(Undecided, Failure, Undecided)]
	[Arguments(Undecided, Undecided, Undecided)]
	[Arguments(Undecided, FailureBothWays, Undecided)]
	[Arguments(FailureBothWays, Success, Success)]
	[Arguments(FailureBothWays, Failure, FailureBothWays)]
	[Arguments(FailureBothWays, Undecided, Undecided)]
	[Arguments(FailureBothWays, FailureBothWays, FailureBothWays)]
	public async Task Outcome_WithOr_ShouldCombineTheOutcomesOfBothParts(Outcome left, Outcome right,
		Outcome expected)
	{
		ConstraintResult sut = new JunctionResult(new DummyConstraintResult(left), new DummyConstraintResult(right),
			false, " or ", FurtherProcessingStrategy.Continue);

		await That(sut.Outcome).IsEqualTo(expected);
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
	public async Task Negate_WhenAMappingIsNegatedTwice_ShouldRestoreItsOutcomeAndExpectation()
	{
		ConstraintResult sut = new MappingResult(new DummyConstraintResult(Success, "parent"),
			new DummyConstraintResult(Failure, "member"), sb => sb.Append(" whose value "), "value");

		sut.Negate().Negate();

		await That(sut.Outcome).IsEqualTo(Failure);
		await That(sut.GetExpectationText()).IsEqualTo("parent whose value member");
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
	[Arguments(Success)]
	[Arguments(Failure)]
	public async Task Outcome_WhenTheLeftOperandWasSkipped_ShouldBeTheOutcomeOfTheRightOperand(Outcome right)
	{
		ConstraintResult sut = new JunctionResult(new DummyConstraintResult(Failure).AsExpectationOnly(),
			new DummyConstraintResult(right), false, " or ", FurtherProcessingStrategy.Continue);

		await That(sut.Outcome).IsEqualTo(right)
			.Because("a skipped operand was not evaluated");
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

	private sealed class SubjectResult(Outcome outcome, string subject, string result)
		: ConstraintResult(FurtherProcessingStrategy.Continue)
	{
		public override Outcome Outcome { get; protected set; } = outcome;

		public override string LeadingSubject => subject;

		public override string TrailingSubject => subject;

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(result);

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate() => this;
	}
}
