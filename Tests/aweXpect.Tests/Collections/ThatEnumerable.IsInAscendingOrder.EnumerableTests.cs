using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInAscendingOrder
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 1, 2, 3, 1,
				};

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
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

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
				IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x is MyIntClass c ? c.Value : 0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder((Func<object?, int>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}
		}
	}
}
