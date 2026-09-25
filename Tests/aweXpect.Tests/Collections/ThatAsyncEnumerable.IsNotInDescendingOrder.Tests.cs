#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class IsNotInDescendingOrder
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(3, 3, 2, 1, 3);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(3, 2, 1);

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
				IAsyncEnumerable<int>? subject = null;

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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(3, 3, 2, 1, 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInDescendingOrder());

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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(3, 2, 1);

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["A", "a",]);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["A", "a",]);

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
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "b", "a", "c",]);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["c", "b", "a",]);

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
				IAsyncEnumerable<string>? subject = null;

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
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([3, 2, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x.Value,
					             but it was

					             Collection:
					             [
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
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
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([3, 3, 2, 1, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInDescendingOrder(x => x.Value));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order by x => x.Value,
					             but it had 1 before 3, which is not in descending order

					             Collection:
					             [
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.MyIntClass {
					                 Value = 3
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([3, 2, 1,], x => new MyIntClass(x));

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
				IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["A", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["A", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x.Value using OrdinalIgnoreCaseComparer,
					             but it was

					             Collection:
					             [
					               ThatAsyncEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "A"
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<MyStringClass> subject =
					ToAsyncEnumerable(["c", "b", "a", "c",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<MyStringClass>
					subject = ToAsyncEnumerable(["c", "b", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order by x => x.Value,
					             but it was

					             Collection:
					             [
					               ThatAsyncEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatAsyncEnumerable.IsNotInDescendingOrder.StringMemberTests.MyStringClass {
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

		public sealed class DateTimeTests
		{
			private static readonly DateTime Utc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			private static readonly DateTime Unspecified = new(2026, 1, 1, 1, 0, 0, DateTimeKind.Unspecified);
			private static readonly DateTime Local = new(2026, 1, 1, 2, 0, 0, DateTimeKind.Local);

			[Fact]
			public async Task WhenCustomComparerIsUsed_ShouldNotCheckKinds()
			{
				IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable<DateTime>(Utc, Local);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder()
						.Using(Comparer<DateTime>.Create((a, b) => a.Ticks.CompareTo(b.Ticks)));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable<DateTime>(Unspecified, Utc, Local);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in descending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenKindIsUnspecified_ShouldSucceed()
			{
				IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable<DateTime>(Utc, Unspecified, Utc.AddHours(2));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMemberKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<Item> subject = ToAsyncEnumerable(new Item(Utc), new Item(Local));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in descending order by x => x.Value,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenNullableItemKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<DateTime?> subject = ToAsyncEnumerable<DateTime?>(Utc, Local, null);

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in descending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenNullableMemberKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<Item> subject = ToAsyncEnumerable(new Item(Utc), new Item(Local));

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.NullableValue);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in descending order by x => x.NullableValue,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			private sealed class Item(DateTime value)
			{
				public DateTime Value { get; } = value;
				public DateTime? NullableValue { get; } = value;
			}
		}

		private sealed class MyIntClass(int value)
		{
			public int Value { get; } = value;
		}
	}
}
#endif
