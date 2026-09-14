using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsContainedIn
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

				async Task Act()
					=> await That(subject).IsContainedIn(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsContainedIn(Array.Empty<Expression<Func<string, bool>>>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection Array.Empty<Expression<Func<string, bool>>>() in order,
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y")

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it contained item "c" at index 0 that was not expected

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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it contained item "c" at index 3 that was not expected

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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it contained item "a" at index 0 that was not expected

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
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected);

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y")

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it contained item "c" at index 3 that was not expected

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it contained item "c" at index 3 that was not expected

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it contained item "a" at index 1 that was not expected

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
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order ignoring duplicates,
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
			public async Task EmptyCollection_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order ignoring duplicates,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order ignoring duplicates,
					             but it contained item "d" at index 3 that was not expected

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y")

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained all expected items

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               contained all expected items

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c") and
					               contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "c" at index 0 that was not expected and
					               contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "c" at index 3 that was not expected and
					               contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "a" at index 0 that was not expected and
					               contained all expected items

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
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y")

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained all expected items

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               contained all expected items

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c") and
					               contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it contained all expected items

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
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained all expected items

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               contained all expected items

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it
					               contained item "c" at index 3 that was not expected and
					               contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it
					               contained item "c" at index 3 that was not expected and
					               contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it
					               contained item "a" at index 1 that was not expected and
					               contained all expected items

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
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
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
			public async Task EmptyCollection_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it
					               contained item 1 at index 0 that was not expected and
					               contained item 2 at index 1 that was not expected and
					               contained item 3 at index 2 that was not expected and
					               contained item 4 at index 3 that was not expected and
					               contained item 5 at index 4 that was not expected and
					               contained item 6 at index 5 that was not expected and
					               contained item 7 at index 6 that was not expected and
					               contained item 8 at index 7 that was not expected and
					               contained item 9 at index 8 that was not expected and
					               contained item 10 at index 9 that was not expected

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained all expected items

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d"
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
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               contained all expected items

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
					               x => (x == "c")
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
