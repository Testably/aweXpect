using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInDescendingOrder
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([3, 3, 2, 1, 3,]);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([3, 2, 1,]);

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([3, 3, 2, 1, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInDescendingOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had 1 before 3 which is not in descending order

					             Collection:
					             [
					               3,
					               3,
					               2,
					               1,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([3, 2, 1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInDescendingOrder());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<string> subject = ToEnumerable(["A", "a",]);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<string> subject = ToEnumerable(["A", "a",]);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order using Ordinal*Comparer,
					             but it was

					             Collection:
					             [
					               "A",
					               "a"
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "b", "a", "c",]);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "b", "a",]);

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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class MemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 2, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x.Value,
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

		public sealed class NegatedMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInDescendingOrder(x => x.Value));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x.Value,
					             but it had 1 before 3 which is not in descending order

					             Collection:
					             [
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 2, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInDescendingOrder(x => x.Value));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class StringMemberTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["A", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["A", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x.Value using Ordinal*Comparer,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "A"
					               },
					               ThatEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               }
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["c", "b", "a", "c",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["c", "b", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x.Value,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
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

		private sealed class MyIntClass(int value)
		{
			public int Value { get; } = value;
		}
	}
}
