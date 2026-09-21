using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed class EnumerablePredicateTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).HasItem(_ => true)
						.And.HasItem(_ => true).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItem(a => 5.Equals(a));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new []{0, 1, 2,};

				async Task Act()
					=> await That(subject).HasItem(_ => false).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item _ => false at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new []{0, 1, 2,};

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new []{0, 1, 2,};

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item _ => true at index 3,
					              but it did not contain any item at index 3

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasItem(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true,
					             but it did not contain any item

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(_ => true).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IEnumerable subject = ToEnumerable([0, 1, 2, 3, 4,]);

				async Task Act()
					=> await That(subject).HasItem(_ => true).WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => true with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [
					               0,
					               1,
					               2,
					               3,
					               4
					             ]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItem(_ => false).AtIndex(0).And.HasItem(_ => false).AtIndex(1).And
						.HasItem(_ => false)
				;

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item _ => false at index 0 and has item _ => false at index 1 and has item _ => false,
					             but it had item "a" at index 0 and it had item "b" at index 1 and it did not match at any index

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class EnumerableItemTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).HasItem(1)
						.And.HasItem(1).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItem(5);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Insert(2, expected);
				IEnumerable subject = values;

				async Task Act()
					=> await That(subject).HasItem(expected - 1).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item {expected - 1} at index 2,
					              but it had item {expected} at index 2

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Insert(2, expected);
				IEnumerable subject = values;

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail(int expected)
			{
				IEnumerable subject = new []{0, 1, expected,};

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item {expected} at index 3,
					              but it did not contain any item at index 3

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				IEnumerable subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasItem(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item {expected},
					              but it did not contain any item

					              Collection:
					              []
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				int expected = 42;
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 42,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				int expected = 42;
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(expected).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 42 at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IEnumerable subject = ToEnumerable([0, 1, 2, 3, 4,]);

				async Task Act()
					=> await That(subject).HasItem(2).WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 2 with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [
					               0,
					               1,
					               2,
					               3,
					               4
					             ]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItem("d").AtIndex(0).And.HasItem("e").AtIndex(1).And.HasItem("f")
				;

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item "d" at index 0 and has item "e" at index 1 and has item "f",
					             but it had item "a" at index 0 and it had item "b" at index 1 and it did not match at any index

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class EnumerableEquivalentTests
		{
			[Fact]
			public async Task WhenEquivalentItemIsFound_ShouldSucceed()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
				MyClass expected = new(5);

				async Task Act()
					=> await That(subject).HasItem(expected).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEquivalentItemIsNotFound_ShouldFail()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
				MyClass expected = new(4);

				async Task Act()
					=> await That(subject).HasItem(expected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item equivalent to MyClass {
					               StringValue = "",
					               Value = 4
					             },
					             but it did not match at any index

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
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
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 5
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 8
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 13
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 21
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 34
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 55
					               },
					               (… and 10 more)
					             ]
					             """);
			}
		}

		public sealed class EnumerableUsingTests
		{
			[Fact]
			public async Task WithAllDifferentComparer_ShouldFail()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).HasItem(1).Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item 1 using AllDifferentComparer,
					             but it did not match at any index

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
					               (… and 10 more)
					             ]
					             """);
			}

			[Fact]
			public async Task WithAllEqualComparer_ShouldSucceed()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).HasItem(4).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
