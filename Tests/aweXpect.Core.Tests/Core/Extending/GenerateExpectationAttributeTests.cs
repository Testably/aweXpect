using System.Threading;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class GenerateExpectationAttributeTests
{
	[Test]
	public async Task AsyncPredicate_ShouldReceiveTheCancellationTokenOfTheEvaluation()
	{
		Account subject = new("Alice", 1);
		using CancellationTokenSource cts = new();

		await That(subject).IsVerified().WithCancellation(cts.Token);

		await That(subject.ReceivedToken.CanBeCanceled).IsTrue()
			.Because("the token of the evaluation is passed on to an asynchronous predicate");
	}

	[Test]
	public async Task AsyncPredicate_WhenNotMet_ShouldFail()
	{
		Account subject = new("Alice", 0);

		async Task Act()
			=> await That(subject).IsVerified();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is verified,
			             but it was Alice
			             """);
	}

	[Test]
	public async Task AsyncValueTaskPredicate_WhenMet_ShouldSucceed()
	{
		Account subject = new("Alice", 6);

		async Task Act()
			=> await That(subject).IsPremium();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task AsyncValueTaskPredicate_WhenNegatedAndMet_ShouldFail()
	{
		Account subject = new("Alice", 6);

		async Task Act()
			=> await That(subject).IsNotPremium();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not premium,
			             but it was Alice
			             """);
	}

	[Test]
	public async Task DoesNotComplyWith_ShouldUseTheNegatedText()
	{
		Account subject = new("owner", 1);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.IsTheOwner());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not the owner,
			             but it was owner
			             """)
			.Because("an expectation without a negated name can still be negated");
	}

	[Test]
	public async Task NegatedExpectationText_ShouldBeUsedForTheNegatedExpectation()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).DoesNotHaveOwner("Alice");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not have owner "Alice",
			             but it was Alice
			             """);
	}

	[Test]
	public async Task NegatedExpectationText_WhenNegatedWithDoesNotComplyWith_ShouldBeUsed()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).DoesNotComplyWith(it => it.HasOwner("Alice"));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not have owner "Alice",
			             but it was Alice
			             """);
	}

	[Test]
	public async Task NotNullWhenFalse_WhenNegatedAndSubjectIsNull_ShouldFail()
	{
		Account? subject = null;

		async Task Act()
			=> await That(subject).IsNotNullOrUnnamed();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not null or unnamed,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task NotNullWhenFalse_WhenSubjectIsNull_ShouldLetThePredicateDecide()
	{
		Account? subject = null;

		async Task Act()
			=> await That(subject).IsNullOrUnnamed();

		await That(Act).DoesNotThrow()
			.Because("a nullable subject parameter lets the predicate decide about null");
	}

	[Test]
	public async Task NullableOverload_WhenNegatedAndSubjectIsNull_ShouldFail()
	{
		int? subject = null;

		async Task Act()
			=> await That(subject).IsNotLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not lucky,
			             but it was <null>
			             """)
			.Because("a null subject fails the expectation and its negation alike");
	}

	[Test]
	public async Task NullableOverload_WhenValueIsMet_ShouldSucceed()
	{
		int? subject = 7;

		int result = await That(subject).IsLucky();

		await That(result).IsEqualTo(7);
	}

	[Test]
	public async Task NullSubject_ShouldFail()
	{
		Account? subject = null;

		async Task Act()
			=> await That(subject).IsNamed("Alice");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is named "Alice",
			             but it was <null>
			             """);
	}

	[Test]
	public async Task NullSubject_WhenNegated_ShouldFail()
	{
		Account? subject = null;

		async Task Act()
			=> await That(subject).IsNotNamed("Alice");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not named "Alice",
			             but it was <null>
			             """)
			.Because("a null subject fails the expectation and its negation alike");
	}

	[Test]
	public async Task Parameters_ShouldBeFormattedInTheExpectation()
	{
		int subject = 4;

		async Task Act()
			=> await That(subject).IsNotBetween(1, 5);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not between 1 and 5,
			             but it was 4
			             """);
	}

	[Test]
	public async Task ParamsParameter_ShouldFormatTheValues()
	{
		int subject = 4;

		async Task Act()
			=> await That(subject).IsAnyOf(1, 2, 3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is any of [1, 2, 3],
			             but it was 4
			             """);
	}

	[Test]
	public async Task PredicateResult_WhenAsyncAndFailureBothWays_ShouldFailWithTheReason()
	{
		Account subject = new("Alice", -1);

		async Task Act()
			=> await That(subject).IsNotTrusted();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not trusted,
			             but it was locked
			             """);
	}

	[Test]
	public async Task PredicateResult_WhenFailureBothWays_ShouldFailWithTheReason()
	{
		Account subject = new("Alice", -1);

		async Task Act()
			=> await That(subject).IsSenior();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is senior,
			             but it had no level
			             """);
	}

	[Test]
	public async Task PredicateResult_WhenFailureBothWaysAndNegated_ShouldAlsoFail()
	{
		Account subject = new("Alice", -1);

		async Task Act()
			=> await That(subject).IsNotSenior();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not senior,
			             but it had no level
			             """)
			.Because("a question that cannot be answered fails the expectation and its negation alike");
	}

	[Test]
	public async Task PredicateResult_WhenMet_ShouldSucceed()
	{
		Account subject = new("Alice", 5);

		async Task Act()
			=> await That(subject).IsSenior();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task PredicateResult_WhenNotMet_ShouldDescribeTheSubject()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsSenior();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is senior,
			             but it was Alice
			             """);
	}

	[Test]
	public async Task PredicateResult_WhenSubjectIsNull_ShouldFail()
	{
		int? subject = null;

		async Task Act()
			=> await That(subject).IsNotASquare();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not a square,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task PredicateResult_WhenUsedOnItems_ShouldFailWithTheReasonOfTheItem()
	{
		Account[] subject = [new("Alice", 5), new("Bob", -1),];

		async Task Act()
			=> await That(subject).All().ComplyWith(x => x.IsSenior());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is senior for all items,
			             but for the item at index 1, it had no level

			             Collection:
			             [
			               Alice,
			               Bob
			             ]
			             """);
	}

	[Test]
	public async Task Result_ShouldDescribeTheSubject()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsAtLevel(2);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is at level 2,
			             but it was at level 1
			             """);
	}

	[Test]
	public async Task Result_WhenItTakesItAndTheGrammars_ShouldDescribeTheWholeSubject()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsAtLeastLevel(2);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is at least level 2,
			             but it was at level 1 instead of 2
			             """);
	}

	[Test]
	public async Task Result_WhenNegated_ShouldDescribeTheSubject()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsNotAtLevel(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not at level 1,
			             but it was at level 1
			             """);
	}

	[Test]
	public async Task Result_WhenSubjectIsNull_ShouldNotCallTheResultMethod()
	{
		Account? subject = null;

		async Task Act()
			=> await That(subject).IsAtLevel(1);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is at level 1,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task SyncPredicate_ShouldReceiveTheCancellationTokenOfTheEvaluation()
	{
		Account subject = new("Alice", 1);
		using CancellationTokenSource cts = new();

		await That(subject).IsActive().WithCancellation(cts.Token);

		await That(subject.ReceivedToken.CanBeCanceled).IsTrue()
			.Because("the token of the evaluation is passed on to a synchronous predicate as well");
	}

	[Test]
	public async Task SyncPredicate_WhenNegatedAndMet_ShouldFail()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsNotActive();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not active,
			             but it was Alice
			             """);
	}

	[Test]
	public async Task WhenArgumentIsNull_ShouldThrowArgumentNullException()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsNamed(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("name").And
			.WithMessage("The 'name' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WhenDisallowNullSubjectIsNull_ShouldFail()
	{
		int? subject = null;

		async Task Act()
			=> await That(subject).IsNotOdd();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not odd,
			             but it was <null>
			             """)
			.Because("[DisallowNull] restores the failure for a null subject");
	}

	[Test]
	public async Task WhenMet_ShouldReturnTheSubject()
	{
		Account subject = new("Alice", 1);

		Account result = await That(subject).IsNamed("Alice");

		await That(result).IsSameAs(subject);
	}

	[Test]
	public async Task WhenNegatedAndMet_ShouldFail()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsNotNamed("Alice");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not named "Alice",
			             but it was Alice
			             """);
	}

	[Test]
	public async Task WhenNotMet_ShouldFail()
	{
		Account subject = new("Alice", 1);

		async Task Act()
			=> await That(subject).IsNamed("Bob");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is named "Bob",
			             but it was Alice
			             """);
	}

	[Test]
	public async Task WhenUsedOnItems_ShouldDescribeTheItems()
	{
		int[] subject = [7, 3,];

		async Task Act()
			=> await That(subject).All().ComplyWith(item => item.IsLucky());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [3]

			             Collection:
			             [7, 3]
			             """);
	}
}
