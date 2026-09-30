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
					              but it was {Formatter.Format(subject)}, which differs by -1 day
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
			public async Task WhenTheCurrentCultureHasAnotherNegativeSign_ShouldUseTheInvariantOne()
			{
				using CultureOverride _ = new("sv-SE");
				DateOnly subject = new(2024, 1, 1);
				DateOnly expected = new(2024, 1, 3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2024-01-03,
					             but it was 2024-01-01, which differs by -2 days
					             """).Because("the day difference must not depend on the current culture");
			}

			[Fact]
			public async Task WhenTheDefaultToleranceIsAtLeastOneDay_ShouldMentionItsWholeDays()
			{
				DateOnly subject = EarlierTime(2);
				DateOnly expected = CurrentTime();

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());
					await That(subject).IsEqualTo(expected);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1 day,
					              but it was {Formatter.Format(subject)}, which differs by -2 days
					              """)
					.Because("the default is truncated to whole days and the applied part is named in the expectation");
			}

			[Fact]
			public async Task WhenTheDefaultToleranceIsBelowOneDay_ShouldNotMentionTheTolerance()
			{
				DateOnly subject = EarlierTime(2);
				DateOnly expected = CurrentTime();

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(12.Hours());
					await That(subject).IsEqualTo(expected);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by -2 days
					              """)
					.Because("a default below one day is truncated to zero days and must not read as ± 0 days");
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

				object Act()
					=> That(subject).IsEqualTo(expected)
						.Within(hours.Hours() + minutes.Minutes());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("tolerance").And
					.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
					.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
			}

			[Fact]
			public async Task Within_WhenToleranceIsOneDay_ShouldUseTheSingular()
			{
				DateOnly subject = EarlierTime(3);
				DateOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1.Days());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1 day,
					              but it was {Formatter.Format(subject)}, which differs by -3 days
					              """);
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
					              but it was {Formatter.Format(subject)}, which differs by -{actualDifference} days
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(new DateOnly(2010, 11, 13)));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(new DateOnly(2010, 11, 12)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 2010-11-12,
					             but it was 2010-11-12
					             """);
			}
		}
	}
}
#endif
