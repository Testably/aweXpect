#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq.Expressions;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class SingleEnumerationTests
		{
			[Test]
			public async Task Expectations_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
				];
				IEnumerable<Action<IThat<int>>> expected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(1), a => a.IsEqualTo(2),
						a => a.IsEqualTo(3));

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringDuplicates_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(1, 1, 2, 3),
					ToAsyncEnumerable(1, 2, 2, 3),
					ToAsyncEnumerable(1, 2, 3, 3),
				];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected).IgnoringDuplicates());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task InAnyOrderIgnoringDuplicates_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(3, 3, 2, 1),
					ToAsyncEnumerable(1, 2, 2, 3),
					ToAsyncEnumerable(2, 3, 1, 1),
				];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected).InAnyOrder().IgnoringDuplicates());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task InAnyOrder_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(3, 2, 1),
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(2, 3, 1),
				];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected).InAnyOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Predicates_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
				];
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Predicates_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 3, 2);
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 3 at index 1 instead of a => (a == 2) and
					               contained item 2 at index 2 instead of a => (a == 3)
					             (but the items match in a different order)

					             Collection:
					             [1, 3, 2]

					             Expected:
					             [
					               a => (a == 1),
					               a => (a == 2),
					               a => (a == 3)
					             ]
					             """)
					.Because("the comparison, the hint and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task WhenEvaluatedForSeveralItems_ShouldCompareEveryItemWithTheExpectedItems()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 4),
				];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order for all items,
					             but only 2 of 3 were

					             Not matching items:
					             [
					               IAsyncEnumerable<int>
					             ]

					             Collection:
					             [
					               IAsyncEnumerable<int>,
					               IAsyncEnumerable<int>,
					               IAsyncEnumerable<int>
					             ]

					             Collection (item [2]):
					             [1, 2, 4]

					             Expected (item [2]):
					             [
					               1,
					               2,
					               3
					             ]
					             """)
					.Because("every item is compared with the items of the one enumeration");
			}

			[Test]
			public async Task WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
				];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 3, 2);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 3 at index 1 instead of 2 and
					               contained item 2 at index 2 instead of 3
					             (but the items match in a different order)

					             Collection:
					             [1, 3, 2]

					             Expected:
					             [
					               1,
					               2,
					               3
					             ]
					             """)
					.Because("the comparison, the hint and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task WhenExpectedIsALazySequence_ShouldEnumerateItOnceForSeveralItems()
			{
				int enumerations = 0;
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
				];
				IEnumerable<int> expected = LazySequence();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
				await That(enumerations).IsEqualTo(1);

				IEnumerable<int> LazySequence()
				{
					enumerations++;
					yield return 1;
					yield return 2;
					yield return 3;
				}
			}

			[Test]
			public async Task WhenExpectedIsALazySequence_ShouldNotEnumerateItBeforeTheEvaluation()
			{
				int enumerations = 0;
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
				IEnumerable<int> expected = LazySequence();

				Expectation expectation = That(subject).IsEqualTo(expected);

				await That(enumerations).IsEqualTo(0);
				await ThatAll(expectation);
				await That(enumerations).IsEqualTo(1);

				IEnumerable<int> LazySequence()
				{
					enumerations++;
					yield return 1;
					yield return 2;
					yield return 3;
				}
			}

			[Test]
			public async Task WhenExpectedIsAListThatChangesBetweenTheItems_ShouldCompareWithItsCurrentItems()
			{
				List<int> expected = [1, 2,];
				IEnumerable<IAsyncEnumerable<int>> subject = Items();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow()
					.Because("a collection is not copied, so that it is read when an item is compared");

				IEnumerable<IAsyncEnumerable<int>> Items()
				{
					yield return ToAsyncEnumerable(1, 2);
					expected[1] = 3;
					yield return ToAsyncEnumerable(1, 3);
				}
			}

			[Test]
			public async Task WhenExpectedThrowsWhileItIsEnumerated_ShouldThrowTheException()
			{
				int enumerations = 0;
				MyException exception = new("the expected items are not available");
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
					ToAsyncEnumerable(1, 2, 3),
				];
				IEnumerable<int> expected = ThrowingSequence();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).Throws<MyException>().Which.IsSameAs(exception);
				await That(enumerations).IsEqualTo(1);

				IEnumerable<int> ThrowingSequence()
				{
					enumerations++;
					yield return 1;
					throw exception;
				}
			}
		}
	}
}
#endif
