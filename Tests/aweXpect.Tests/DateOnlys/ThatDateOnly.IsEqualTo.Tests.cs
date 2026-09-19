#if NET8_0_OR_GREATER
using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly expected = LaterTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				DateOnly subject = CurrentTime();
				DateOnly expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTheDefaultToleranceIsNotWholeDays_ShouldStillTruncateIt()
			{
				DateOnly subject = LaterTime();
				DateOnly expected = CurrentTime();

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());
					await That(subject).IsEqualTo(expected);
				}

				await That(Act).DoesNotThrow()
					.Because("the global default is shared with all the other time types and must not turn every "
					         + "date expectation into an error");
			}

			[Theory]
			[InlineData(0)]
			[InlineData(24)]
			[InlineData(48)]
			public async Task Within_WhenToleranceIsAWholeNumberOfDays_ShouldBeAccepted(int hours)
			{
				DateOnly subject = LaterTime(hours / 24);
				DateOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(hours.Hours());

				await That(Act).DoesNotThrow()
					.Because("a tolerance that divides into whole days is one a date can honour exactly");
			}

			[Theory]
			[InlineData(23, 0)]
			[InlineData(36, 0)]
			[InlineData(47, 0)]
			[InlineData(0, 30)]
			[InlineData(24, 1)]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException(
				int hours, int minutes)
			{
				DateOnly subject = LaterTime();
				DateOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(hours.Hours() + minutes.Minutes());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("tolerance").And
					.WithMessage("Tolerance must be a whole number of days").AsPrefix()
					.Because("a date has no time of day, so the remainder would be dropped without notice");
			}

			[Theory]
			[InlineData(3, 2, true)]
			[InlineData(5, 3, true)]
			[InlineData(2, 2, false)]
			[InlineData(0, 2, false)]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
				int actualDifference, int tolerance, bool expectToThrow)
			{
				DateOnly subject = EarlierTime(actualDifference);
				DateOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Within(tolerance.Days())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.OnlyIf(expectToThrow)
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± {tolerance} days, because we want to test the failure,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
#endif
