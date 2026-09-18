#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInDescendingOrder
	{
		public sealed class ImmutableArrayTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<int> subject = [3, 3, 2, 1, 3,];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<int> subject = [3, 2, 1,];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order,
					             but it was

					             Collection:
					             [3, 2, 1]
					             """);
			}
		}

		public sealed class ImmutableArrayStringTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<string> subject = ["A", "a",];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<string> subject = ["A", "a",];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order using OrdinalIgnoreCaseComparer,
					             but it was

					             Collection:
					             [
					               "A",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["c", "b", "a", "c",];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<string> subject = ["c", "b", "a",];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order,
					             but it was

					             Collection:
					             [
					               "c",
					               "b",
					               "a"
					             ]
					             """);
			}
		}

		public sealed class ImmutableArrayMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([3, 2, 1,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x is MyIntClass c ? c.Value : 0,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 1
					               }
					             ]
					             """);
			}
		}

		public sealed class ImmutableArrayStringMemberTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["A", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["A", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x is MyStringClass c ? c.Value : "")
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x is MyStringClass c ? c.Value : "" using OrdinalIgnoreCaseComparer,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "A"
					               },
					               ThatEnumerable.IsNotInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyStringClass> subject =
					[..ToEnumerable(["c", "b", "a", "c",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["c", "b", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x is MyStringClass c ? c.Value : "",
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.IsNotInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsNotInDescendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               }
					             ]
					             """);
			}

			private sealed class MyStringClass(string value)
			{
				public string Value { get; } = value;
			}
		}
	}
}
#endif
