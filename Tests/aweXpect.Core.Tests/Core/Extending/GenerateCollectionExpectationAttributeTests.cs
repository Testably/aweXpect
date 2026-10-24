using System.Collections.Generic;
using System.Threading;
using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class GenerateCollectionExpectationAttributeTests
{
#if NET8_0_OR_GREATER
	[Test]
	public async Task AsyncEnumerable_WhenCanceled_ShouldBeUndecided()
	{
		IAsyncEnumerable<int> subject = ToAsyncEnumerable(7, 7);
		using CancellationTokenSource cts = new();
		cts.Cancel();

		async Task Act()
			=> await That(subject).All().AreLucky().WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but it could not be verified, because the evaluation was already canceled
			             """);
	}

	[Test]
	public async Task AsyncEnumerable_WhenMet_ShouldSucceed()
	{
		IAsyncEnumerable<int> subject = ToAsyncEnumerable(7, 7);

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task AsyncEnumerable_WhenNotMet_ShouldFail()
	{
		IAsyncEnumerable<int> subject = ToAsyncEnumerable(7, 1, 7);

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but only 1 of at least 2 were

			             Not matching items:
			             [1, (… and maybe more)]

			             Collection:
			             [7, 1, (… and maybe more)]
			             """);
	}

	[Test]
	public async Task AsyncEnumerable_WhenNull_ShouldFail()
	{
		IAsyncEnumerable<int>? subject = null;

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but it was <null>
			             """);
	}
#endif

	[Test]
	public async Task AsyncPredicate_ShouldKeepTheSynchronizationContextOfTheCaller()
	{
		Account[] subject = [new("Alice", 1), new("Bob", 1), new("Carol", 1),];

		SynchronizationContext? callerContext = await SingleThreadedSynchronizationContext.Run(async () =>
		{
			await That(subject).All().AreVerified();
			return SynchronizationContext.Current;
		});

		await That(subject).All().Satisfy(x => x.VerifiedOn is not null && x.VerifiedOn == callerContext)
			.Because("the predicate of every item runs on the synchronization context of the caller, like the UI thread of WPF or WinForms");
	}

	[Test]
	public async Task AsyncPredicate_ShouldReceiveTheCancellationTokenOfTheEvaluation()
	{
		Account[] subject = [new("Alice", 1), new("Bob", 1),];
		using CancellationTokenSource cts = new();

		await That(subject).All().AreVerified().WithCancellation(cts.Token);

		await That(subject).All().Satisfy(x => x.ReceivedToken.CanBeCanceled)
			.Because("the token of the evaluation is passed on to the predicate for every item");
	}

	[Test]
	public async Task AsyncPredicate_WhenNotMet_ShouldFail()
	{
		Account[] subject = [new("Alice", 6), new("Bob", 1),];

		async Task Act()
			=> await That(subject).All().ArePremium();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is premium for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [
			               Bob
			             ]

			             Collection:
			             [
			               Alice,
			               Bob
			             ]
			             """);
	}

	[Test]
	public async Task AtLeast_WhenNotMet_ShouldFail()
	{
		int[] subject = [7, 1, 2,];

		async Task Act()
			=> await That(subject).AtLeast(2).AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for at least 2 items,
			             but only 1 of 3 were

			             Not matching items:
			             [1, 2]

			             Collection:
			             [7, 1, 2]
			             """);
	}

	[Test]
	public async Task Attribute_ShouldKeepTheTextsAndTheOptions()
	{
		GenerateCollectionExpectationAttribute subject = new("AreLucky", "is lucky", "are lucky")
		{
			NullableOverload = false,
			OnCollection = true,
			NegatedName = "AreUnlucky",
			FormattedItem = "ToBadge",
			Summary = "summary",
			Remarks = "remarks",
		};

		await That(subject.Name).IsEqualTo("AreLucky");
		await That(subject.ExpectationText).IsEqualTo("is lucky");
		await That(subject.PluralExpectationText).IsEqualTo("are lucky");
		await That(subject.NullableOverload).IsFalse();
		await That(subject.OnCollection).IsTrue();
		await That(subject.NegatedName).IsEqualTo("AreUnlucky");
		await That(subject.FormattedItem).IsEqualTo("ToBadge");
		await That(subject.Summary).IsEqualTo("summary");
		await That(subject.Remarks).IsEqualTo("remarks");
	}

	[Test]
	public async Task Attribute_WithoutExpectationText_ShouldLeaveItToTheExpectation()
	{
		GenerateCollectionExpectationAttribute subject = new("AreLucky", "are lucky");

		await That(subject.Name).IsEqualTo("AreLucky");
		await That(subject.ExpectationText).IsNull()
			.Because("the generator takes the text of [GenerateExpectation] on the same method");
		await That(subject.PluralExpectationText).IsEqualTo("are lucky");
		await That(subject.NullableOverload).IsTrue();
		await That(subject.OnCollection).IsFalse();
		await That(subject.NegatedName).IsNull();
		await That(subject.FormattedItem).IsNull();
	}

	[Test]
	public async Task Canceled_ShouldBeUndecided()
	{
		int[] subject = [7, 7,];
		using CancellationTokenSource cts = new();
		cts.Cancel();

		async Task Act()
			=> await That(subject).All().AreLucky().WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but it could not be verified, because the evaluation was already canceled

			             Collection:
			             [7, 7]
			             """);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task DefaultImmutableArray_ShouldFail()
	{
		ImmutableArray<int> subject = default;

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but it was <null>
			             """);
	}
#endif

	[Test]
	public async Task DoesNotComplyWith_ShouldNegateTheQuantifier()
	{
		int[] subject = [7, 7,];

		async Task Act()
			=> await That(subject).DoesNotComplyWith(x => x.All().AreLucky());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky not for all items,
			             but all 2 were

			             Collection:
			             [7, 7]
			             """);
	}

	[Test]
	public async Task EndlessCollection_ShouldStopOnceTheOutcomeIsDetermined()
	{
		IEnumerable<int> subject = Endless(7);

		async Task Act()
			=> await That(subject).AtLeast(2).AreLucky();

		await That(Act).DoesNotThrow()
			.Because("an enumerable without a known count is only read until the outcome is determined");
	}

	[Test]
	public async Task FormattedItem_ShouldStandForTheItemsInTheFailureMessage()
	{
		Account[] subject = [new("Alice", 9), new("Bob", 1), new("Carol", 2),];

		async Task Act()
			=> await That(subject).AreAdmins();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are admins,
			             but only 1 of 3 were

			             Not matching items:
			             [
			               Bob (level 1),
			               Carol (level 2)
			             ]

			             Collection:
			             [
			               Alice (level 9),
			               Bob (level 1),
			               Carol (level 2)
			             ]
			             """);
	}

	[Test]
	public async Task FormattedItem_WithQuantifier_ShouldStandForTheItemsInTheFailureMessage()
	{
		Account[] subject = [new("Alice", 9), new("Bob", 1),];

		async Task Act()
			=> await That(subject).None().AreAdmins();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is an admin for no items,
			             but 1 of 2 were

			             Matching items:
			             [
			               Alice (level 9)
			             ]

			             Collection:
			             [
			               Alice (level 9),
			               Bob (level 1)
			             ]
			             """);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task ImmutableArray_WhenNotMet_ShouldFail()
	{
		ImmutableArray<int> subject = [7, 1,];

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [1]

			             Collection:
			             [7, 1]
			             """);
	}
#endif

	[Test]
	public async Task ItemTypeParameter_ShouldKeepTheTypeOfTheItems()
	{
		Moderator[] subject = [new("Ann"), new("Bob"),];

		AndOrResult<IEnumerable<Moderator?>, IThat<IEnumerable<Moderator?>?>> result = That(subject).HaveShortNames();

		async Task Act()
			=> await result;

		await That(Act).DoesNotThrow()
			.Because("the expectation is generic over the type of the items, so that its result keeps that type");
	}

	[Test]
	public async Task ItemTypeParameter_WhenNotMet_ShouldFail()
	{
		Moderator[] subject = [new("Bob"), new("Alice"),];

		async Task Act()
			=> await That(subject).HaveShortNames();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all have short names,
			             but only 1 of 2 did

			             Not matching items:
			             [
			               Alice
			             ]

			             Collection:
			             [
			               Bob,
			               Alice
			             ]
			             """);
	}

	[Test]
	public async Task NegatedName_WhenAnItemMatches_ShouldFail()
	{
		int[] subject = [1, 2, 4,];

		async Task Act()
			=> await That(subject).AreNotEven();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             none are even,
			             but 2 of 3 were

			             Matching items:
			             [2, 4]

			             Collection:
			             [1, 2, 4]
			             """);
	}

	[Test]
	public async Task NegatedName_WhenNegated_ShouldFailWhenNoItemMatches()
	{
		int[] subject = [1, 3,];

		async Task Act()
			=> await That(subject).DoesNotComplyWith(they => they.AreNotEven());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             at least one is even,
			             but none of 2 were

			             Collection:
			             [1, 3]
			             """);
	}

	[Test]
	public async Task NegatedName_WhenNoItemMatches_ShouldSucceed()
	{
		int[] subject = [1, 3,];

		async Task Act()
			=> await That(subject).AreNotEven();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task Nested_ShouldUseThePluralText()
	{
		Dictionary<string, int> subject = new()
		{
			["a"] = 7,
			["b"] = 1,
			["c"] = 2,
		};

		async Task Act()
			=> await That(subject).Values.AtLeast(2).AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has values of which at least 2 are lucky,
			             but only 1 of 3 were

			             Not matching items (values):
			             [1, 2]

			             Collection (values):
			             [7, 1, 2]
			             """);
	}

	[Test]
	public async Task NullableItemParameter_ShouldLetThePredicateDecide()
	{
		Account?[] subject = [null, new("", 1),];

		async Task Act()
			=> await That(subject).All().AreNullOrUnnamed();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task NullableOverload_WhenItemIsNull_ShouldNotMatch()
	{
		int?[] subject = [7, null,];

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [<null>]

			             Collection:
			             [7, <null>]
			             """);
	}

	[Test]
	public async Task NullableValueTypeWithDisallowNull_WhenItemIsNull_ShouldNotMatch()
	{
		int?[] subject = [1, null,];

		async Task Act()
			=> await That(subject).All().AreOdd();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is odd for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [<null>]

			             Collection:
			             [1, <null>]
			             """);
	}

	[Test]
	public async Task NullCollection_ShouldFail()
	{
		IEnumerable<int>? subject = null;

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is lucky for all items,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task NullItem_ShouldNotMatch()
	{
		Account?[] subject = [new("Alice", 1), null,];

		async Task Act()
			=> await That(subject).All().HaveOwner("Alice");

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has owner "Alice" for all items,
			             but only 1 of 2 did

			             Not matching items:
			             [
			               <null>
			             ]

			             Collection:
			             [
			               Alice,
			               <null>
			             ]
			             """);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task OnCollection_AsyncEnumerable_ShouldReadAllItems()
	{
		IAsyncEnumerable<int> subject = ToAsyncEnumerable(2, 1, 3);

		async Task Act()
			=> await That(subject).AreEven();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are even,
			             but only 1 of 3 were

			             Not matching items:
			             [1, 3]

			             Collection:
			             [2, 1, 3]
			             """);
	}
#endif

	[Test]
	public async Task OnCollection_LazyEnumerable_ShouldReadAllItems()
	{
		IEnumerable<int> subject = ToEnumerable(2, 1, 3);

		async Task Act()
			=> await That(subject).AreEven();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are even,
			             but only 1 of 3 were

			             Not matching items:
			             [1, 3]

			             Collection:
			             [2, 1, 3]
			             """);
	}

	[Test]
	public async Task OnCollection_NullableOverload_WhenItemIsNull_ShouldNotMatch()
	{
		int?[] subject = [2, null,];

		async Task Act()
			=> await That(subject).AreEven();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are even,
			             but only 1 of 2 were

			             Not matching items:
			             [<null>]

			             Collection:
			             [2, <null>]
			             """);
	}

	[Test]
	public async Task OnCollection_WhenAllItemsMatch_ShouldSucceed()
	{
		int[] subject = [2, 4,];

		async Task Act()
			=> await That(subject).AreEven();

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task OnCollection_WhenNegated_ShouldFailWhenAllItemsMatch()
	{
		int[] subject = [2, 4,];

		async Task Act()
			=> await That(subject).DoesNotComplyWith(they => they.AreEven());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             not all are even,
			             but all 2 were

			             Collection:
			             [2, 4]
			             """);
	}

	[Test]
	public async Task OnCollection_WhenNull_ShouldFail()
	{
		IEnumerable<int>? subject = null;

		async Task Act()
			=> await That(subject).AreEven();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are even,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task ParameterIsNull_ShouldThrowArgumentNullException()
	{
		Account[] subject = [new("Alice", 1),];

		async Task Act()
			=> await That(subject).All().HaveOwner(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("owner").And
			.WithMessage("The 'owner' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Parameters_ShouldBeFormattedInTheExpectation()
	{
		int[] subject = [1, 4, 9,];

		async Task Act()
			=> await That(subject).All().AreBetween(1, 5);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is between 1 and 5 for all items,
			             but only 2 of 3 were

			             Not matching items:
			             [9]

			             Collection:
			             [1, 4, 9]
			             """);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task PredicateResult_WhenAnAsyncEnumerableItemFailsBothWays_ShouldFailWithTheReason()
	{
		IAsyncEnumerable<int> subject = ToAsyncEnumerable(4, -1, 9);

		async Task Act()
			=> await That(subject).All().AreSquares();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is a square for all items,
			             but for the item at index 1, it was negative

			             Collection:
			             [4, -1, (… and maybe more)]
			             """);
	}
#endif

	[Test]
	public async Task PredicateResult_WhenAnItemFailsBothWays_ShouldFailWithTheReason()
	{
		Account[] subject = [new("Alice", 5), new("Bob", -1), new("Carol", 5),];

		async Task Act()
			=> await That(subject).All().AreSenior();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is senior for all items,
			             but for the item at index 1, it had no level

			             Collection:
			             [
			               Alice,
			               Bob,
			               Carol
			             ]
			             """);
	}

	[Test]
	public async Task PredicateResult_WhenAnItemFailsBothWaysAndNegated_ShouldAlsoFail()
	{
		Account[] subject = [new("Alice", 1), new("Bob", -1),];

		async Task Act()
			=> await That(subject).None().AreSenior();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is senior for no items,
			             but for the item at index 1, it had no level

			             Collection:
			             [
			               Alice,
			               Bob
			             ]
			             """)
			.Because("an item that cannot be answered fails the expectation and its negation alike");
	}

	[Test]
	public async Task PredicateResult_WhenAsyncAndAnItemFailsBothWays_ShouldFailWithTheReason()
	{
		Account[] subject = [new("Alice", 5), new("Bob", -1),];

		async Task Act()
			=> await That(subject).All().AreTrusted();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is trusted for all items,
			             but for the item at index 1, it was locked

			             Collection:
			             [
			               Alice,
			               Bob
			             ]
			             """);
	}

	[Test]
	public async Task PredicateResult_WhenOutcomeIsDeterminedBeforeTheItem_ShouldIgnoreIt()
	{
		int[] subject = [2, -1, 4,];

		async Task Act()
			=> await That(subject).All().AreSquares();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is a square for all items,
			             but none of at least 1 were

			             Not matching items:
			             [2, (… and maybe more)]

			             Collection:
			             [2, -1, 4]
			             """)
			.Because("an item that cannot be answered does not change an outcome that the items before determine");
	}

	[Test]
	public async Task PredicateThrowsAfterTheOutcomeIsDetermined_ShouldStopReadingTheItems()
	{
		int[] subject = [12, -1, 3,];

		async Task Act()
			=> await That(subject).All().AreSingleDigits();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is a single digit for all items,
			             but none of at least 1 were

			             Not matching items:
			             [12, (… and maybe more)]

			             Collection:
			             [12, -1, 3]
			             """)
			.Because("the items after the one that determines the outcome cannot change it");
	}

	[Test]
	public async Task PredicateThrowsBeforeTheOutcomeIsDetermined_ShouldFail()
	{
		int[] subject = [-1, 3,];

		async Task Act()
			=> await That(subject).All().AreSingleDigits();

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("value");
	}

	[Test]
	public async Task Strings_ShouldUseTheStringElements()
	{
		string?[] subject = ["A", "b", null,];

		async Task Act()
			=> await That(subject).None().AreShouted();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is shouted for no items,
			             but 1 of 3 were

			             Matching items:
			             [
			               "A"
			             ]

			             Collection:
			             [
			               "A",
			               "b",
			               <null>
			             ]
			             """);
	}

	[Test]
	public async Task SyncPredicate_ShouldReceiveTheCancellationTokenOfTheEvaluation()
	{
		Account[] subject = [new("Alice", 1),];
		using CancellationTokenSource cts = new();

		await That(subject).All().AreActive().WithCancellation(cts.Token);

		await That(subject[0].ReceivedToken.CanBeCanceled).IsTrue()
			.Because("the token of the evaluation is passed on to the predicate");
	}

	[Test]
	public async Task WhenMet_ShouldSucceed()
	{
		int[] subject = [7, 7,];

		async Task Act()
			=> await That(subject).All().AreLucky();

		await That(Act).DoesNotThrow();
	}

	private static IEnumerable<int> Endless(int value)
	{
		while (true)
		{
			yield return value;
		}
	}

	private static IEnumerable<int> ToEnumerable(params int[] values)
	{
		foreach (int value in values)
		{
			yield return value;
		}
	}

#if NET8_0_OR_GREATER
	private static async IAsyncEnumerable<int> ToAsyncEnumerable(params int[] values)
	{
		foreach (int value in values)
		{
			await Task.Yield();
			yield return value;
		}
	}
#endif
}
