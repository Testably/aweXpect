namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed partial class Nullable
	{
		public sealed class HasDayOfWeek
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenDayOfWeekOfSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
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
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					DayOfWeek expected = DayOfWeek.Friday;

					async Task Act()
						=> await That(subject).HasDayOfWeek(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
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

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					DayOfWeek expected = DayOfWeek.Friday;

					async Task Act()
						=> await That(subject).HasDayOfWeek(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day of week equal to Friday,
						             but it was <null>
						             """);
				}
			}

			public sealed class EqualToTests
			{
				[Test]
				public async Task WhenDayOfWeekOfSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
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
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					DayOfWeek expected = DayOfWeek.Friday;

					async Task Act()
						=> await That(subject).HasDayOfWeek().EqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
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

				[Test]
				public async Task WhenSubjectAndExpectedAreNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					DayOfWeek? expected = null;

					async Task Act()
						=> await That(subject).HasDayOfWeek().EqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day of week equal to <null>,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					DayOfWeek? expected = DayOfWeek.Friday;

					async Task Act()
						=> await That(subject).HasDayOfWeek().EqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has day of week equal to Friday,
						             but it was <null>
						             """);
				}
			}

			public sealed class NotEqualToTests
			{
				[Test]
				public async Task WhenDayOfWeekOfSubjectIsDifferent_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
					DayOfWeek? unexpected = DayOfWeek.Monday;

					async Task Act()
						=> await That(subject).HasDayOfWeek().NotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayOfWeekOfSubjectIsTheSame_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
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
				public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					DayOfWeek? unexpected = null;

					async Task Act()
						=> await That(subject).HasDayOfWeek().NotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have day of week equal to <null>,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;
					DayOfWeek? unexpected = DayOfWeek.Friday;

					async Task Act()
						=> await That(subject).HasDayOfWeek().NotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have day of week equal to Friday,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());
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
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasDayOfWeek(DayOfWeek.Monday));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDayOfWeekMatches_ShouldFail()
				{
					DateTimeOffset? subject = 12.November(2010).At(13, 14, 15, 167).WithOffset(2.Hours());

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasDayOfWeek(DayOfWeek.Friday));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have day of week equal to Friday,
						             but it had day of week Friday
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTimeOffset? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.HasDayOfWeek(DayOfWeek.Friday));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have day of week equal to Friday,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
