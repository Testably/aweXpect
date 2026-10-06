using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInDescendingOrder
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([3, 3, 2, 1, 3,]);

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had 1 before 3, which is not in descending order

					             Collection:
					             [3, 3, 2, 1, 3, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([3, 2, 1,]);

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([3, 3, 2, 1, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInDescendingOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([3, 2, 1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInDescendingOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order,
					             but it was

					             Collection:
					             [3, 2, 1]
					             """);
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<string> subject = ToEnumerable(["A", "a",]);

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had "A" before "a", which is not in descending order

					             Collection:
					             [
					               "A",
					               "a",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<string> subject = ToEnumerable(["A", "a",]);

				async Task Act()
					=> await That(subject).IsInDescendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "b", "a", "c",]);

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it had "a" before "c", which is not in descending order

					             Collection:
					             [
					               "c",
					               "b",
					               "a",
					               "c",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["c", "b", "a",]);

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class MemberTests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x.Value,
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
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 2, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 2, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder((Func<MyIntClass, int>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedMemberTests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInDescendingOrder(x => x.Value));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([3, 2, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInDescendingOrder(x => x.Value));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x.Value,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsInDescendingOrder.MyIntClass {
					                 Value = 1
					               }
					             ]
					             """);
			}
		}

		public sealed class StringMemberTests
		{
			[Test]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["A", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x.Value,
					             but it had "A" before "a", which is not in descending order

					             Collection:
					             [
					               ThatEnumerable.IsInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "A"
					               },
					               ThatEnumerable.IsInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["A", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x.Value)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["c", "b", "a", "c",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x.Value,
					             but it had "a" before "c", which is not in descending order

					             Collection:
					             [
					               ThatEnumerable.IsInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.IsInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["c", "b", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
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
