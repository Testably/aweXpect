using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasSingle
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it contained more than one item

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable subject = ToEnumerable([42,]);

				object? result = await That(subject).HasSingle();

				await That(result).IsEqualTo(42);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it contained more than one item

					             Collection:
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1,]);

				object? result = await That(subject).HasSingle();

				await That(result).IsEqualTo(1);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableMatchingPredicateTests
		{
			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => (int?)x > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => (int?)x > 1,
					             but it contained more than one item

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				object? result = await That(subject).HasSingle().Matching(x => (int?)x == 2);

				await That(result).IsEqualTo(2);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => (int?)x > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => (int?)x > 1,
					             but it contained more than one item

					             Collection:
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				object? result = await That(subject).HasSingle().Matching(x => (int?)x > 2);

				await That(result).IsEqualTo(3);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle().Matching(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching _ => true,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle().Matching(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.HasSingle().Matching(_ => false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching _ => false,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableMatchingTypeTests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable subject =
					ToEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyOtherClass result = await That(subject).HasSingle().Matching<MyOtherClass>();

				await That(result.Value).IsEqualTo(2);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<MyBaseClass>(
					new MyClass(1), new MyOtherClass(2), new MyClass(3));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass,
					             but it contained more than one item

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyOtherClass {
					                 Value = 2
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(
					new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyClass result = await That(subject).HasSingle().Matching<MyClass>();

				await That(result.Value).IsEqualTo(1);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<MyBaseClass>();

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass,
					             but it was empty
					             """);
			}
		}

		public sealed class EnumerableMatchingTypePredicateTests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value == 2);

				await That(result.Value).IsEqualTo(2);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyBaseClass matching x => x.Value > 1,
					             but it contained more than one item

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsNoMatchingElements_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyBaseClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>(x => x.Value > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass matching x => x.Value > 1,
					             but it did not contain any matching item
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value > 2);

				await That(result.Value).IsEqualTo(3);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<MyClass>();

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyBaseClass>(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyBaseClass matching _ => true,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyBaseClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}
		}

		public sealed class EnumerableNegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item,
					             but it had the single item 1

					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithPredicate_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Matching(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item matching _ => true,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableWhichTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasSingle().Which.Satisfies(x => (int?)x > 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that satisfies x => (int?)x > 2,
					             but it contained more than one item

					             Collection:
					             [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle().Which.Satisfies(x => (int?)x > 4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that satisfies x => (int?)x > 4,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenSingleItemDoesNotSatisfyExpectation_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([3,]);

				async Task Act()
					=> await That(subject).HasSingle().Which.Satisfies(x => (int?)x > 4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that satisfies x => (int?)x > 4,
					             but it was 3
					             """);
			}

			[Fact]
			public async Task WhenSingleItemSatisfiesExpectation_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([3,]);

				async Task Act()
					=> await That(subject).HasSingle().Which.Satisfies(x => (int?)x > 2);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
