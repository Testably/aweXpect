using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInAscendingOrder
	{
		public sealed class DateTimeTests
		{
			private static readonly DateTime Utc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			private static readonly DateTime Unspecified = new(2026, 1, 1, 1, 0, 0, DateTimeKind.Unspecified);
			private static readonly DateTime Local = new(2026, 1, 1, 2, 0, 0, DateTimeKind.Local);

			[Fact]
			public async Task WhenCustomComparerIsUsed_ShouldNotCheckKinds()
			{
				IEnumerable<DateTime> subject = [Local, Utc,];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder()
						.Using(Comparer<DateTime>.Create((a, b) => a.Ticks.CompareTo(b.Ticks)));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<DateTime> subject = [Unspecified, Local, Utc,];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order,
					              but it had {Formatter.Format(Local)} with kind Local and {Formatter.Format(Utc)} with kind Utc, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenKindIsUnspecified_ShouldSucceed()
			{
				IEnumerable<DateTime> subject = [Utc.AddHours(2), Unspecified, Utc,];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMemberKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<Item> subject = [new(Local), new(Utc),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order by x => x.Value,
					              but it had {Formatter.Format(Local)} with kind Local and {Formatter.Format(Utc)} with kind Utc, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenNullableItemKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<DateTime?> subject = [Local, Utc, null,];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order,
					              but it had {Formatter.Format(Local)} with kind Local and {Formatter.Format(Utc)} with kind Utc, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenNullableMemberKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<Item> subject = [new(Local), new(Utc),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.NullableValue);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order by x => x.NullableValue,
					              but it had {Formatter.Format(Local)} with kind Local and {Formatter.Format(Utc)} with kind Utc, which cannot be compared
					              """).AsPrefix();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WhenImmutableArrayKindsAreIncompatible_ShouldFail()
			{
				ImmutableArray<DateTime> subject = [Local, Utc,];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order,
					              but it had {Formatter.Format(Local)} with kind Local and {Formatter.Format(Utc)} with kind Utc, which cannot be compared
					              """).AsPrefix();
			}

			[Fact]
			public async Task WhenImmutableArrayMemberKindsAreIncompatible_ShouldFail()
			{
				ImmutableArray<Item> subject = [new(Local), new(Utc),];

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder(x => x.NullableValue);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order by x => x.NullableValue,
					              but it had {Formatter.Format(Local)} with kind Local and {Formatter.Format(Utc)} with kind Utc, which cannot be compared
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
