using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed class EnumerablePredicateTests
		{
			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).HasItem(_ => true)
						.And.HasItem(_ => true).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItem(a => 5.Equals(a));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					0, 1, 2,
				};

				async Task Act()
					=> await That(subject).HasItem(_ => false).AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item matching _ => false at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					0, 1, 2,
				};

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					0, 1, 2,
				};

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(3);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item matching _ => true at index 3,
					              but it had no item at index 3

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableContainsNullItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					null, "a",
				};

				async Task Act()
					=> await That(subject).HasItem(x => "a".Equals(x)).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching x => "a".Equals(x) at index 0,
					             but it had item <null> at index 0

					             Collection:
					             [
					               <null>,
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasItem(_ => true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching _ => true,
					             but it had no item

					             Collection:
					             []
					             """);
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasItem(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateIsNull_WhenNegated_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(_ => true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching _ => true,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(_ => true).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching _ => true at index 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IEnumerable subject = ToEnumerable([0, 1, 2, 3, 4,]);

				async Task Act()
					=> await That(subject).HasItem(_ => true).WithInvalidMatch();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching _ => true with invalid match,
					             but it had no item with invalid match

					             Collection:
					             [0, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItem(_ => false).AtIndex(0).And.HasItem(_ => false).AtIndex(1).And
						.HasItem(_ => false)
				;

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching _ => false at index 0 and has an item matching _ => false at index 1 and has an item matching _ => false,
					             but it had item "a" at index 0 and had item "b" at index 1 and had no matching item

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
			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).HasItem(1)
						.And.HasItem(1).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItem(5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail(
				List<int> values, int expected)
			{
				values.Add(0);
				values.Add(1);
				values.Insert(2, expected);
				IEnumerable subject = values;

				async Task Act()
					=> await That(subject).HasItem(expected - 1).AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item equal to {expected - 1} at index 2,
					              but it had item {expected} at index 2

					              Collection:
					              {Formatter.Format(values)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldNotReadFurther()
			{
				int readItems = 0;
				IEnumerable subject = new[]
				{
					2, 3, 4,
				}.Select(x =>
				{
					readItems++;
					return x;
				});

				async Task Act()
					=> await That(subject).HasItem(1).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 1 at index 0,
					             but it had item 2 at index 0

					             Collection:
					             [2, (… and maybe more)]
					             """);
				await That(readItems).IsEqualTo(1)
					.Because("the item at index 0 decides the outcome, so no further item must be read");
			}

			[Test]
			[AutoArguments]
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

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail(int expected)
			{
				IEnumerable subject = new[]
				{
					0, 1, expected,
				};

				async Task Act()
					=> await That(subject).HasItem(expected).AtIndex(3);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item equal to {expected} at index 3,
					              but it had no item at index 3

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenEnumerableContainsNullItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					null, "a",
				};

				async Task Act()
					=> await That(subject).HasItem("a").AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to "a" at index 0,
					             but it had item <null> at index 0

					             Collection:
					             [
					               <null>,
					               "a"
					             ]
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				IEnumerable subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).HasItem(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has an item equal to {expected},
					              but it had no item

					              Collection:
					              []
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				int expected = 42;
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 42,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				int expected = 42;
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject!).HasItem(expected).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 42 at index 0,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IEnumerable subject = ToEnumerable([0, 1, 2, 3, 4,]);

				async Task Act()
					=> await That(subject).HasItem(2).WithInvalidMatch();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 2 with invalid match,
					             but it had no item with invalid match

					             Collection:
					             [0, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IEnumerable subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItem("d").AtIndex(0).And.HasItem("e").AtIndex(1).And.HasItem("f")
				;

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to "d" at index 0 and has an item equal to "e" at index 1 and has an item equal to "f",
					             but it had item "a" at index 0 and had item "b" at index 1 and had no matching item

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
			[Test]
			public async Task WhenEquivalentItemIsFound_ShouldSucceed()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
				MyClass expected = new(5);

				async Task Act()
					=> await That(subject).HasItem(expected).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEquivalentItemIsNotFound_ShouldFail()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x));
				MyClass expected = new(4);

				async Task Act()
					=> await That(subject).HasItem(expected).Equivalent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equivalent to MyClass {
					               StringValue = "",
					               Value = 4
					             },
					             but it had no matching item

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

					             Equivalency options:
					              - include public fields and properties
					             """);
			}
		}

		public sealed class EnumerableUsingTests
		{
			[Test]
			public async Task WithAllDifferentComparer_ShouldFail()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(20);

				async Task Act()
					=> await That(subject).HasItem(1).Using(new AllDifferentComparer());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 1 using AllDifferentComparer,
					             but it had no matching item

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

			[Test]
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
