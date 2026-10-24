#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed class HasDayOfWeek
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenDayOfWeekOfSubjectIsDifferent_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek expected = DayOfWeek.Monday;

				async Task Act()
					=> await That(subject).HasDayOfWeek(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has day of week equal to Monday,
					             but it had day of week Friday
					             """);
			}

			[Test]
			public async Task WhenDayOfWeekOfSubjectIsTheSame_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek expected = DayOfWeek.Friday;

				async Task Act()
					=> await That(subject).HasDayOfWeek(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek? expected = null;

				async Task Act()
					=> await That(subject).HasDayOfWeek(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has day of week equal to <null>,
					             but it had day of week Friday
					             """);
			}
		}

		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenDayOfWeekOfSubjectIsDifferent_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek? expected = DayOfWeek.Monday;

				async Task Act()
					=> await That(subject).HasDayOfWeek().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has day of week equal to Monday,
					             but it had day of week Friday
					             """);
			}

			[Test]
			public async Task WhenDayOfWeekOfSubjectIsTheSame_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek expected = DayOfWeek.Friday;

				async Task Act()
					=> await That(subject).HasDayOfWeek().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek? expected = null;

				async Task Act()
					=> await That(subject).HasDayOfWeek().EqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has day of week equal to <null>,
					             but it had day of week Friday
					             """);
			}
		}

		public sealed class NotEqualToTests
		{
			[Test]
			public async Task WhenDayOfWeekOfSubjectIsDifferent_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek? unexpected = DayOfWeek.Monday;

				async Task Act()
					=> await That(subject).HasDayOfWeek().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDayOfWeekOfSubjectIsTheSame_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek unexpected = DayOfWeek.Friday;

				async Task Act()
					=> await That(subject).HasDayOfWeek().NotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have day of week equal to Friday,
					             but it had day of week Friday
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);
				DayOfWeek? unexpected = null;

				async Task Act()
					=> await That(subject).HasDayOfWeek().NotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenDayOfWeekDiffers_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasDayOfWeek(DayOfWeek.Monday));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenDayOfWeekMatches_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasDayOfWeek(DayOfWeek.Friday));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have day of week equal to Friday,
					             but it had day of week Friday
					             """);
			}
		}
	}
}
#endif
