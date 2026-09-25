#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class IsInAscendingOrder
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 2, 3, 1);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had 3 before 1, which is not in ascending order

					             Collection:
					             [1, 1, 2, 3, 1]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 2, 3, 1);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder());

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

		public sealed class StringTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "a" before "A", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "A"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c", "a",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "c" before "a", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class MemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value,
					             but it had 3 before 1, which is not in ascending order

					             Collection:
					             [
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder((Func<MyIntClass, int>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder(x => x.Value));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<MyIntClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder(x => x.Value));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x.Value,
					             but it was

					             Collection:
					             [
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 3
					               }
					             ]
					             """);
			}
		}

		public sealed class StringMemberTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value,
					             but it had "a" before "A", which is not in ascending order

					             Collection:
					             [
					               ThatAsyncEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "A"
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IAsyncEnumerable<MyStringClass> subject = ToAsyncEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IAsyncEnumerable<MyStringClass> subject =
					ToAsyncEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value,
					             but it had "c" before "a", which is not in ascending order

					             Collection:
					             [
					               ThatAsyncEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatAsyncEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IAsyncEnumerable<MyStringClass>
					subject = ToAsyncEnumerable(["a", "b", "c",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
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
					=> await That(subject).IsInAscendingOrder()
						.Using(Comparer<DateTime>.Create((a, b) => a.Ticks.CompareTo(b.Ticks)));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable<DateTime>(Utc, Unspecified, Local);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenKindIsUnspecified_ShouldSucceed()
			{
				IAsyncEnumerable<DateTime> subject = ToAsyncEnumerable<DateTime>(Utc, Unspecified, Utc.AddHours(2));

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMemberKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<Item> subject = ToAsyncEnumerable(new Item(Utc), new Item(Local));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order by x => x.Value,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenNullableItemKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<DateTime?> subject = ToAsyncEnumerable<DateTime?>(null, null, Utc, Local);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenNullableMemberKindsAreIncompatible_ShouldFail()
			{
				IAsyncEnumerable<Item> subject = ToAsyncEnumerable(new Item(Utc), new Item(Local));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.NullableValue);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order by x => x.NullableValue,
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
