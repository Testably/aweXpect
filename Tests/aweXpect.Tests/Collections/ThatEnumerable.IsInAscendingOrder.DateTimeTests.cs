using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInAscendingOrder
	{
		public sealed class DateTimeTests
		{
			private static readonly DateTime Utc = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			private static readonly DateTime Unspecified = new(2026, 1, 1, 1, 0, 0, DateTimeKind.Unspecified);
			private static readonly DateTime Local = new(2026, 1, 1, 2, 0, 0, DateTimeKind.Local);

			[Test]
			public async Task WhenCustomComparerIsUsed_ShouldNotCheckKinds()
			{
				IEnumerable<DateTime> subject = [Utc, Local,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder()
						.Using(Comparer<DateTime>.Create((a, b) => a.Ticks.CompareTo(b.Ticks)));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<DateTime> subject = [Utc, Unspecified, Local,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Test]
			public async Task WhenKindsAreIncompatibleAndNegated_ShouldFail()
			{
				IEnumerable<DateTime> subject = [Utc, Local,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsInAscendingOrder());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix()
					.Because("the order of incompatible kinds cannot be verified, which also fails the negation");
			}

			[Test]
			public async Task WhenKindIsUnspecified_ShouldSucceed()
			{
				IEnumerable<DateTime> subject = [Utc, Unspecified, Utc.AddHours(2),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<Item> subject = [new(Utc), new(Local),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenMemberKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<Item> subject = [new(Utc), new(Local),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order by x => x.Value,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Test]
			public async Task WhenNullableItemKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<DateTime?> subject = [null, null, Utc, Local,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Test]
			public async Task WhenNullableMemberKindsAreIncompatible_ShouldFail()
			{
				IEnumerable<Item> subject = [new(Utc), new(Local),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.NullableValue);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order by x => x.NullableValue,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Test]
			public async Task WhenRetriedAfterIncompatibleKinds_ShouldJudgeTheNextAttemptOnItsOwn()
			{
				int attempts = 0;

				DateTime[] GetSubject()
					=> attempts++ == 0 ? [Utc, Local,] : [Utc, Utc.AddHours(1),];

				async Task Act()
					=> await That(GetSubject).Eventually().CheckEvery(1.Milliseconds()).IsInAscendingOrder();

				await That(Act).DoesNotThrow()
					.Because("the second attempt only contains UTC times in ascending order");
			}

			private sealed class Item(DateTime value)
			{
				public DateTime Value { get; } = value;
				public DateTime? NullableValue { get; } = value;
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenImmutableArrayIsRetriedAfterIncompatibleKinds_ShouldJudgeTheNextAttemptOnItsOwn()
			{
				int attempts = 0;

				ImmutableArray<DateTime> GetSubject()
					=> attempts++ == 0 ? [Utc, Local,] : [Utc, Utc.AddHours(1),];

				async Task Act()
					=> await That(GetSubject).Eventually().CheckEvery(1.Milliseconds()).IsInAscendingOrder();

				await That(Act).DoesNotThrow()
					.Because("the second attempt only contains UTC times in ascending order");
			}

			[Test]
			public async Task WhenImmutableArrayKindsAreIncompatible_ShouldFail()
			{
				ImmutableArray<DateTime> subject = [Utc, Local,];

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}

			[Test]
			public async Task WhenImmutableArrayKindsAreIncompatibleAndNegated_ShouldFail()
			{
				ImmutableArray<DateTime> subject = [Utc, Local,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsInAscendingOrder());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not in ascending order,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix()
					.Because("the order of incompatible kinds cannot be verified, which also fails the negation");
			}

			[Test]
			public async Task WhenImmutableArrayMemberKindsAreIncompatible_ShouldFail()
			{
				ImmutableArray<Item> subject = [new(Utc), new(Local),];

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.NullableValue);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is in ascending order by x => x.NullableValue,
					              but it had {Formatter.Format(Utc)} with kind Utc and {Formatter.Format(Local)} with kind Local, which cannot be compared
					              """).AsPrefix();
			}
#endif
		}
	}
}
