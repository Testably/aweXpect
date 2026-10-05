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
	public sealed partial class IsContainedIn
	{
		public sealed class ImmutableInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
			public async Task WhenExpectedExpectationsContainNull_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<Action<IThat<int>>> expected = [a => a.IsEqualTo(1), null!,];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int>? expected = null;

				async Task Act()
					=> await That(subject).IsContainedIn(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedPredicatesContainNull_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<Expression<Func<int, bool>>> expected = [a => a == 1, null!,];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot contain <null>.").AsPrefix();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
					             but it contained item "c" at index 1 in wrong order

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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous ignoring duplicates,
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
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous ignoring duplicates,
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous ignoring duplicates,
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous ignoring duplicates,
					             but it contained item "c" at index 1 in wrong order

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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous ignoring duplicates,
					             but it contained item "c" at index 0 in wrong order

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
					             """)
					.Because("ignoring duplicates must only relax duplicates, not the order of the remaining items");
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
					               contained item 10 at index 9 that was not expected and
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}


			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

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
					               contained item 10 at index 9 that was not expected and
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableProperlyInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
					             but it
					               contained item "c" at index 1 in wrong order and
					               contained all expected items

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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
					             but it contained all expected items

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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
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
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               contained item "c" at index 1 in wrong order and
					               contained all expected items

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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
					             but it
					               contained item "c" at index 0 in wrong order and
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
					             but it contained all expected items

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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
					             but it contained all expected items

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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous ignoring duplicates,
					             but it contained all expected items

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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
					             but it contained all expected items

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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order,
					             but it contained all expected items

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
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
					               contained item 11 at index 10 that was not expected

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

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldFail()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
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
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in any order ignoring duplicates,
					             but it contained all expected items

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

		public sealed class ImmutableWithExpectationsTests
		{
			[Fact]
			public async Task IgnoringDuplicates_WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 1, 2,];

				async Task Act()
					=> await That(subject).IsContainedIn([
						x => x.IsEqualTo(0),
						x => x.IsEqualTo(1),
						x => x.IsEqualTo(2),
					]).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IgnoringInterspersedItems_WithItemsInBetween_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 3,];

				async Task Act()
					=> await That(subject).IsContainedIn([
						x => x.IsEqualTo(1),
						x => x.IsEqualTo(2),
						x => x.IsEqualTo(3),
					]).IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task InAnyOrder_WithItemsInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<int> subject = [3, 2,];

				async Task Act()
					=> await That(subject).IsContainedIn([
						x => x.IsEqualTo(1),
						x => x.IsEqualTo(2),
						x => x.IsEqualTo(3),
					]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Properly_WhenExpectedHasNoAdditionalItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2,];
				List<Action<IThat<int>>> expected =
				[
					x => x.IsEqualTo(1),
					x => x.IsEqualTo(2),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected that has at least one additional item in order and contiguous,
					             but it contained all expected items

					             Collection:
					             [1, 2]

					             Expected:
					             [an item that is equal to 1, an item that is equal to 2]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				List<Action<IThat<int>>> expected =
				[
					x => x.IsEqualTo(1),
					x => x.IsEqualTo(2),
				];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
					             but it contained item 3 at index 2 that was not expected

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [an item that is equal to 1, an item that is equal to 2]
					             """);
			}

			[Fact]
			public async Task WithContainedItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [2, 3,];

				async Task Act()
					=> await That(subject).IsContainedIn([
						x => x.IsEqualTo(1),
						x => x.IsEqualTo(2),
						x => x.IsGreaterThan(2),
					]);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableWithPredicatesTests
		{
			[Fact]
			public async Task IgnoringDuplicates_WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 1, 2,];

				async Task Act()
					=> await That(subject).IsContainedIn([x => x == 0, x => x == 1, x => x == 2,]).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IgnoringInterspersedItems_WithItemsInBetween_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 3,];

				async Task Act()
					=> await That(subject).IsContainedIn([x => x == 1, x => x == 2, x => x == 3,])
						.IgnoringInterspersedItems();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task InAnyOrder_WithItemsInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<int> subject = [3, 2,];

				async Task Act()
					=> await That(subject).IsContainedIn([x => x == 1, x => x == 2, x => x == 3,]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Properly_WhenExpectedHasNoAdditionalItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2,];

				async Task Act()
					=> await That(subject).IsContainedIn([x => x == 1, x => x == 2,]).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection [x => x == 1, x => x == 2,] that has at least one additional item in order and contiguous,
					             but it contained all expected items

					             Collection:
					             [1, 2]

					             Expected:
					             [
					               x => (x == 1),
					               x => (x == 2)
					             ]
					             """);
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				List<Expression<Func<int, bool>>> expected = [x => x == 1, x => x == 2,];

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
					             but it contained item 3 at index 2 that was not expected

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               x => (x == 1),
					               x => (x == 2)
					             ]
					             """);
			}

			[Fact]
			public async Task WithContainedItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [2, 3,];

				async Task Act()
					=> await That(subject).IsContainedIn([x => x == 1, x => x == 2, x => x > 2,]);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
