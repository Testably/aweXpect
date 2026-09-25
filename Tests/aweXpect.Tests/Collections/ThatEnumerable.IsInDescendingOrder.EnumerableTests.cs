using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInDescendingOrder
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					3, 3, 2, 1, 3,
				};

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had 1 before 3, which is not in descending order

					             Collection:
					             [3, 3, 2, 1, 3]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					3, 2, 1,
				};

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x is MyIntClass c ? c.Value : 0,
					             but it had 1 before 3, which is not in descending order

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
				IEnumerable subject = ToEnumerable([3, 2, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
