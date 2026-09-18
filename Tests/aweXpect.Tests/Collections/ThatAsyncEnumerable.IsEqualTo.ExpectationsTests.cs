#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class WithExpectationsInSameOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Action<IThat<int>>>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Action<IThat<int>>>() in order,
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
					             []
					             """);
			}


			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
					               an item that is equal to 100,
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());

				async Task Act()
					=> await That(subject).IsEqualTo([
						x => x.IsEqualTo("a"),
						x => x.IsEqualTo("b"),
						x => x.IsEqualTo("c"),
					]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection [
					             	x => x.IsEqualTo("a"),
					             	x => x.IsEqualTo("b"),
					             	x => x.IsEqualTo("c"),
					             ] in order,
					             but it lacked all 3 expected items

					             Collection:
					             []
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
					               contained item 10 at index 9 that was not expected and
					               lacked all 10 expected items

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
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               an item that is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<Action<IThat<int>>>? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but the expected collection was <null>

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
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldSucceed()
			{
				IEnumerable<int>? subject = null;
				IEnumerable<Action<IThat<int>>>? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected!);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;
				IEnumerable<Action<IThat<string?>>> expected = [];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "d" at index 3 instead of an item that is equal to "x" and
					               contained item "e" at index 4 instead of an item that is equal to "y" and
					               lacked 3 of 6 expected items:
					                 an item that is equal to "x",
					                 an item that is equal to "y",
					                 an item that is equal to "z"
					             
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "x",
					               an item that is equal to "y",
					               an item that is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "c" at index 1 instead of an item that is equal to "b" and
					               contained item "b" at index 2 instead of an item that is equal to "c"
					             
					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it lacked 1 of 4 expected items: an item that is equal to "c"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "b" at index 1 instead of an item that is equal to "a" and
					               contained item "c" at index 2 instead of an item that is equal to "b" and
					               lacked 1 of 4 expected items: an item that is equal to "a"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it lacked 1 of 4 expected items: an item that is equal to "d"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it lacked 2 of 5 expected items:
					               an item that is equal to "d",
					               an item that is equal to "e"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d",
					               an item that is equal to "e"
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Action<IThat<int>>>()).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Action<IThat<int>>>() in order ignoring duplicates,
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
					             []
					             """);
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
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
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
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
					               an item that is equal to 100,
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked all 3 unique expected items
					             
					             Collection:
					             []
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked all 2 unique expected items

					             Collection:
					             []
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "a",
					               an item that is equal to "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
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
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
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
					               contained item 10 at index 9 that was not expected and
					               lacked all 10 unique expected items

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
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               an item that is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
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
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item "d" at index 3 instead of an item that is equal to "x" and
					               contained item "e" at index 4 instead of an item that is equal to "y" and
					               lacked 3 of 6 expected items:
					                 an item that is equal to "x",
					                 an item that is equal to "y",
					                 an item that is equal to "z"
					             
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "x",
					               an item that is equal to "y",
					               an item that is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of an item that is equal to "b" and
					               contained item "b" at index 2 instead of an item that is equal to "c"
					             
					             Collection:
					             [
					               "a",
					               "c",
					               "b"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked 1 of 4 expected items: an item that is equal to "d"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               an item that is equal to "d",
					               an item that is equal to "e"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d",
					               an item that is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsInAnyOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Action<IThat<int>>>()).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Action<IThat<int>>>() in any order,
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
					             []
					             """);
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
					               an item that is equal to 100,
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked all 3 expected items

					             Collection:
					             []
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
					               contained item 10 at index 9 that was not expected and
					               lacked all 10 expected items

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
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               an item that is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               lacked 3 of 6 expected items:
					                 an item that is equal to "x",
					                 an item that is equal to "y",
					                 an item that is equal to "z"
					             
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "x",
					               an item that is equal to "y",
					               an item that is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 4 expected items: an item that is equal to "c"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 4 expected items: an item that is equal to "a"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 1 of 4 expected items: an item that is equal to "d"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it lacked 2 of 5 expected items:
					               an item that is equal to "d",
					               an item that is equal to "e"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d",
					               an item that is equal to "e"
					             ]
					             """);
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithExpectationsInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Action<IThat<int>>>()).InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Action<IThat<int>>>() in any order ignoring duplicates,
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
					             []
					             """);
			}


			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
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
					               an item that is equal to 100,
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               (… and 1 more)
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("a"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked all 3 unique expected items

					             Collection:
					             []
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "a"
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked all 2 unique expected items

					             Collection:
					             []
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "a",
					               an item that is equal to "b"
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
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
					               contained item 10 at index 9 that was not expected and
					               lacked all 10 unique expected items

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
					               an item that is equal to 101,
					               an item that is equal to 102,
					               an item that is equal to 103,
					               an item that is equal to 104,
					               an item that is equal to 105,
					               an item that is equal to 106,
					               an item that is equal to 107,
					               an item that is equal to 108,
					               an item that is equal to 109,
					               an item that is equal to 110
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it
					               contained item "d" at index 3 that was not expected and
					               contained item "e" at index 4 that was not expected and
					               lacked 3 of 6 expected items:
					                 an item that is equal to "x",
					                 an item that is equal to "y",
					                 an item that is equal to "z"
					             
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "x",
					               an item that is equal to "y",
					               an item that is equal to "z"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked 1 of 4 expected items: an item that is equal to "d"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
					x => x.IsEqualTo("d"),
					x => x.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it lacked 2 of 5 expected items:
					               an item that is equal to "d",
					               an item that is equal to "e"
					             
					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             
					             Expected:
					             [
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c",
					               an item that is equal to "d",
					               an item that is equal to "e"
					             ]
					             """);
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
