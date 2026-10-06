#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInAscendingOrder
	{
		public sealed class ImmutableArrayTests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 1, 2, 3, 1,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had 3 before 1, which is not in ascending order

					             Collection:
					             [1, 1, 2, 3, 1]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullableItemIsNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<int?> subject = [2, null, 3,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had 2 before <null>, which is not in ascending order

					             Collection:
					             [2, <null>, 3]
					             """)
					.Because("a null item sorts before any other item and must not be skipped");
			}
		}

		public sealed class ImmutableArrayStringTests
		{
			[Test]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<string> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "a" before "A", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "A"
					             ]
					             """);
			}

			[Test]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<string> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<string> subject = ["a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "c" before "a", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "a"
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNullItemIsNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", null, "b",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "a" before <null>, which is not in ascending order

					             Collection:
					             [
					               "a",
					               <null>,
					               "b"
					             ]
					             """)
					.Because("a null item sorts before any other item and must not be skipped");
			}
		}

		public sealed class ImmutableArrayMemberTests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x is MyIntClass c ? c.Value : 0,
					             but it had 3 before 1, which is not in ascending order

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               }
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([1, 2, 3,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([1, 2, 3,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder((Func<MyIntClass, int>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}
		}

		public sealed class ImmutableArrayStringMemberTests
		{
			[Test]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "A",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x is MyStringClass c ? c.Value : "",
					             but it had "a" before "A", which is not in ascending order

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "A"
					               }
					             ]
					             """);
			}

			[Test]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "A",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyStringClass c ? c.Value : "")
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyStringClass> subject =
					[..ToEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x is MyStringClass c ? c.Value : "",
					             but it had "c" before "a", which is not in ascending order

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.IsInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               }
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "b", "c",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			private sealed class MyStringClass(string value)
			{
				public string Value { get; } = value;
			}
		}
	}
}
#endif
