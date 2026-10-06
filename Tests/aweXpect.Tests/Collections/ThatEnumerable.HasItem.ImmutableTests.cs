#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed class ImmutablePredicateTests
		{
			[Test]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

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
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItem(_ => true).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

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
				ImmutableArray<string?> subject = [null, "a",];

				async Task Act()
					=> await That(subject).HasItem(x => x == "a").AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching x => x == "a" at index 0,
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
				ImmutableArray<int> subject = [];

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
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItem(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateIsNull_WhenNegated_ShouldThrowArgumentNullException()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItem(predicate: null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				ImmutableArray<int> subject = [0, 1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).HasItem(_ => true).WithInvalidMatch();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item matching _ => true with invalid match,
					             but it had no item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Test]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				ImmutableArray<string> subject = ["a", "b", "c",];

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

		public sealed class ImmutableItemTests
		{
			[Test]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).HasItem(3).AtIndex(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 3 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2, 3, 4, 5]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).HasItem(2).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItem(2).AtIndex(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 2 at index 3,
					             but it had no item at index 3

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsNullItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int?> subject = [null, 1,];

				async Task Act()
					=> await That(subject).HasItem(1).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 1 at index 0,
					             but it had item <null> at index 0

					             Collection:
					             [<null>, 1]
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				ImmutableArray<int> subject = [];

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
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				ImmutableArray<int> subject = [0, 1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).HasItem(2).WithInvalidMatch();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 2 with invalid match,
					             but it had no item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Test]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				ImmutableArray<string?> subject = ["a", "b", "c",];

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

		public sealed class ImmutableEquivalentTests
		{
			[Test]
			public async Task WhenEquivalentItemIsFound_ShouldSucceed()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)),];
				MyClass expected = new(5);

				async Task Act()
					=> await That(subject).HasItem(expected).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEquivalentItemIsNotFound_ShouldFail()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)),];
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

		public sealed class ImmutableUsingTests
		{
			[Test]
			public async Task WithAllDifferentComparer_ShouldFail()
			{
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

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
				ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

				async Task Act()
					=> await That(subject).HasItem(4).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
