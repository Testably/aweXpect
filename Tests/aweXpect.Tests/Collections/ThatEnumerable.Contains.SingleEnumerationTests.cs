using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif
using aweXpect.Core;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class SingleEnumerationTests
		{
			[Test]
			public async Task Enumerable_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(2, 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [2, 4]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task EnumerableWithUntypedExpected_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				IEnumerable expected = Factory.GetSingleUseEnumerable(2, 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               2,
					               4
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task Expectations_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				int[] subject = [1, 2, 3,];
				IEnumerable<Action<IThat<int>>> expected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(2), a => a.IsEqualTo(4));

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: an item that is equal to 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [an item that is equal to 2, an item that is equal to 4]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task Immutable_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(2, 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [2, 4]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutableExpectations_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<Action<IThat<int>>> expected =
					Factory.GetSingleUseEnumerable<Action<IThat<int>>>(a => a.IsEqualTo(2), a => a.IsEqualTo(4));

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: an item that is equal to 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [an item that is equal to 2, an item that is equal to 4]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutablePredicates_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 2, a => a == 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: a => (a == 4)

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               a => (a == 2),
					               a => (a == 4)
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutableStrings_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("b", "d");

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "d"
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutableWithin_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<double> subject = [1.0, 2.0, 3.0,];
				IEnumerable<double> expected = Factory.GetSingleUseEnumerable(2.1, 4.0);

				async Task Act()
					=> await That(subject).Contains(expected).Within(0.25);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected ± 0.25 in order and contiguous,
					             but it lacked 1 of 2 expected items: 4.0

					             Collection:
					             [1.0, 2.0, 3.0]

					             Expected:
					             [2.1, 4.0]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}
#endif

			[Test]
			public async Task Predicates_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				int[] subject = [1, 2, 3,];
				IEnumerable<Expression<Func<int, bool>>> expected =
					Factory.GetSingleUseEnumerable<Expression<Func<int, bool>>>(a => a == 2, a => a == 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: a => (a == 4)

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               a => (a == 2),
					               a => (a == 4)
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task StringArray_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				string[] subject = ["a", "b", "c",];
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("b", "d");

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "d"
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task Strings_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("b", "d");

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "d"
					             ]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task WhenExpectationIsBuilt_ShouldOnlyReadTheFirstExpectedItem()
			{
				int readItems = 0;
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				IEnumerable<int> GetExpected()
				{
					readItems++;
					yield return 2;
					readItems++;
					yield return 3;
				}

				ObjectProperCollectionMatchWithToleranceResult<IEnumerable<int>, IThat<IEnumerable<int>?>, int, int>
					expectation =
					That(subject).Contains(GetExpected());
				int readItemsWhenBuilt = readItems;
				await expectation;

				await That(readItemsWhenBuilt).IsEqualTo(1)
					.Because("the check for an empty sequence reads only the first item and the rest is read on evaluation");
			}

			[Test]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(2, 4);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [2, 4]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}

			[Test]
			public async Task WhenExpectedHasSideEffects_ShouldEnumerateItOnlyOnce()
			{
				int enumerations = 0;
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				IEnumerable<int> GetExpected()
				{
					enumerations++;
					yield return 2;
					yield return 3;
				}

				await That(subject).Contains(GetExpected());

				await That(enumerations).IsEqualTo(1)
					.Because("the guard must not consume the expected items that the comparison needs");
			}

			[Test]
			public async Task WhenExpectedThrows_ShouldThrowTheExceptionOfTheExpectedItems()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				IEnumerable<int> GetExpected()
				{
					yield return 2;
					throw new InvalidOperationException("the expected items are broken");
				}

				async Task Act()
					=> await That(subject).Contains(GetExpected());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("the expected items are broken")
					.Because("an exception of the expected items is not reported as if the subject threw it");
			}

			[Test]
			public async Task WhenUnexpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> unexpected = Factory.GetSingleUseEnumerable(2, 3);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection unexpected in order and contiguous,
					             but it did

					             Collection:
					             [1, 2, 3, (… and maybe more)]

					             Expected:
					             [2, 3]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the unexpected items");
			}

			[Test]
			public async Task Within_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<double> subject = ToEnumerable(1.0, 2.0, 3.0);
				IEnumerable<double> expected = Factory.GetSingleUseEnumerable(2.1, 4.0);

				async Task Act()
					=> await That(subject).Contains(expected).Within(0.25);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected ± 0.25 in order and contiguous,
					             but it lacked 1 of 2 expected items: 4.0

					             Collection:
					             [1.0, 2.0, 3.0]

					             Expected:
					             [2.1, 4.0]
					             """)
					.Because("the guard, the comparison and the message share one enumeration of the expected items");
			}
		}
	}
}
