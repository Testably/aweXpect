using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class InSameOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked all 11 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked all 3 expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked all 10 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([1, 2,]);

				await That(Act).DoesNotThrow()
					.Because("all expected items already decide the result");
			}

			[Test]
			public async Task WhenExpectedContainsDuplicateButMissingItems_ShouldFail()
			{
				int[] collection = [1, 2, 1, 3, 12, 2, 2,];

				async Task Act()
					=> await That(collection).Contains([1, 2, 1, 1, 2,]);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that collection
					             contains collection [1, 2, 1, 1, 2,] in order and contiguous,
					             but it lacked 1 of 5 expected items: 1

					             Collection:
					             [1, 2, 1, 3, 12, 2, 2]

					             Expected:
					             [1, 2, 1, 1, 2]
					             """);
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = [];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;
				IEnumerable<string?> expected = ["foo",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 2 of 6 expected items:
					               "a",
					               "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it contained item "b" at index 2 in wrong order

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithContiguousSubset_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3, 4,]);
				int[] expected = [2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDeviationFollowedByMatchingItems_ShouldOnlyReportTheMissingItem()
			{
				IEnumerable<int> subject = ToEnumerable([1, 4, 3,]);
				int[] expected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 3 expected items: 2

					             Collection:
					             [1, 4, 3]

					             Expected:
					             [1, 2, 3]
					             """)
					.Because("the other expected items are found in order");
			}

			[Test]
			public async Task WithDuplicateBeforeAnInterruptedRun_ShouldReportTheInterruptingItem()
			{
				IEnumerable<int> subject = ToEnumerable([2, 1, 9, 2,]);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it contained item 9 at index 2 instead of 2

					             Collection:
					             [2, 1, 9, 2]

					             Expected:
					             [1, 2]
					             """)
					.Because("the last 2 follows the 1, so only the 9 keeps the items from being contiguous");
			}

			[Test]
			public async Task WithDuplicateWithinAnInterruptedRun_ShouldReportItInsteadOfTheNextExpectedItem()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 2, 3,]);
				int[] expected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it contained item 2 at index 2 instead of 3

					             Collection:
					             [1, 2, 2, 3]

					             Expected:
					             [1, 2, 3]
					             """)
					.Because("an interrupting item is not reported instead of an expected item that it is equal to");
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 4 expected items: "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 4 expected items: "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it contained item "c" at index 2 in wrong order

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "c",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItemAfterMoreThan20Items_ShouldReportTheMissingItem()
			{
				IEnumerable<int> subject = Enumerable.Range(0, 30);
				int[] expected = [99,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked the one expected item

					             Collection:
					             [
					               0,
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               (… and 20 more)
					             ]

					             Expected:
					             [99]
					             """)
					.Because("the items outside the expected run are no deviations of the containment");
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithNonContiguousSubset_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				int[] expected = [1, 3,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it contained item 2 at index 1 instead of 3

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [1, 3]
					             """)
					.Because("the expected items may not have other items in between");
			}

			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSubsetAfterAbandonedPartialMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 4, 2, 3,]);
				int[] expected = [2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSubsetAfterMoreThan20ItemsThatBreakAPartialMatch_ShouldSucceed()
			{
				IEnumerable<int> subject =
					ToEnumerable(new[]
					{
						1,
					}.Concat(Enumerable.Repeat(0, 21)).Concat(Enumerable.Range(1, 30)).ToArray());

				async Task Act()
					=> await That(subject).Contains(Enumerable.Range(1, 30));

				await That(Act).DoesNotThrow()
					.Because("the items after a broken partial match are no deviations, as later items can still contain the expected items");
			}

			[Test]
			public async Task WithSubsetMissingAfterAbandonedPartialMatch_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 4, 5,]);
				int[] expected = [2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: 3

					             Collection:
					             [1, 2, 4, 5]

					             Expected:
					             [2, 3]
					             """);
			}

			[Test]
			public async Task WithSubsetMissingInsideTheAbandonedPartialMatch_ShouldReportTheMissingItem()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 1, 3,]);
				int[] expected = [1, 1, 2,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 3 expected items: 2

					             Collection:
					             [1, 1, 1, 3]

					             Expected:
					             [1, 1, 2]
					             """)
					.Because("the subject contains the expected 1s, but no 2");
			}

			[Test]
			public async Task WithSubsetOverlappingTheAbandonedPartialMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 1, 2, 1, 3,]);
				int[] expected = [1, 2, 1, 3,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow()
					.Because("the expected items can start inside an abandoned partial match");
			}

			[Test]
			public async Task WithSubsetStartingAtTheDeviatingItem_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2,]);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow()
					.Because("containment searches for the expected items anywhere in the subject, so an item that breaks a partial match can start the next one");
			}

			[Test]
			public async Task WithSubsetStartingInsideTheAbandonedPartialMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 1, 2,]);
				int[] expected = [1, 1, 2,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow()
					.Because("the expected items can start inside an abandoned partial match");
			}
		}

		public sealed class InSameOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked all 11 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked all 3 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked all 2 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked all 10 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([1, 2,]).IgnoringDuplicates();

				await That(Act).DoesNotThrow()
					.Because("all expected items already decide the result");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = [];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!).IgnoringDuplicates();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               "a",
					               "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it contained item "b" at index 2 in wrong order

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicateBeforeADeviation_ShouldReportTheIndexInTheSubject()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 4, 2,]);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it contained item 4 at index 2 instead of 2

					             Collection:
					             [1, 1, 4, 2]

					             Expected:
					             [1, 2]
					             """)
					.Because("the index counts the position in the subject, not the distinct items");
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it contained item "c" at index 2 in wrong order

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "c",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItemAfterMoreThan20Items_ShouldReportTheMissingItem()
			{
				IEnumerable<int> subject = Enumerable.Range(0, 30);
				int[] expected = [99,];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked the one unique expected item

					             Collection:
					             [
					               0,
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               (… and 20 more)
					             ]

					             Expected:
					             [99]
					             """)
					.Because("the items outside the expected run are no deviations of the containment");
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSubsetAfterAbandonedPartialMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 4, 2, 3,]);
				int[] expected = [2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSubsetAfterMoreThan20Items_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(0, 30);
				int[] expected = [25, 26,];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow()
					.Because("the items outside the expected run are no deviations of the containment");
			}
		}

		public sealed class InAnyOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked all 11 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked all 3 expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked all 10 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([2, 1,]).InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("all expected items already decide the result");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = [];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!).InAnyOrder();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 2 of 6 expected items:
					               "a",
					               "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithManyAdditionalItems_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([..Enumerable.Range(100, 25), 1,]);
				int[] expected = [1,];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithManyAdditionalItemsAndMissingItem_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([..Enumerable.Range(100, 25), 1,]);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 2 expected items: 2

					             Collection:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 16 more)
					             ]

					             Expected:
					             [1, 2]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}


			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class InAnyOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked all 11 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked all 3 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked all 2 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked all 10 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([2, 1,]).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow()
					.Because("all expected items already decide the result");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = [];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               "a",
					               "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithManyAdditionalItems_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([..Enumerable.Range(100, 25), 1,]);
				int[] expected = [1,];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithManyAdditionalItemsAndMissingItem_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([..Enumerable.Range(100, 25), 1,]);
				int[] expected = [1, 2,];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 1 of 2 expected items: 2

					             Collection:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 16 more)
					             ]

					             Expected:
					             [1, 2]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               "d",
					               "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ProperlyInSameOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it lacked all 11 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items and
					               lacked all 3 expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it lacked all 10 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsAndAnAdditionalItemWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([1, 2,]).Properly();

				await That(Act).DoesNotThrow()
					.Because("all expected items and an additional item already decide the result");
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 6 expected items:
					                 "a",
					                 "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               contained item "b" at index 2 in wrong order and
					               did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 5 expected items:
					                 "d",
					                 "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}


			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithSubsetAfterAbandonedPartialMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([2, 4, 2, 3,]);
				int[] expected = [2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it lacked all 11 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 3 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 2 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it lacked all 10 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsAndAnAdditionalItemWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([1, 2,]).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow()
					.Because("all expected items and an additional item already decide the result");
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 5 expected items:
					                 "a",
					                 "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               contained item "b" at index 2 in wrong order and
					               did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 5 expected items:
					                 "d",
					                 "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ProperlyInAnyOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it lacked all 11 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked all 3 expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it lacked all 10 expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsAndAnAdditionalItemWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([2, 1,]).Properly().InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("all expected items and an additional item already decide the result");
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 6 expected items:
					                 "a",
					                 "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithManyAdditionalItems_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([..Enumerable.Range(100, 25), 1,]);
				int[] expected = [1,];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 5 expected items:
					                 "d",
					                 "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}


			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
				IEnumerable<int> expected = Enumerable.Range(100, 11).ToArray();

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it lacked all 11 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10,
					               (… and 1 more)
					             ]

					             Expected:
					             [
					               100,
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 3 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 2 unique expected items

					             Collection:
					             []

					             Expected:
					             [
					               "a",
					               "a",
					               "b"
					             ]
					             """);
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it lacked all 10 unique expected items

					             Collection:
					             [
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               10
					             ]

					             Expected:
					             [
					               101,
					               102,
					               103,
					               104,
					               105,
					               106,
					               107,
					               108,
					               109,
					               110
					             ]
					             """);
			}

			[Test]
			public async Task WhenAllExpectedItemsAndAnAdditionalItemWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([2, 1,]).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow()
					.Because("all expected items and an additional item already decide the result");
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               "x",
					               "y",
					               "z"

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "x",
					               "y",
					               "z"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 5 expected items:
					                 "a",
					                 "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Test]
			public async Task WithManyAdditionalItems_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([..Enumerable.Range(100, 25), 1,]);
				int[] expected = [1,];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
					               lacked 2 of 5 expected items:
					                 "d",
					                 "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               "e"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class StringCollectionTests
		{
			[Test]
			[Arguments("[a-f]{1}[o]*", true)]
			[Arguments("[g-h]{1}[o]*", false)]
			public async Task AsRegex_ShouldUseRegex(string regex, bool expectSuccess)
			{
				string?[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains([regex,]).AsRegex();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains collection [regex,] as regex in order and contiguous,
					             but it lacked the one expected item

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]

					             Expected:
					             [
					               "[g-h]{1}[o]*"
					             ]
					             """);
			}

			[Test]
			[Arguments("?oo", true)]
			[Arguments("f??o", false)]
			public async Task AsWildcard_ShouldUseWildcard(string wildcard, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains([wildcard,]).AsWildcard();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains collection [wildcard,] as wildcard in order and contiguous,
					             but it lacked the one expected item

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]

					             Expected:
					             [
					               "f??o"
					             ]
					             """);
			}

			[Test]
			public async Task WhenIgnoringCase_WithDuplicateBeforeAnInterruptedRun_ShouldReportTheInterruptingItem()
			{
				string[] subject = ["B", "a", "x", "b",];
				string[] expected = ["A", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected ignoring case in order and contiguous,
					             but it contained item "x" at index 2 instead of "b"

					             Collection:
					             [
					               "B",
					               "a",
					               "x",
					               "b"
					             ]

					             Expected:
					             [
					               "A",
					               "b"
					             ]
					             """);
			}

			[Test]
			[Arguments("FOO", true)]
			[Arguments("goo", false)]
			public async Task WhenIgnoringCase_ShouldUseCaseInsensitiveMatch(string match, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains([match,]).IgnoringCase();

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains collection [match,] ignoring case in order and contiguous,
					             but it lacked the one expected item

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]

					             Expected:
					             [
					               "goo"
					             ]
					             """);
			}
		}

		public sealed class InSameOrderIgnoringInterspersedItemsTests
		{
			[Test]
			public async Task WhenAllExpectedItemsWereFound_ShouldStopEnumerating()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("enumerated too far"), 1, 2, 3);

				async Task Act()
					=> await That(subject).Contains([1, 3,]).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow()
					.Because("all expected items already decide the result");
			}

			[Test]
			[Arguments(false, false)]
			[Arguments(false, true)]
			[Arguments(true, false)]
			[Arguments(true, true)]
			public async Task WhenCombinedWithInAnyOrder_ShouldThrowInvalidOperationException(bool inAnyOrderFirst,
				bool negated)
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				int[] expected = [1, 3,];

				async Task Act()
				{
					switch (inAnyOrderFirst, negated)
					{
						case (true, false):
							await That(subject).Contains(expected).InAnyOrder().IgnoringInterspersedItems();
							break;
						case (true, true):
							await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringInterspersedItems();
							break;
						case (false, false):
							await That(subject).Contains(expected).IgnoringInterspersedItems().InAnyOrder();
							break;
						case (false, true):
							await That(subject).DoesNotContain(expected).IgnoringInterspersedItems().InAnyOrder();
							break;
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage(inAnyOrderFirst
						? "IgnoringInterspersedItems cannot be combined with InAnyOrder."
						: "InAnyOrder cannot be combined with IgnoringInterspersedItems.")
					.Because("the any-order match never requires contiguous items, so the option would silently be dropped");
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenFalseIsCombinedWithInAnyOrder_ShouldMatchInAnyOrder(bool inAnyOrderFirst)
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				int[] expected = [3, 1,];

				async Task Act()
				{
					if (inAnyOrderFirst)
					{
						await That(subject).Contains(expected).InAnyOrder().IgnoringInterspersedItems(false);
					}
					else
					{
						await That(subject).Contains(expected).IgnoringInterspersedItems(false).InAnyOrder();
					}
				}

				await That(Act).DoesNotThrow()
					.Because("not ignoring interspersed items states the default, which does not compete with the order");
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldFailForInterspersedItems()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				int[] expected = [1, 3,];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it contained item 2 at index 1 instead of 3

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [1, 3]
					             """)
					.Because("the items are required to be contiguous, as without the option");
			}

			[Test]
			public async Task WhenFalseIsSpecified_ShouldSucceedForTheNegationWithInterspersedItems()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				int[] expected = [1, 3,];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringInterspersedItems(false);

				await That(Act).DoesNotThrow()
					.Because("the items are required to be contiguous, as without the option");
			}

			[Test]
			[Arguments(true, true)]
			[Arguments(true, false)]
			[Arguments(false, true)]
			[Arguments(false, false)]
			public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException(bool first, bool second)
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				int[] expected = [1, 3,];

				async Task Act()
					=> await That(subject).Contains(expected)
						.IgnoringInterspersedItems(first).IgnoringInterspersedItems(second);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IgnoringInterspersedItems cannot be specified more than once.")
					.Because("the second value would silently replace the first one");
			}

			[Test]
			public async Task WithDuplicateBeforeTheItemsInOrder_ShouldOnlyReportTheMissingItem()
			{
				IEnumerable<int> subject = ToEnumerable([2, 1, 2,]);
				int[] expected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring interspersed items,
					             but it lacked 1 of 3 expected items: 3

					             Collection:
					             [2, 1, 2]

					             Expected:
					             [1, 2, 3]
					             """)
					.Because("the last 2 follows the 1, so the found items are in order");
			}

			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring interspersed items,
					             but it contained item "b" at index 1 in wrong order

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSubsetAfterAbandonedPartialMatch_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 4, 2, 3,]);
				int[] expected = [2, 3,];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class InSameOrderIgnoringDuplicatesAndInterspersedItemsTests
		{
			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates and interspersed items,
					             but it contained item "b" at index 1 in wrong order

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ProperlyInSameOrderIgnoringInterspersedItemsTests
		{
			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring interspersed items,
					             but it contained item "b" at index 1 in wrong order

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring interspersed items,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ProperlyInSameOrderIgnoringDuplicatesAndInterspersedItemsTests
		{
			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates()
						.IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems()
						.IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates and interspersed items,
					             but it contained item "b" at index 1 in wrong order

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "b",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates()
						.IgnoringInterspersedItems();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates and interspersed items,
					             but it did not contain any additional items

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}
	}
}
