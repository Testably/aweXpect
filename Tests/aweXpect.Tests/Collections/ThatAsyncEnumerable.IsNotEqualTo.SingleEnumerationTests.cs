#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq.Expressions;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class SingleEnumerationTests
		{
			[Test]
			public async Task Expectations_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable([1, 2, 4,]),
					ToAsyncEnumerable([1, 2,]),
					ToAsyncEnumerable([3, 2, 1,]),
				];
				IEnumerable<Action<IThat<int>>> unexpected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(1), a => a.IsEqualTo(2),
						a => a.IsEqualTo(3));

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task InAnyOrder_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable([1, 2, 4,]),
					ToAsyncEnumerable([1, 2,]),
					ToAsyncEnumerable([1, 2, 3, 4,]),
				];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected).InAnyOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Predicates_WhenEvaluatedForSeveralItems_ShouldEnumerateUnexpectedOnce()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable([1, 2, 4,]),
					ToAsyncEnumerable([1, 2,]),
					ToAsyncEnumerable([3, 2, 1,]),
				];
				IEnumerable<Expression<Func<int, bool>>> unexpected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 1, a => a == 2, a => a == 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEvaluatedForSeveralItems_ShouldCompareEveryItemWithTheUnexpectedItems()
			{
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable([1, 2, 4,]),
					ToAsyncEnumerable([1, 2,]),
					ToAsyncEnumerable([1, 2, 3,]),
				];
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
					               IAsyncEnumerable<int>
					             ]

					             Collection:
					             [
					               IAsyncEnumerable<int>,
					               IAsyncEnumerable<int>,
					               IAsyncEnumerable<int>
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
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable([1, 2, 4,]),
					ToAsyncEnumerable([1, 2,]),
					ToAsyncEnumerable([3, 2, 1,]),
				];
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsALazySequence_ShouldEnumerateItOnceForSeveralItems()
			{
				int enumerations = 0;
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable([1, 2, 4,]),
					ToAsyncEnumerable([1, 2,]),
					ToAsyncEnumerable([3, 2, 1,]),
				];
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 4,]);
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
				IEnumerable<IAsyncEnumerable<int>> subject = Items();

				async Task Act()
					=> await That(subject).All().ComplyWith(x => x.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow()
					.Because("a collection is not copied, so that it is read when an item is compared");

				IEnumerable<IAsyncEnumerable<int>> Items()
				{
					yield return ToAsyncEnumerable([1, 3,]);
					unexpected[1] = 3;
					yield return ToAsyncEnumerable([1, 2,]);
				}
			}

			[Test]
			public async Task WhenUnexpectedThrowsWhileItIsEnumerated_ShouldThrowTheException()
			{
				int enumerations = 0;
				MyException exception = new("the unexpected items are not available");
				IAsyncEnumerable<int>[] subject =
				[
					ToAsyncEnumerable([1, 2, 4,]),
					ToAsyncEnumerable([1, 2,]),
					ToAsyncEnumerable([3, 2, 1,]),
				];
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
#endif
