using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class ExpectationsInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
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
				IEnumerable<Action<IThat<int>>> expected = [];

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
				IEnumerable<Action<IThat<int>>>? expected = null;

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
				IEnumerable<Action<IThat<string?>>> expected = [a => a.IsEqualTo("foo"),];

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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y" and
					               lacked 3 of 6 expected items:
					                 it is equal to "x",
					                 it is equal to "y",
					                 it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "c" at index 1 instead of it is equal to "b" and
					               contained item "b" at index 2 instead of it is equal to "c"

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked 1 of 4 expected items: it is equal to "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it
					               contained item "b" at index 1 instead of it is equal to "a" and
					               contained item "c" at index 2 instead of it is equal to "b" and
					               lacked 1 of 4 expected items: it is equal to "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order,
					             but it lacked 2 of 5 expected items:
					               it is equal to "d",
					               it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected = [];

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
				IEnumerable<Action<IThat<int>>>? expected = null;

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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y" and
					               lacked 3 of 6 expected items:
					                 it is equal to "x",
					                 it is equal to "y",
					                 it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of it is equal to "b" and
					               contained item "b" at index 2 instead of it is equal to "c"

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               it is equal to "d",
					               it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected = [];

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
				IEnumerable<Action<IThat<int>>>? expected = null;

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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 3 of 6 expected items:
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 2 of 6 expected items:
					               it is equal to "a",
					               it is equal to "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: it is equal to "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: it is equal to "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order,
					             but it lacked 2 of 5 expected items:
					               it is equal to "d",
					               it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("a"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "a"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected = [];

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
				IEnumerable<Action<IThat<int>>>? expected = null;

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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               it is equal to "a",
					               it is equal to "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               it is equal to "d",
					               it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsProperlyInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y" and
					               did not contain any additional items and
					               lacked 3 of 6 expected items:
					                 it is equal to "x",
					                 it is equal to "y",
					                 it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "c" at index 1 instead of it is equal to "b" and
					               contained item "b" at index 2 instead of it is equal to "c" and
					               did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               contained item "b" at index 1 instead of it is equal to "a" and
					               contained item "c" at index 2 instead of it is equal to "b" and
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					                 it is equal to "d",
					                 it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y" and
					               lacked 3 of 6 expected items:
					                 it is equal to "x",
					                 it is equal to "y",
					                 it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of it is equal to "b" and
					               contained item "b" at index 2 instead of it is equal to "c" and
					               did not contain any additional items

					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					                 it is equal to "d",
					                 it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsProperlyInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it lacked 3 of 6 expected items:
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					                 it is equal to "a",
					                 it is equal to "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "c"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "a"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					                 it is equal to "d",
					                 it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(100),
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 100,
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               …
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("a"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "a"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> expected =
				[
					a => a.IsEqualTo(101),
					a => a.IsEqualTo(102),
					a => a.IsEqualTo(103),
					a => a.IsEqualTo(104),
					a => a.IsEqualTo(105),
					a => a.IsEqualTo(106),
					a => a.IsEqualTo(107),
					a => a.IsEqualTo(108),
					a => a.IsEqualTo(109),
					a => a.IsEqualTo(110),
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
					               it is equal to 101,
					               it is equal to 102,
					               it is equal to 103,
					               it is equal to 104,
					               it is equal to 105,
					               it is equal to 106,
					               it is equal to 107,
					               it is equal to 108,
					               it is equal to 109,
					               it is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("x"),
					x => x.IsEqualTo("y"),
					x => x.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it lacked 3 of 6 expected items:
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "x",
					               it is equal to "y",
					               it is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					                 it is equal to "a",
					                 it is equal to "e"

					             Collection:
					             [
					               "b",
					               "b",
					               "c",
					               "d"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked 1 of 4 expected items: it is equal to "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
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
					                 it is equal to "d",
					                 it is equal to "e"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]

					             Expected:
					             [
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c",
					               it is equal to "d",
					               it is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsInSameOrderIgnoringInterspersedItemsTests
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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsInSameOrderIgnoringDuplicatesAndInterspersedItemsTests
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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsProperlyInSameOrderIgnoringInterspersedItemsTests
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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsProperlyInSameOrderIgnoringDuplicatesAndInterspersedItemsTests
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
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}
		}
	}
}
