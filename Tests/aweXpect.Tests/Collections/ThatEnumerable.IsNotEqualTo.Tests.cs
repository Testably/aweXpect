using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class InSameOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<int>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).DoesNotThrow();
			}
			
			[Fact]
			public async Task WhenReferenceTypeDoesNotMatchNullability_ShouldStillWork()
			{
				IEnumerable<string?> subject = ["foo", null, "bar",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(["foo", "", "bar",]);
				
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
			public async Task WhenTypeDoesNotMatchNullability_ShouldStillWork()
			{
				IEnumerable<int?> subject = [1, null, 3,];

				async Task Act()
					=> await That(subject).IsNotEqualTo([1, 2, 3,]);
				
				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order,
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

		public sealed class InSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<int>()).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] unexpected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
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
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring duplicates,
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

		public sealed class InAnyOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<int>()).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order,
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
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order,
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

		public sealed class InAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<int>()).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<int> unexpected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] unexpected = ["a", "a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				string[] unexpected = ["a", "a", "b",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<int> unexpected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
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
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
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
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c", "d", "e",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				string[] unexpected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in any order ignoring duplicates,
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

		public sealed class StringsTests
		{
			[Fact]
			public async Task AsWildcard_ShouldThrowWhenMatchingWildcard()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);
				string[] unexpected = ["*oo", "*a?", "?a?",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).AsWildcard();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order as wildcard,
					             but it did

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
				IEnumerable<string> subject = ToEnumerable([" a", "b", "\tc",]);
				string[] unexpected = ["a", " b", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring leading white-space,
					             but it did

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
				IEnumerable<string> subject = ToEnumerable(["a ", "b", "c\t",]);
				string[] unexpected = ["a", "b ", "c",];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection unexpected in order ignoring trailing white-space,
					             but it did

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
	}
}
