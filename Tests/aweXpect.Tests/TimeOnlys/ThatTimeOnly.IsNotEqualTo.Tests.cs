#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly unexpected = LaterTime();

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Within_WhenToleranceIsTwelveHours_ShouldFailForOppositeTimes()
			{
				TimeOnly subject = new(6, 0);
				TimeOnly unexpected = new(18, 0);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Within(12.Hours());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 18:00:00.0000000 ± 12:00:00,
					             but it was 06:00:00.0000000, which differs by 12:00:00
					             """)
					.Because("it must stay the exact complement of is equal to");
			}

			[Test]
			[Arguments(3, 2, false)]
			[Arguments(5, 3, false)]
			[Arguments(2, 2, true)]
			[Arguments(0, 2, true)]
			public async Task Within_WhenValuesAreInsideTheTolerance_ShouldFail(
				int actualDifference, int toleranceSeconds, bool expectToThrow)
			{
				TimeSpan tolerance = toleranceSeconds.Seconds();
				TimeOnly subject = EarlierTime(actualDifference);
				TimeOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Within(tolerance)
						.Because("we want to test the failure");

				string difference = actualDifference == 0
					? ""
					: $", which differs by -0:0{actualDifference}";

				await That(Act).Throws<FailException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)}, because we want to test the failure,
					              but it was {Formatter.Format(subject)}{difference}
					              """);
			}

			[Test]
			public async Task Within_WhenValuesWrapAroundMidnight_ShouldFail()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly unexpected = new(23, 59);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Within(1.Minutes());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 23:59:00.0000000 ± 1:00,
					             but it was 00:00:00.0000000, which differs by 1:00
					             """)
					.Because("equality uses the shortest distance around the clock face");
			}
		}
	}
}
#endif
