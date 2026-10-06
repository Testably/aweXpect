using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif
using System.Linq.Expressions;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class SingleEnumerationTests
		{
			[Test]
			public async Task Collection_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				int[][] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EnumerableWithUntypedExpected_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IEnumerable[] subject =
				[
					ToEnumerable([1, 2, 3,]),
					ToEnumerable([1, 2, 3,]),
					ToEnumerable([1, 2, 3,]),
				];
				IEnumerable expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Enumerable_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IEnumerable[] subject =
				[
					ToEnumerable([1, 2, 3,]),
					ToEnumerable([1, 2, 3,]),
					ToEnumerable([1, 2, 3,]),
				];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Enumerable_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 3, 2,]);
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
			public async Task Expectations_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
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
				IEnumerable<int>[] subject = [[1, 1, 2, 3,], [1, 2, 2, 3,], [1, 2, 3, 3,],];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected).IgnoringDuplicates());

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutableExpectations_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				ImmutableArray<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
				IEnumerable<Action<IThat<int>>> expected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(1), a => a.IsEqualTo(2),
						a => a.IsEqualTo(3));

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutablePredicates_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				ImmutableArray<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task Immutable_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				ImmutableArray<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task InAnyOrderIgnoringDuplicates_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IEnumerable<int>[] subject = [[3, 3, 2, 1,], [1, 2, 2, 3,], [2, 3, 1, 1,],];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected).InAnyOrder().IgnoringDuplicates());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task InAnyOrder_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IEnumerable<int>[] subject = [[3, 2, 1,], [1, 2, 3,], [2, 3, 1,],];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected).InAnyOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Predicates_WhenEvaluatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Predicates_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 3, 2,]);
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
				IEnumerable<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 4,],];
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
					               [
					                 1,
					                 2,
					                 4
					               ]
					             ]

					             Collection:
					             [
					               [
					                 1,
					                 2,
					                 3
					               ],
					               [
					                 1,
					                 2,
					                 3
					               ],
					               [
					                 1,
					                 2,
					                 4
					               ]
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
				IEnumerable<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEvaluatedRepeatedly_ShouldEnumerateExpectedOnce()
			{
				int attempts = 0;
				IEnumerable<int> subject = ChangingItems();
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(expected)).Within(30.Seconds());

				await That(Act).DoesNotThrow();
				await That(attempts).IsEqualTo(3);

				IEnumerable<int> ChangingItems()
				{
					bool isLate = ++attempts > 2;
					yield return 1;
					yield return 2;
					yield return isLate ? 3 : 4;
				}
			}

			[Test]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 3, 2,]);
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
				IEnumerable<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
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
				IEnumerable<int> subject = [1, 2, 3,];
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
				IEnumerable<IEnumerable<int>> subject = Items();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsEqualTo(expected));

				await That(Act).DoesNotThrow()
					.Because("a collection is not copied, so that it is read when an item is compared");

				IEnumerable<IEnumerable<int>> Items()
				{
					yield return [1, 2,];
					expected[1] = 3;
					yield return [1, 3,];
				}
			}

			[Test]
			public async Task WhenExpectedThrowsWhileItIsEnumerated_ShouldThrowTheException()
			{
				int enumerations = 0;
				MyException exception = new("the expected items are not available");
				IEnumerable<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
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

			[Test]
			public async Task WhenNegatedForSeveralItems_ShouldEnumerateExpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(expected)));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
