using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class ExpectationsInSameOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<Action<IThat<int>>>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());

				async Task Act()
					=> await That(subject).IsNotEqualTo([
						a => a.IsEqualTo("a"),
						a => a.IsEqualTo("b"),
						a => a.IsEqualTo("c"),
					]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;
				IEnumerable<Action<IThat<int>>>? unexpected = null;

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
				IEnumerable<Action<IThat<string?>>> unexpected = [];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("x"),
					a => a.IsEqualTo("y"),
					a => a.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
					a => a.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsInSameOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<Action<IThat<int>>>())
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("x"),
					a => a.IsEqualTo("y"),
					a => a.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("c"),
				];

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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
					a => a.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsInAnyOrderTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<Action<IThat<int>>>()).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("x"),
					a => a.IsEqualTo("y"),
					a => a.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
					a => a.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}
		}

		public sealed class ExpectationsInAnyOrderIgnoringDuplicatesTests
		{
			[Fact]
			public async Task CollectionWithMoreThan20Deviations_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 21);

				async Task Act()
					=> await That(subject).IsNotEqualTo(Array.Empty<Action<IThat<int>>>()).InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("a"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Action<IThat<int>>> unexpected =
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
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("x"),
					a => a.IsEqualTo("y"),
					a => a.IsEqualTo("z"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAdditionalItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("c"),
				];

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
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
					a => a.IsEqualTo("d"),
					a => a.IsEqualTo("e"),
				];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Action<IThat<string?>>> unexpected =
				[
					a => a.IsEqualTo("a"),
					a => a.IsEqualTo("b"),
					a => a.IsEqualTo("c"),
				];

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
					               an item that is equal to "a",
					               an item that is equal to "b",
					               an item that is equal to "c"
					             ]
					             """);
			}
		}
	}
}
