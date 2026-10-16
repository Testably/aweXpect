using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class PredicateResultTests
{
	[Test]
	public async Task Default_ShouldNotBeMet()
	{
		PredicateResult subject = default;

		await That(subject.Outcome).IsEqualTo(Outcome.Failure)
			.Because("an uninitialized result must never let an expectation pass");
		await That(subject.Reason).IsNull();
	}

	[Test]
	public async Task FailureBothWays_ShouldKeepTheReason()
	{
		PredicateResult subject = PredicateResult.FailureBothWays("had no duration");

		await That(subject.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(subject.Reason).IsEqualTo("had no duration");
	}

	[Test]
	public async Task FailureBothWays_WhenReasonIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> PredicateResult.FailureBothWays(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("reason").And
			.WithMessage("The 'reason' cannot be null.").AsPrefix();
	}

	[Test]
	[Arguments(true, Outcome.Success)]
	[Arguments(false, Outcome.Failure)]
	public async Task ImplicitConversion_ShouldMapTheBoolean(bool isMet, Outcome expectedOutcome)
	{
		PredicateResult subject = isMet;

		await That(subject.Outcome).IsEqualTo(expectedOutcome);
		await That(subject.Reason).IsNull();
	}

	[Test]
	public async Task UnansweredItem_ShouldDescribeTheItemOnlyWithTheReason()
	{
		PredicateResult.UnansweredItem subject = new(-1, "was negative");
		StringBuilder expectation = new();
		StringBuilder result = new();

		subject.AppendExpectation(expectation);
		subject.AppendResult(result);

		await That(expectation.ToString()).IsEmpty()
			.Because("the collection constraint states the expectation on the items");
		await That(result.ToString()).IsEqualTo("it was negative");
	}

	[Test]
	public async Task UnansweredItem_ShouldStoreTheItem()
	{
		PredicateResult.UnansweredItem subject = new(-1, "was negative");

		bool isInt = subject.TryGetStoredValue(out int item);
		bool isString = subject.TryGetStoredValue(out string? text);

		await That(isInt).IsTrue();
		await That(item).IsEqualTo(-1);
		await That(isString).IsFalse();
		await That(text).IsNull();
	}

	[Test]
	public async Task UnansweredItem_WhenNegated_ShouldStillFailBothWays()
	{
		PredicateResult.UnansweredItem subject = new(-1, "was negative");

		ConstraintResult negated = subject.Negate();

		await That(negated).IsSameAs(subject);
		await That(negated.Outcome).IsEqualTo(Outcome.FailureBothWays);
	}
}
