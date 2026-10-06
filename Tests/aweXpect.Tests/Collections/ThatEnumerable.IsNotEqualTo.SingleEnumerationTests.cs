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
	public sealed partial class IsNotEqualTo
	{
		public sealed class SingleEnumerationTests
		{
			[Test]
			public async Task Collection_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				int[][] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EnumerableWithUntypedUnexpected_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IEnumerable[] subject =
				[
					ToEnumerable([1, 2, 4,]),
					ToEnumerable([1, 2,]),
					ToEnumerable([3, 2, 1,]),
				];
				IEnumerable unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Enumerable_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IEnumerable[] subject =
				[
					ToEnumerable([1, 2, 4,]),
					ToEnumerable([1, 2,]),
					ToEnumerable([3, 2, 1,]),
				];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Expectations_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<Action<IThat<int>>> unexpected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(1), a => a.IsEqualTo(2),
						a => a.IsEqualTo(3));

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutableExpectations_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				ImmutableArray<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<Action<IThat<int>>> unexpected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(1), a => a.IsEqualTo(2),
						a => a.IsEqualTo(3));

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutablePredicates_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				ImmutableArray<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<Expression<Func<int, bool>>> unexpected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task Immutable_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				ImmutableArray<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			public async Task InAnyOrder_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [1, 2, 3, 4,],];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected).InAnyOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Predicates_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<Expression<Func<int, bool>>> unexpected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEvaluatedForSeveralItems_ShouldCompareEveryItemWithTheUnexpectedItems()
			{
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [1, 2, 3,],];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order for all items,
					             but only 2 of 3 were

					             Not matching items:
					             [
					               [
					                 1,
					                 2,
					                 3
					               ]
					             ]

					             Collection:
					             [
					               [
					                 1,
					                 2,
					                 4
					               ],
					               [
					                 1,
					                 2
					               ],
					               [
					                 1,
					                 2,
					                 3
					               ]
					             ]

					             Collection (item [2]):
					             [1, 2, 3]

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
			public async Task WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEvaluatedRepeatedly_ShouldEnumerateUnexpectedOnce()
			{
				int attempts = 0;
				IEnumerable<int> subject = ChangingItems();
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsNotEqualTo(unexpected)).Within(30.Seconds());

				await That(Act).DoesNotThrow();
				await That(attempts).IsEqualTo(3);

				IEnumerable<int> ChangingItems()
				{
					bool isLate = ++attempts > 2;
					yield return 1;
					yield return 2;
					yield return isLate ? 4 : 3;
				}
			}

			[Test]
			public async Task WhenNegatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IEnumerable<int>[] subject = [[1, 2, 3,], [1, 2, 3,], [1, 2, 3,],];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.DoesNotComplyWith(it => it.IsNotEqualTo(unexpected)));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsALazySequence_ShouldEnumerateItOnceForSeveralItems()
			{
				int enumerations = 0;
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<int> unexpected = LazySequence();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

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
			public async Task WhenUnexpectedIsALazySequence_ShouldNotEnumerateItBeforeTheEvaluation()
			{
				int enumerations = 0;
				IEnumerable<int> subject = [1, 2, 4,];
				IEnumerable<int> unexpected = LazySequence();

				Expectation expectation = That(subject).IsNotEqualTo(unexpected);

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
			public async Task WhenUnexpectedIsAListThatChangesBetweenTheItems_ShouldCompareWithItsCurrentItems()
			{
				List<int> unexpected = [1, 2,];
				IEnumerable<IEnumerable<int>> subject = Items();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow()
					.Because("a collection is not copied, so that it is read when an item is compared");

				IEnumerable<IEnumerable<int>> Items()
				{
					yield return [1, 3,];
					unexpected[1] = 3;
					yield return [1, 2,];
				}
			}

			[Test]
			public async Task WhenUnexpectedThrowsWhileItIsEnumerated_ShouldThrowTheException()
			{
				int enumerations = 0;
				MyException exception = new("the unexpected items are not available");
				IEnumerable<int>[] subject = [[1, 2, 4,], [1, 2,], [3, 2, 1,],];
				IEnumerable<int> unexpected = ThrowingSequence();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

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
