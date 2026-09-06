#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class Satisfy
		{
			public sealed class ItemsTests
			{
				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().Satisfy(x => x > 0)
							.And.All().Satisfy(x => x < 2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeAsyncEnumerable()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(x => x <= 1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x <= 1 for all items,
						             but not all did

						             Not matching items:
						             [2, 3, 5, 8, 13, 21, 34, 55, 89, (… and maybe others)]

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
						               (… and maybe others)
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x is > 4 and < 6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x is > 4 and < 6 for all items,
						             but not all did

						             Not matching items:
						             [
						               1,
						               1,
						               2,
						               3,
						               8,
						               13,
						               21,
						               34,
						               55,
						               89,
						               (… and maybe others)
						             ]

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
						               (… and maybe others)
						             ]
						             """);
				}

				[Fact]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IAsyncEnumerable<int> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == constantValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The predicate cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().Satisfy(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies _ => true for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.StartsWith("ba") == true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x?.StartsWith("ba") == true for all items,
						             but only 2 of 3 did

						             Not matching items:
						             [
						               "foo"
						             ]

						             Collection:
						             [
						               "foo",
						               "bar",
						               "baz"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable((string[]) []);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.Length == 3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The predicate cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == "" for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedItemsTests
			{
				[Fact]
				public async Task WhenItemsDiffer_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x is > 4 and < 6));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNoItemsDiffer_ShouldFail()
				{
					int constantValue = 42;
					IAsyncEnumerable<int> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == constantValue));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not satisfy x => x == constantValue for all items,
						             but all 20 did

						             Collection:
						             [
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               …
						             ]
						             """);
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(null!));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The predicate cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(_ => true));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not satisfy _ => true for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedStringTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x?.StartsWith("ba") == true));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable((string[]) []);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == ""));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not satisfy x => x == "" for all items,
						             but all 0 did

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x?.Length == 3));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not satisfy x => x?.Length == 3 for all items,
						             but all 3 did

						             Collection:
						             [
						               "foo",
						               "bar",
						               "baz"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(null!));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The predicate cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == ""));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not satisfy x => x == "" for all items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
#endif
