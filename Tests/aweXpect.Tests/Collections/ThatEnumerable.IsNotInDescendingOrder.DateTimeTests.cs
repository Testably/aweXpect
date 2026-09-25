using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInDescendingOrder
	{
		public sealed class DateTimeTests
		{
			private static readonly DateTime Utc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			private static readonly DateTime Unspecified = new(2026, 1, 1, 1, 0, 0, DateTimeKind.Unspecified);
			private static readonly DateTime Local = new(2026, 1, 1, 2, 0, 0, DateTimeKind.Local);

			[Fact]
			public async Task WhenCustomComparerIsUsed_ShouldNotCheckKinds()
			{
				IEnumerable<DateTime> subject = [Utc, Local,];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder()
						.Using(Comparer<DateTime>.Create((a, b) => a.Ticks.CompareTo(b.Ticks)));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<DateTime> subject = [Unspecified, Utc, Local,];

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
				IEnumerable<DateTime> subject = [Utc, Unspecified, Utc.AddHours(2),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMemberKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<Item> subject = [new(Utc), new(Local),];

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
				IEnumerable<DateTime?> subject = [Utc, Local, null,];

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
				IEnumerable<Item> subject = [new(Utc), new(Local),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.NullableValue);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in descending order by x => x.NullableValue,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenImmutableArrayKindsAreIncompatible_ShouldFail()
			{
				ImmutableArray<DateTime> subject = [Utc, Local,];

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
			public async Task WhenImmutableArrayMemberKindsAreIncompatible_ShouldFail()
			{
				ImmutableArray<Item> subject = [new(Utc), new(Local),];

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder(x => x.NullableValue);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in descending order by x => x.NullableValue,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}
#endif

			private sealed class Item(DateTime value)
			{
				public DateTime Value { get; } = value;
				public DateTime? NullableValue { get; } = value;
			}
		}
	}
}
