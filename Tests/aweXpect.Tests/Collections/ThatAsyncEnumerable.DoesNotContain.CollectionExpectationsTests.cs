#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class DoesNotContain
	{
		public sealed class ExpectationsInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Enumerable.Range(1, 11));
				IEnumerable<Action<IThat<int>>>? expected = null;

				async Task Act()
					=> await That(subject).DoesNotContain(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;
				IEnumerable<Action<IThat<string?>>> unexpected = [a => a.IsEqualTo("foo"),];

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection unexpected in order,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order,
					             but it did

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

		public sealed class ExpectationsInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order ignoring duplicates,
					             but it did

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

		public sealed class ExpectationsInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}


			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

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

		public sealed class ExpectationsInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

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

		public sealed class ExpectationsProperlyInSameOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsProperlyInAnyOrderTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
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
					               it is equal to "a",
					               it is equal to "b",
					               it is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ExpectationsProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["b", "b", "c", "d",]);
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order ignoring duplicates,
					             but it did

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> expected =
				[
					x => x.IsEqualTo("a"),
					x => x.IsEqualTo("b"),
					x => x.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
