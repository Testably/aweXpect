using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInAscendingOrder
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInAscendingOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had 3 before 1 which is not in ascending order

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               1
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInAscendingOrder());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order using Ordinal*Comparer,
					             but it was

					             Collection:
					             [
					               "a",
					               "A"
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "a",]);

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class MemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x.Value,
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

			[Fact]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder((Func<MyIntClass, int>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInAscendingOrder(x => x.Value));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value,
					             but it had 3 before 1 which is not in ascending order

					             Collection:
					             [
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsNotInAscendingOrder.MyIntClass {
					                 Value = 1
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInAscendingOrder(x => x.Value));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class StringMemberTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.Value)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x.Value using Ordinal*Comparer,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsNotInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "A"
					               }
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x.Value,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsNotInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsNotInAscendingOrder.StringMemberTests.MyStringClass {
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

		private sealed class MyIntClass(int value)
		{
			public int Value { get; } = value;
		}
	}
}
