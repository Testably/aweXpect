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
	public sealed partial class IsNotEqualTo
	{
		public sealed class ImmutableInSameOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 21),];

				async Task Act()
					=> await That(subject).IsNotEqualTo([]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;
				IEnumerable<int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<string>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionExpression_WithSameItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotEqualTo([1, 2, 3,]);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection [1, 2, 3,] in order,
					             but it was

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [1, 2, 3]
					             """)
					.Because("a collection expression must compare the items and not the immutable arrays");
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithImmutableArrayWithSameItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				ImmutableArray<int> unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
					             but it was

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [1, 2, 3]
					             """)
					.Because("the items are compared, although the backing arrays are different instances");
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
					             but it was

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
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 21),];

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<int>()).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] unexpected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				ImmutableArray<string?> subject = ["c", "a", "b", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
					             but it was

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
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 21),];

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<int>()).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order,
					             but it was

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
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "a", "b", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order,
					             but it was

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
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 21),];

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<int>()).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 11),];
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] unexpected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				ImmutableArray<string?> subject = [];
				string[] unexpected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				ImmutableArray<int> subject = [..Enumerable.Range(1, 10),];
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c", "d", "e",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "c", "b",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
					             but it was

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
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
					             but it was

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

		public sealed class ImmutableStringsTests
		{
			[Fact]
			public async Task AsWildcard_ShouldThrowWhenMatchingWildcard()
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];
				string[] unexpected = ["*oo", "*a?", "?a?",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order as wildcard,
					             but it was

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]

					             Expected:
					             [
					               "*oo",
					               "*a?",
					               "?a?"
					             ]
					             """);
			}

			[Fact]
			public async Task
				IgnoringLeadingWhiteSpace_ShouldThrowWhenOnlyDifferenceIsInLeadingWhiteSpace()
			{
				ImmutableArray<string?> subject = [" a", "b", "\tc",];
				string[] unexpected = ["a", " b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring leading whitespace,
					             but it was

					             Collection:
					             [
					               " a",
					               "b",
					               "\tc"
					             ]
					             
					             Expected:
					             [
					               "a",
					               " b",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task
				IgnoringTrailingWhiteSpace_ShouldThrowWhenOnlyDifferenceIsInTrailingWhiteSpace()
			{
				ImmutableArray<string?> subject = ["a ", "b", "c\t",];
				string[] unexpected = ["a", "b ", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring trailing whitespace,
					             but it was
					             
					             Collection:
					             [
					               "a ",
					               "b",
					               "c\t"
					             ]
					             
					             Expected:
					             [
					               "a",
					               "b ",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ImmutableWithExpectationsTests
		{
			[Fact]
			public async Task InAnyOrder_WithItemsInDifferentOrder_ShouldFail()
			{
				ImmutableArray<int> subject = [3, 1, 2,];
				List<Action<IThat<int>>> unexpected =
				[
					x => x.IsEqualTo(1),
					x => x.IsEqualTo(2),
					x => x.IsEqualTo(3),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order,
					             but it was

					             Collection:
					             [3, 1, 2]

					             Expected:
					             [an item that is equal to 1, an item that is equal to 2, an item that is equal to 3]
					             """);
			}

			[Fact]
			public async Task WithDifferentItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotEqualTo([
						x => x.IsEqualTo(1),
						x => x.IsEqualTo(2),
						x => x.IsEqualTo(4),
					]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				List<Action<IThat<int>>> unexpected =
				[
					x => x.IsEqualTo(1),
					x => x.IsGreaterThan(1),
					x => x.IsEqualTo(3),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
					             but it was

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [an item that is equal to 1, an item that is greater than 1, an item that is equal to 3]
					             """);
			}
		}

		public sealed class ImmutableWithPredicatesTests
		{
			[Fact]
			public async Task InAnyOrder_WithItemsInDifferentOrder_ShouldFail()
			{
				ImmutableArray<int> subject = [3, 1, 2,];

				async Task Act()
					=> await That(subject).IsNotEqualTo([x => x == 1, x => x == 2, x => x == 3,]).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection [x => x == 1, x => x == 2, x => x == 3,] in any order,
					             but it was

					             Collection:
					             [3, 1, 2]

					             Expected:
					             [
					               x => (x == 1),
					               x => (x == 2),
					               x => (x == 3)
					             ]
					             """);
			}

			[Fact]
			public async Task WithDifferentItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotEqualTo([x => x == 1, x => x == 2, x => x == 4,]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				List<Expression<Func<int, bool>>> unexpected = [x => x == 1, x => x > 1, x => x == 3,];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
					             but it was

					             Collection:
					             [1, 2, 3]

					             Expected:
					             [
					               x => (x == 1),
					               x => (x > 1),
					               x => (x == 3)
					             ]
					             """);
			}
		}
	}
}
#endif
