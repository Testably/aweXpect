using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotContain
	{
		public sealed class PredicatesInSameOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>>? expected = null;

				async Task Act()
					=> await That(subject).DoesNotContain(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenLazyUnexpectedContainsNull_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 3);
				IEnumerable<Expression<Func<int, bool>>> unexpected =
					ToEnumerable<Expression<Func<int, bool>>>(a => a == 1, null!);

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;
				IEnumerable<Expression<Func<string, bool>>> unexpected = [a => a == "foo",];

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection unexpected in order and contiguous,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedContainsNull_ShouldThrowArgumentException()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 3);
				IEnumerable<Expression<Func<int, bool>>> unexpected = [a => a == 1, null!,];

				async Task Act()
					=> await That(subject).DoesNotContain(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot contain <null>.").AsPrefix();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous,
					             but it did

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);

				await That(Act).DoesNotThrow();
			}


			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected);


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesInSameOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
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

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
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

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesInAnyOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "c",
					               "b",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).DoesNotThrow();
			}


			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesInAnyOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "a",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "c",
					               "b",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
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

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
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

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();


				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSameCollection_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).InAnyOrder().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected in any order ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}
		}

		public sealed class PredicatesProperlyInSameOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order and contiguous,
					             but it did

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order and contiguous,
					             but it did

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}


			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesProperlyInSameOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in order and contiguous ignoring duplicates,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "d",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesProperlyInAnyOrderTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
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
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
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
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
					             but it did

					             Collection:
					             [
					               "c",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection expected and at least one additional item in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "a",
					               "b",
					               "c",
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}


			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class PredicatesProperlyInAnyOrderIgnoringDuplicatesTests
		{
			[Test]
			public async Task CompletelyDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 11);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "a",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EmptyCollectionWithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(Array.Empty<string>());
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task VeryDifferentCollections_ShouldSucceed()
			{
				IEnumerable<int> subject = Enumerable.Range(1, 10);
				IEnumerable<Expression<Func<int, bool>>> expected =
				[
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
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalAndMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "x",
					x => x == "y",
					x => x == "z",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalExpectedItemAtBeginningAndEnd_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["b", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithAdditionalItem_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<FailException>()
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
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithAdditionalItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "d", "e",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).Throws<FailException>()
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
					               (… and maybe more)
					             ]

					             Expected:
					             [
					               x => (x == "a"),
					               x => (x == "b"),
					               x => (x == "c")
					             ]
					             """);
			}

			[Test]
			public async Task WithCollectionInDifferentOrder_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "c", "b",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtBeginOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesAtEndOfSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInExpected_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithDuplicatesInSubject_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItem_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithMissingItems_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
					x => x == "d",
					x => x == "e",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSameCollection_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);
				IEnumerable<Expression<Func<string, bool>>> expected =
				[
					x => x == "a",
					x => x == "b",
					x => x == "c",
				];

				async Task Act()
					=> await That(subject).DoesNotContain(expected).Properly().InAnyOrder()
						.IgnoringDuplicates();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
