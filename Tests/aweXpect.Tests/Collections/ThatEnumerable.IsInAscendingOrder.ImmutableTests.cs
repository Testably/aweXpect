#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInAscendingOrder
	{
		public sealed class ImmutableArrayTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 1, 2, 3, 1,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had 3 before 1 which is not in ascending order

					             Collection:
					             [1, 1, 2, 3, 1]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableArrayStringTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<string> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "a" before "A" which is not in ascending order

					             Collection:
					             [
					               "a",
					               "A"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<string> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<string> subject = ["a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "c" before "a" which is not in ascending order

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableArrayMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x is MyIntClass c ? c.Value : 0,
					             but it had 3 before 1 which is not in ascending order

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

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([1, 2, 3,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableArrayStringMemberTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "A",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x is MyStringClass c ? c.Value : "",
					             but it had "a" before "A" which is not in ascending order

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

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "A",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyStringClass c ? c.Value : "")
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyStringClass> subject =
					[..ToEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x is MyStringClass c ? c.Value : "",
					             but it had "c" before "a" which is not in ascending order

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

			[Fact]
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
