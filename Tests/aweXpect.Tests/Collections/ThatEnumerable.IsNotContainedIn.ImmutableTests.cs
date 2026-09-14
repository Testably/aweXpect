#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotContainedIn
	{
		public sealed class ImmutableInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotContainedIn(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsNotContainedIn(Array.Empty<string?>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection Array.Empty<string?>() in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order,
					             but it did

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

		public sealed class ImmutableInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in order ignoring duplicates,
					             but it did

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

		public sealed class ImmutableInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order,
					             but it did

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

		public sealed class ImmutableInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected in any order ignoring duplicates,
					             but it did

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

		public sealed class ImmutableProperlyInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableProperlyInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = [];
				string[] expected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it did

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
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				ImmutableArray<string?> subject = ["b", "b", "c", "d",];
				string[] expected = ["a", "b", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection expected which has at least one additional item in any order ignoring duplicates,
					             but it did

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
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
