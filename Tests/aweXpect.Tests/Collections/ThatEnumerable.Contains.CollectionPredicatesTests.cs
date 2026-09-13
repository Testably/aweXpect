using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class PredicatesInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
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
				IEnumerable<Expression<Func<int, bool>>> expected = [];

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
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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
				IEnumerable<Expression<Func<string, bool>>> expected = [a => a == "foo",];

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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y") and
					               lacked 3 of 6 expected items:
					                 x => (x == "x"),
					                 x => (x == "y"),
					                 x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c")

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked 1 of 4 expected items: x => (x == "c")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "b" at index 1 instead of x => (x == "a") and
					               contained item "c" at index 2 instead of x => (x == "b") and
					               lacked 1 of 4 expected items: x => (x == "a")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked 2 of 5 expected items:
					               x => (x == "d"),
					               x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected = [];

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
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y") and
					               lacked 3 of 6 expected items:
					                 x => (x == "x"),
					                 x => (x == "y"),
					                 x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c")

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               x => (x == "d"),
					               x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected = [];

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
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 3 of 6 expected items:
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 2 of 6 expected items:
					               x => (x == "a"),
					               x => (x == "e")

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: x => (x == "c")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: x => (x == "a")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 2 of 5 expected items:
					               x => (x == "d"),
					               x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "a",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected = [];

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
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               x => (x == "a"),
					               x => (x == "e")

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               x => (x == "d"),
					               x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesProperlyInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y") and
					               did not contain any additional items and
					               lacked 3 of 6 expected items:
					                 x => (x == "x"),
					                 x => (x == "y"),
					                 x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c") and
					               did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "c")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "b" at index 1 instead of x => (x == "a") and
					               contained item "c" at index 2 instead of x => (x == "b") and
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "a")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               did not contain any additional items and
					               lacked 2 of 5 expected items:
					                 x => (x == "d"),
					                 x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y") and
					               lacked 3 of 6 expected items:
					                 x => (x == "x"),
					                 x => (x == "y"),
					                 x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c") and
					               did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 2 of 5 expected items:
					                 x => (x == "d"),
					                 x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it lacked 3 of 6 expected items:
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 2 of 6 expected items:
					                 x => (x == "a"),
					                 x => (x == "e")

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "c")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "a")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 2 of 5 expected items:
					                 x => (x == "d"),
					                 x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 100,
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 100),
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "a",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 101,
					a => a == 102,
					a => a == 103,
					a => a == 104,
					a => a == 105,
					a => a == 106,
					a => a == 107,
					a => a == 108,
					a => a == 109,
					a => a == 110,
				];

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
					               a => (a == 101),
					               a => (a == 102),
					               a => (a == 103),
					               a => (a == 104),
					               a => (a == 105),
					               a => (a == 106),
					               a => (a == 107),
					               a => (a == 108),
					               a => (a == 109),
					               a => (a == 110)
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "x"),
					               x => (x == "y"),
					               x => (x == "z")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 2 of 5 expected items:
					                 x => (x == "a"),
					                 x => (x == "e")

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: x => (x == "d")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 2 of 5 expected items:
					                 x => (x == "d"),
					                 x => (x == "e")

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "d"),
					               x => (x == "e")
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesInSameOrderIgnoringInterspersedItemsTests
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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesInSameOrderIgnoringDuplicatesAndInterspersedItemsTests
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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesProperlyInSameOrderIgnoringInterspersedItemsTests
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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInSameOrderIgnoringDuplicatesAndInterspersedItemsTests
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
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}
	}
}
