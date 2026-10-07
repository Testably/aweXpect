#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class ImmutableInSameOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WhenExpectedExpectationsContainNull_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<Action<IThat<int>>> expected = [a => a.IsEqualTo(1), null!,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).Contains(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedPredicatesContainNull_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<Expression<Func<int, bool>>> expected = [a => a == 1, null!,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
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
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithItemsOfATypeWithoutTolerance_ShouldSucceed()
			{
				ImmutableArray<char> subject = ['a', 'b', 'c',];
				char[] expected = ['b', 'c',];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow()
					.Because("items without a tolerance use the general overload");
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
			public async Task WithMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<int> subject = [1, 2, 3,];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableInSameOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
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
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
			public async Task WithMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableInAnyOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableInAnyOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableProperlyInSameOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
		}

		public sealed class ImmutableProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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

		public sealed class ImmutableProperlyInAnyOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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

		public sealed class ImmutableProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<string?> subject = [];
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
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
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
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
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
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).Contains(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
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
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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
				ImmutableArray<string?> subject = ["a", "b", "c",];
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

		public sealed class ImmutableStringCollectionTests
		{
			[Test]
			[Arguments("[a-f]{1}[o]*", true)]
			[Arguments("[g-h]{1}[o]*", false)]
			public async Task AsRegex_ShouldUseRegex(string regex, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

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
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

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
			[Arguments("FOO", true)]
			[Arguments("goo", false)]
			public async Task WhenIgnoringCase_ShouldUseCaseInsensitiveMatch(string match, bool expectSuccess)
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

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

		public sealed class ImmutableWithExpectationsTests
		{
			[Test]
			public async Task IgnoringDuplicates_WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([
						x => x.IsEqualTo(1),
						x => x.IsEqualTo(2),
					]).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringInterspersedItems_WithItemsInBetween_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([
						x => x.IsEqualTo(1),
						x => x.IsEqualTo(3),
					]).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task InAnyOrder_WithItemsInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([
						x => x.IsEqualTo(3),
						x => x.IsEqualTo(2),
					]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Properly_WhenSubjectHasNoAdditionalItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2,];
				List<Action<IThat<int>>> expected =
				[
					x => x.IsEqualTo(1),
					x => x.IsEqualTo(2),
				];

				async Task Act()
					=> await That(subject).Contains(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected and at least one additional item in order and contiguous,
					             but it did not contain any additional items

					             Collection:
					             [1, 2]

					             Expected:
					             [an item that is equal to 1, an item that is equal to 2]
					             """);
			}

			[Test]
			public async Task WithContainedItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([
						x => x.IsEqualTo(2),
						x => x.IsGreaterThan(2),
					]);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				List<Action<IThat<int>>> expected =
				[
					x => x.IsEqualTo(3),
					x => x.IsEqualTo(4),
				];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: an item that is equal to 4

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [an item that is equal to 3, an item that is equal to 4]
					             """);
			}
		}

		public sealed class ImmutableWithPredicatesTests
		{
			[Test]
			public async Task IgnoringDuplicates_WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([x => x == 1, x => x == 2,]).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringInterspersedItems_WithItemsInBetween_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([x => x == 1, x => x == 3,]).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task InAnyOrder_WithItemsInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([x => x == 3, x => x == 2,]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Properly_WhenSubjectHasNoAdditionalItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2,];

				async Task Act()
					=> await That(subject).Contains([x => x == 1, x => x == 2,]).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection [x => x == 1, x => x == 2,] and at least one additional item in order and contiguous,
					             but it did not contain any additional items

					             Collection:
					             [1, 2]

					             Expected:
					             [
					               x => (x == 1),
					               x => (x == 2)
					             ]
					             """);
			}

			[Test]
			public async Task WithContainedItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Contains([x => x == 2, x => x > 2,]);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				List<Expression<Func<int, bool>>> expected = [x => x == 3, x => x == 4,];

				async Task Act()
					=> await That(subject).Contains(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains collection expected in order and contiguous,
					             but it lacked 1 of 2 expected items: x => (x == 4)

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               x => (x == 3),
					               x => (x == 4)
					             ]
					             """);
			}
		}
	}
}
#endif
