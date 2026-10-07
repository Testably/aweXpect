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
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
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

			[Test]
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

			[Test]
			public async Task WhenExpectedContainsDuplicateButMissingItems_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 1, 3, 12, 2, 2,]);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					a => a == 1,
					a => a == 2,
					a => a == 1,
					a => a == 1,
					a => a == 2,
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 5 expected items: a => (a == 1)

					             Collection:
					             [1, 2, 1, 3, 12, 2, 2]

					             Expected:
					             [
					               a => (a == 1),
					               a => (a == 2),
					               a => (a == 1),
					               a => (a == 1),
					               a => (a == 2)
					             ]
					             """);
			}

			[Test]
			public async Task WhenExpectedContainsNull_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 3);
				IEnumerable<Expression<Func<int, bool>>> expected = [a => a == 1, null!,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
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

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenLazyExpectedContainsNull_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 3);
				IEnumerable<Expression<Func<int, bool>>> expected =
					ToEnumerable<Expression<Func<int, bool>>>(a => a == 1, null!);

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;
				IEnumerable<Expression<Func<string, bool>>> expected = [a => a == "foo",];

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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
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

			[Test]
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

			[Test]
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
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

			[Test]
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

			[Test]
			public async Task WithItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "c",
					x => x == "a",
				];

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
					               x => (x == "c"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Test]
			public async Task WithItemsMatchingTheSamePredicateInAnInterruptedRun_ShouldReportTheItemThatMatchesNone()
			{
				IEnumerable<int> subject = ToEnumerable([1, 5, 0, 7,]);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
					x => x == 1,
					x => x == 0 || x == 5,
					x => x == 7,
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it contained item 0 at index 2 instead of x => (x == 7)

					             Collection:
					             [1, 5, 0, 7]

					             Expected:
					             [
					               x => (x == 1),
					               x => ((x == 0) OrElse (x == 5)),
					               x => (x == 7)
					             ]
					             """)
					.Because("the 5 matches the second predicate, so it is not reported instead of it");
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
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

			[Test]
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
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
					             but it lacked all 3 expected items

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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
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

			[Test]
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

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
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

			[Test]
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

			[Test]
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
			public async Task WithItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "c",
					x => x == "a",
				];

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
					               x => (x == "c"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous ignoring duplicates,
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

			[Test]
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
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

			[Test]
			public async Task WhenAnItemOnlyMatchesAnAlreadyMatchedPredicate_ShouldStopOnceDecided()
			{
				int readItems = 0;

				IEnumerable<int> Source()
				{
					readItems++;
					yield return 6;
					for (int i = 0; i < 10000; i++)
					{
						readItems++;
						yield return 3;
					}
				}

				async Task Act()
					=> await That(Source()).Contains([x => x > 0, x => x > 5,]).InAnyOrder();

				await That(Act).DoesNotThrow();
				await That(readItems).IsEqualTo(2)
					.Because("6 and 3 already satisfy both predicates");
			}

			[Test]
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

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
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


			[Test]
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
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked all 5 expected items

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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
					             but it lacked all 3 expected items

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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
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

			[Test]
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

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in any order ignoring duplicates,
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items
					             and
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

			[Test]
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

			[Test]
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it
					               did not contain any additional items
					             and
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


			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 3 expected items

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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
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

			[Test]
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

			[Test]
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInAnyOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items
					             and
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

			[Test]
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

			[Test]
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order,
					             but it
					               did not contain any additional items
					             and
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


			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11).ToArray();
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
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
					               (… and 1 more)
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 5 expected items

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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items and
					               lacked all 3 expected items

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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
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

			[Test]
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

			[Test]
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in any order ignoring duplicates,
					             but it
					               did not contain any additional items
					             and
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

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesInSameOrderIgnoringInterspersedItemsTests
		{
			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "b",
					x => x == "a",
				];

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
					               x => (x == "b"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Test]
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
			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "b",
					x => x == "a",
				];

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
					               x => (x == "b"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Test]
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
			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "b",
					x => x == "a",
				];

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
					               x => (x == "b"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInSameOrderIgnoringDuplicatesAndInterspersedItemsTests
		{
			[Test]
			public async Task WithInterspersedItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates()
						.IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithInterspersedItemsInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "b",
					x => x == "a",
				];

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
					               x => (x == "b"),
					               x => (x == "a")
					             ]
					             """);
			}

			[Test]
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}
	}
}
