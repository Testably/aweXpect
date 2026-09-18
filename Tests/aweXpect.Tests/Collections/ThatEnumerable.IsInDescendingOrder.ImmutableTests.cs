#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInDescendingOrder
	{
		public sealed class ImmutableArrayTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<int> subject = [3, 3, 2, 1, 3,];

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had 1 before 3 which is not in descending order

					             Collection:
					             [3, 3, 2, 1, 3]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<int> subject = [3, 2, 1,];

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableArrayStringTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<string> subject = ["A", "a",];

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had "A" before "a" which is not in descending order

					             Collection:
					             [
					               "A",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<string> subject = ["A", "a",];

				async Task Act()
					=> await That(subject).IsInDescendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<string> subject = ["c", "b", "a", "c",];

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had "a" before "c" which is not in descending order

					             Collection:
					             [
					               "c",
					               "b",
					               "a",
					               "c"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["c", "b", "a",];

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableArrayMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x is MyIntClass c ? c.Value : 0,
					             but it had 1 before 3 which is not in descending order

					             Collection:
					             [
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 3
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([3, 2, 1,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ImmutableArrayStringMemberTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["A", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x is MyStringClass c ? c.Value : "",
					             but it had "A" before "a" which is not in descending order

					             Collection:
					             [
					               ThatEnumerable.IsInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "A"
					               },
					               ThatEnumerable.IsInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["A", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyStringClass c ? c.Value : "")
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyStringClass> subject =
					[..ToEnumerable(["c", "b", "a", "c",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x is MyStringClass c ? c.Value : "",
					             but it had "a" before "c" which is not in descending order

					             Collection:
					             [
					               ThatEnumerable.IsInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.IsInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "c"
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["c", "b", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyStringClass c ? c.Value : "");

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
