using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class SingleEnumerationTests
		{
			[Test]
			public async Task Enumerable_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(4, 3);

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 1 instead of 4

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}

			[Test]
			public async Task Enumerable_WhenSubjectCanOnlyBeEnumeratedOnce_ShouldFailForDoesNotEndWith()
			{
				IEnumerable subject = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotEndWith(2, 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [2, 3],
					             but it did end with [2, 3]
					             """)
					.Because("the message lists the matching items from the enumeration that compared them");
			}

			[Test]
			public async Task EnumerableWithUntypedExpected_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				IEnumerable expected = Factory.GetSingleUseEnumerable(4, 3);

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 1 instead of 4

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task Immutable_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(4, 3);

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 1 instead of 4

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutableStrings_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("d", "c");

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item "b" at index 1 instead of "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ImmutableWithin_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				ImmutableArray<double> subject = [1.0, 2.0, 3.0,];
				IEnumerable<double> expected = Factory.GetSingleUseEnumerable(4.0, 2.9);

				async Task Act()
					=> await That(subject).EndsWith(expected).Within(0.25);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected ± 0.25,
					             but it contained item 2.0 at index 1 instead of 4.0

					             Collection:
					             [1.0, 2.0, 3.0]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}
#endif

			[Test]
			public async Task Strings_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("d", "c");

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item "b" at index 1 instead of "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}

			[Test]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(4, 3);

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 1 instead of 4

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}

			[Test]
			public async Task WhenSubjectCanOnlyBeEnumeratedOnce_ShouldFailForDoesNotEndWith()
			{
				IEnumerable<int> subject = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotEndWith(2, 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [2, 3],
					             but it did end with [2, 3]
					             """)
					.Because("the message lists the matching items from the enumeration that compared them");
			}

			[Test]
			public async Task WhenSubjectHasSideEffects_ShouldEnumerateItOnlyOnceForDoesNotEndWith()
			{
				int enumerations = 0;

				IEnumerable<int> GetSubject()
				{
					enumerations++;
					yield return 1;
					yield return 2;
					yield return 3;
				}

				async Task Act()
					=> await That(GetSubject()).DoesNotEndWith(2, 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that GetSubject()
					             does not end with [2, 3],
					             but it did end with [2, 3]
					             """);
				await That(enumerations).IsEqualTo(1)
					.Because("the message must not run the side effects of the subject a second time");
			}

			[Test]
			public async Task Within_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<double> subject = ToEnumerable(1.0, 2.0, 3.0);
				IEnumerable<double> expected = Factory.GetSingleUseEnumerable(4.0, 2.9);

				async Task Act()
					=> await That(subject).EndsWith(expected).Within(0.25);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected ± 0.25,
					             but it contained item 2.0 at index 1 instead of 4.0

					             Collection:
					             [1.0, 2.0, 3.0]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}
		}
	}
}
