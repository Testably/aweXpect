#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class WithPredicatesInSameOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Expression<Func<int, bool>>>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Expression<Func<int, bool>>>() in order,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

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
					               (… and maybe more)
					             ]
					             
					             Expected:
					             []
					             """);
			}


			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 1 at index 0 instead of a => (a == 100) and
					               contained item 2 at index 1 instead of a => (a == 101) and
					               contained item 3 at index 2 instead of a => (a == 102) and
					               contained item 4 at index 3 instead of a => (a == 103) and
					               contained item 5 at index 4 instead of a => (a == 104) and
					               contained item 6 at index 5 instead of a => (a == 105) and
					               contained item 7 at index 6 instead of a => (a == 106) and
					               contained item 8 at index 7 instead of a => (a == 107) and
					               contained item 9 at index 8 instead of a => (a == 108) and
					               contained item 10 at index 9 instead of a => (a == 109) and
					               contained item 11 at index 10 instead of a => (a == 110)

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

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());

				async Task Act()
					=> await That(subject).IsEqualTo([
						x => x == "a",
						x => x == "b",
						x => x == "c",
					]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection [
					             	x => x == "a",
					             	x => x == "b",
					             	x => x == "c",
					             ] in order,
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item 1 at index 0 instead of a => (a == 101) and
					               contained item 2 at index 1 instead of a => (a == 102) and
					               contained item 3 at index 2 instead of a => (a == 103) and
					               contained item 4 at index 3 instead of a => (a == 104) and
					               contained item 5 at index 4 instead of a => (a == 105) and
					               contained item 6 at index 5 instead of a => (a == 106) and
					               contained item 7 at index 6 instead of a => (a == 107) and
					               contained item 8 at index 7 instead of a => (a == 108) and
					               contained item 9 at index 8 instead of a => (a == 109) and
					               contained item 10 at index 9 instead of a => (a == 110)

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
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

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
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldSucceed()
			{
				IEnumerable<int>? subject = null;
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected!);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;
				IEnumerable<Expression<Func<string, bool>>> expected = [];

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
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "d" at index 3 instead of x => (x == "x") and
					               contained item "e" at index 4 instead of x => (x == "y") and
					               lacked 1 of 6 expected items: x => (x == "z")
					             
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
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c")
					             (but the items match in a different order)

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
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithPredicatesInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Expression<Func<int, bool>>>()).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Expression<Func<int, bool>>>() in order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

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
					               (… and maybe more)
					             ]
					             
					             Expected:
					             []
					             """);
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

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

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "a",
					x => x == "b",
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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
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
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
					             but it
					               contained item "c" at index 1 instead of x => (x == "b") and
					               contained item "b" at index 2 instead of x => (x == "c")
					             (but the items match in a different order)

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
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in order ignoring duplicates,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithPredicatesInAnyOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Expression<Func<int, bool>>>()).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Expression<Func<int, bool>>>() in any order,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

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
					               (… and maybe more)
					             ]
					             
					             Expected:
					             []
					             """);
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

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

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
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
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithPredicatesInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 21));

				async Task Act()
					=> await That(subject).IsEqualTo(Array.Empty<Expression<Func<int, bool>>>()).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection Array.Empty<Expression<Func<int, bool>>>() in any order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

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
					               (… and maybe more)
					             ]
					             
					             Expected:
					             []
					             """);
			}


			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
					             but it had more than 20 deviations:
					               contained item 1 at index 0 that was not expected,
					               contained item 2 at index 1 that was not expected,
					               contained item 3 at index 2 that was not expected,
					               contained item 4 at index 3 that was not expected,
					               contained item 5 at index 4 that was not expected,
					               contained item 6 at index 5 that was not expected,
					               contained item 7 at index 6 that was not expected,
					               contained item 8 at index 7 that was not expected,
					               contained item 9 at index 8 that was not expected,
					               contained item 10 at index 9 that was not expected,
					               (… and maybe more)

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

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "a",
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "a",
					x => x == "b",
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
					               x => (x == "a"),
					               x => (x == "a"),
					               x => (x == "b")
					             ]
					             """);
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<Expression<Func<int, bool>>> expected = [
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
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
			public async Task WithAdditionalItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
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
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection expected in any order ignoring duplicates,
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected = [
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
