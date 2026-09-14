using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsContainedIn
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>>? expected = null;

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
					=> await That(subject).IsContainedIn(Array.Empty<Action<IThat<string?>>>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection Array.Empty<Action<IThat<string?>>>() in order,
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y"

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected);

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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y"

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order ignoring duplicates,
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

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
			public async Task EmptyCollection_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y"

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order,
					             but it
					               contained item "c" at index 1 instead of it is equal to "b" and
					               contained item "b" at index 2 instead of it is equal to "c" and
					               contained all expected items

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of it is equal to "x" and
					               contained item "e" at index 4 instead of it is equal to "y"

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of it is equal to "b" and
					               contained item "b" at index 2 instead of it is equal to "c" and
					               contained all expected items

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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
			public async Task EmptyCollection_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}
		}
	}
}
