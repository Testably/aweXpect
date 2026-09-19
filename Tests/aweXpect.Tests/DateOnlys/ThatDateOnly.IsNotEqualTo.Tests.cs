#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				DateOnly subject = CurrentTime();
				DateOnly unexpected = LaterTime();

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				DateOnly subject = CurrentTime();
				DateOnly? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
			{
				DateOnly subject = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotEqualTo(LaterTime())
						.Within(23.Hours());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("tolerance").And
					.WithMessage("Tolerance must be a whole number of days").AsPrefix()
					.Because("a date has no time of day, so the remainder would be dropped without notice");
			}

			[Theory]
			[InlineData(3, 2, false)]
			[InlineData(5, 3, false)]
			[InlineData(2, 2, true)]
			[InlineData(0, 2, true)]
			public async Task Within_WhenValuesAreInsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				DateOnly subject = EarlierTime(actualDifference);
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Within(tolerance.Days())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± {tolerance} days, because we want to test the failure,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
#endif
