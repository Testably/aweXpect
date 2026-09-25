#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed class PredicateTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => false)
						.And.DoesNotHaveItem(_ => false).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotHaveItem(a => a == 5).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item matching _ => true at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(_ => true).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item matching _ => true at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAnyIndex_WhenAnItemMatches_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(a => a == 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item matching a => a == 1,
					             but it had item 1

					             Collection:
					             [0, 1, 2]
					             """);
			}
		}

		public sealed class ItemTests
		{
			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(2).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to 2 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(2).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItem(42).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to 42 at index 0,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringItemTests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringCase(bool ignoreCase)
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable("foo", "bar");

				async Task Act()
					=> await That(subject).DoesNotHaveItem("BAR").IgnoringCase(ignoreCase).AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(ignoreCase)
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to "BAR" ignoring case at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldSupportIgnoringIndentation(bool ignoreIndentation)
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable("a\n  b", "c\n  d");

				async Task Act()
					=> await That(subject).DoesNotHaveItem("c\nd").IgnoringIndentation(ignoreIndentation)
						.AtIndex(1);

				await That(Act).Throws<XunitException>().OnlyIf(ignoreIndentation)
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to "c\nd" ignoring indentation at index 1,
					             but it had item "c\n  d" at index 1

					             Collection:
					             [
					               "a\n  b",
					               "c\n  d"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable("foo", "bar");

				async Task Act()
					=> await That(subject).DoesNotHaveItem("foo").AtIndex(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<string?> subject = ToAsyncEnumerable("foo", "bar");

				async Task Act()
					=> await That(subject).DoesNotHaveItem("bar").AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to "bar" at index 1,
					             but it had item "bar" at index 1

					             Collection:
					             [
					               "foo",
					               "bar"
					             ]
					             """);
			}
		}

		public sealed class EquivalentTests
		{
			[Fact]
			public async Task WhenEquivalentItemIsNotFound_ShouldSucceed()
			{
				IAsyncEnumerable<MyClass> subject =
					ToAsyncEnumerable(Factory.GetFibonacciNumbers(20).Select(x => new MyClass(x)).ToArray());
				MyClass unexpected = new(4);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(unexpected).Equivalent();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class UsingTests
		{
			[Fact]
			public async Task WithAllDifferentComparer_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAllEqualComparer_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(4).Using(new AllEqualComparer()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to 4 using AllEqualComparer at index 1,
					             but it had item 2 at index 1

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class FromEndTests
		{
			[Fact]
			public async Task WhenEnumerableContainsUnexpectedItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).AtIndex(1).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to 1 at index 1 from end,
					             but it had item 1 at index 1 from end

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1).AtIndex(3).FromEnd();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
