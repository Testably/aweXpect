#if NET8_0_OR_GREATER
using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly expected = LaterTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by -0:01
					              """);
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
			{
				TimeOnly subject = new(23, 59);
				TimeOnly expected = new(0, 1);

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Minutes());
					await That(subject).IsEqualTo(expected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 00:01:00.0000000 ± 1:00,
					             but it was 23:59:00.0000000, which differs by -2:00
					             """)
					.Because("the applied default tolerance is part of the expectation");
			}

			[Test]
			public async Task Within_WhenToleranceExceedsTwelveHours_ShouldSucceedForOppositeTimes()
			{
				TimeOnly subject = new(6, 0);
				TimeOnly expected = new(18, 0);

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(13.Hours());

				await That(Act).DoesNotThrow()
					.Because("the circular distance saturates at 12 hours, so a larger tolerance accepts every time");
			}

			[Test]
			public async Task Within_WhenToleranceIsJustBelowTwelveHours_ShouldFailForOppositeTimes()
			{
				TimeOnly subject = new(6, 0);
				TimeOnly expected = new(18, 0);

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(11.Hours() + 59.Minutes());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 18:00:00.0000000 ± 11:59:00,
					             but it was 06:00:00.0000000, which differs by 12:00:00
					             """)
					.Because("opposite times are exactly 12 hours apart on the clock face");
			}

			[Test]
			public async Task Within_WhenToleranceIsTwelveHours_ShouldSucceedForOppositeTimes()
			{
				TimeOnly subject = new(6, 0);
				TimeOnly expected = new(18, 0);

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(12.Hours());

				await That(Act).DoesNotThrow()
					.Because("12 hours is the largest possible circular distance, so it accepts every time");
			}

			[Test]
			[Arguments(3, 2, true)]
			[Arguments(5, 3, true)]
			[Arguments(2, 2, false)]
			[Arguments(0, 2, false)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int toleranceSeconds, bool expectToThrow)
			{
				TimeSpan tolerance = toleranceSeconds.Seconds();
				TimeOnly subject = EarlierTime(actualDifference);
				TimeOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(tolerance)
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)}, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -0:0{actualDifference}
					              """);
			}

			[Test]
			public async Task Within_WhenValuesWrapAroundMidnight_ShouldShowTheShorterDifference()
			{
				TimeOnly subject = new(23, 59);
				TimeOnly expected = new(0, 1);

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(1.Minutes());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 00:01:00.0000000 ± 1:00,
					             but it was 23:59:00.0000000, which differs by -2:00
					             """)
					.Because("the difference must be the circular distance that the comparison used");
			}

			[Test]
			public async Task Within_WhenValuesWrapAroundMidnight_ShouldSucceed()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly expected = new(23, 59);

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(1.Minutes());

				await That(Act).DoesNotThrow()
					.Because("equality uses the shortest distance around the clock face");
			}

			[Test]
			public async Task Within_WhenValuesWrapAroundMidnightInReverse_ShouldSucceed()
			{
				TimeOnly subject = new(23, 59);
				TimeOnly expected = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(1.Minutes());

				await That(Act).DoesNotThrow()
					.Because("the circular distance is symmetric");
			}
		}
	}
}
#endif
