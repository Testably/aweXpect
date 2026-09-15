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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
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

			[Fact]
			public async Task WhenExpectedContainsDuplicateButMissingItems_ShouldFail()
			{
				int[] collection = [1, 2, 1, 3, 12, 2, 2,];

				async Task Act()
					=> await That(collection).Contains([1, 2, 1, 1, 2,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that collection
					             contains collection [1, 2, 1, 1, 2,] in order,
					             but it
					               contained item 3 at index 3 instead of 1 and
					               contained item 12 at index 4 instead of 2

					             Collection:
					             [1, 2, 1, 3, 12, 2, 2]

					             Expected:
					             [1, 2, 1, 1, 2]
					             """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;
				IEnumerable<string?> expected = ["foo",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "d" at index 3 instead of "x" and
					               contained item "e" at index 4 instead of "y" and
					               lacked 3 of 6 expected items:
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked all 6 expected items

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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "c" at index 1 instead of "b" and
					               contained item "b" at index 2 instead of "c"

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

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
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

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "b" at index 1 instead of "a" and
					               contained item "c" at index 2 instead of "b" and
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

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked 1 of 2 expected items: "a"

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

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
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

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class InSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
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

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
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

			[Fact]
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

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!).IgnoringDuplicates();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of "x" and
					               contained item "e" at index 4 instead of "y" and
					               lacked 3 of 6 expected items:
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it lacked all 5 unique expected items

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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of "b" and
					               contained item "b" at index 2 instead of "c"

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

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it lacked 1 of 2 expected items: "a"

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

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
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

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class InAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!).InAnyOrder();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
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


			[Fact]
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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
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

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "d" at index 3 instead of "x" and
					               contained item "e" at index 4 instead of "y" and
					               did not contain any additional items and
					               lacked 3 of 6 expected items:
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               did not contain any additional items and
					               lacked all 6 expected items

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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "c" at index 1 instead of "b" and
					               contained item "b" at index 2 instead of "c" and
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

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
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

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "b" at index 1 instead of "a" and
					               contained item "c" at index 2 instead of "b" and
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

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               did not contain any additional items and
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


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
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

		public sealed class ProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of "x" and
					               contained item "e" at index 4 instead of "y" and
					               lacked 3 of 6 expected items:
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 5 unique expected items

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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of "b" and
					               contained item "b" at index 2 instead of "c" and
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

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               did not contain any additional items and
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

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
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


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it had more than 20 deviations

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
					               …
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
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
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

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
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

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
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
			[Theory]
			[InlineData("[a-f]{1}[o]*", true)]
			[InlineData("[g-h]{1}[o]*", false)]
			public async Task AsRegex_ShouldUseRegex(string regex, bool expectSuccess)
			{
				string?[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains([regex,]).AsRegex();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains collection [regex,] in order as regex,
					             but it lacked all 1 expected items

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

			[Theory]
			[InlineData("?oo", true)]
			[InlineData("f??o", false)]
			public async Task AsWildcard_ShouldUseWildcard(string wildcard, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains([wildcard,]).AsWildcard();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains collection [wildcard,] in order as wildcard,
					             but it lacked all 1 expected items

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

			[Theory]
			[InlineData("FOO", true)]
			[InlineData("goo", false)]
			public async Task WhenIgnoringCase_ShouldUseCaseInsensitiveMatch(string match, bool expectSuccess)
			{
				string[] subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).Contains([match,]).IgnoringCase();

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             contains collection [match,] in order ignoring case,
					             but it lacked all 1 expected items

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
			[Fact]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring interspersed items,
					             but it lacked 1 of 2 expected items: "a"

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

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class InSameOrderIgnoringDuplicatesAndInterspersedItemsTests
		{
			[Fact]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates and interspersed items,
					             but it lacked 1 of 2 expected items: "a"

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

			[Fact]
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
			[Fact]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring interspersed items,
					             but it lacked 1 of 2 expected items: "a"

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

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems();

				await That(Act).Throws<XunitException>()
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
			[Fact]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates()
						.IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["b", "a",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates and interspersed items,
					             but it lacked 1 of 2 expected items: "a"

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

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates()
						.IgnoringInterspersedItems();

				await That(Act).Throws<XunitException>()
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
