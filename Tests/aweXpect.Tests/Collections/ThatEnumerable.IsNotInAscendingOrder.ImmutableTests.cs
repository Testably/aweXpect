#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInAscendingOrder
	{
		public sealed class ImmutableArrayTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 1, 2, 3, 1,];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order,
					             but it was

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayStringTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				ImmutableArray<string> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<string> subject = ["a", "A",];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order using OrdinalIgnoreCaseComparer,
					             but it was

					             Collection:
					             [
					               "a",
					               "A"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["a", "b", "c", "a",];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<string> subject = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order,
					             but it was

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ImmutableArrayMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyIntClass> subject = [..ToEnumerable([1, 2, 3,], x => new MyIntClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x is MyIntClass c ? c.Value : 0,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 3
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
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "A",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "A",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x is MyStringClass c ? c.Value : "")
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x is MyStringClass c ? c.Value : "" using OrdinalIgnoreCaseComparer,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsNotInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "A"
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				ImmutableArray<MyStringClass> subject =
					[..ToEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				ImmutableArray<MyStringClass> subject = [..ToEnumerable(["a", "b", "c",], x => new MyStringClass(x)),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x is MyStringClass c ? c.Value : "");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x is MyStringClass c ? c.Value : "",
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsNotInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsNotInAscendingOrder.ImmutableArrayStringMemberTests.MyStringClass {
					                 Value = "c"
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
