#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class IsNotContainedIn
	{
		public sealed class InSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
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
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsNotContainedIn(Array.Empty<string>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection Array.Empty<string>() in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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

		public sealed class InSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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

		public sealed class InAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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

		public sealed class InAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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

		public sealed class ProperlyInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ProperlyInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<int> expected = Enumerable.Range(100, 11);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 10));
				IEnumerable<int> expected = Enumerable.Range(101, 10);

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c", "x", "y", "z",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				string[] expected = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotContainedIn(expected).Properly().InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
