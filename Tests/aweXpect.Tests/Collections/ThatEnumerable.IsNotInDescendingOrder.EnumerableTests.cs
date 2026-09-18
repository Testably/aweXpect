using System.Collections;
// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInDescendingOrder
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					3, 3, 2, 1, 3,
				};

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					3, 2, 1,
				};

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

		public sealed class EnumerableMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([3, 2, 1,], x => new MyIntClass(x));

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
	}
}
