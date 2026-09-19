#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class IsNotBetween
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMaximumIsNull_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly? minimum = subject;
				TimeOnly? maximum = null;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMinimumIsNull_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly? minimum = null;
				TimeOnly? maximum = subject;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between <null> and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenRangeCrossesMidnightAndSubjectIsInside_ShouldFail()
			{
				TimeOnly subject = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsNotBetween(new TimeOnly(23, 0)).And(new TimeOnly(1, 0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not between 23:00:00.0000000 and 01:00:00.0000000,
					             but it was 00:00:00.0000000
					             """)
					.Because("a range runs clockwise from the minimum to the maximum, across midnight if needed");
			}

			[Fact]
			public async Task WhenRangeCrossesMidnightAndSubjectIsOutside_ShouldSucceed()
			{
				TimeOnly subject = new(12, 0);

				async Task Act()
					=> await That(subject).IsNotBetween(new TimeOnly(23, 0)).And(new TimeOnly(1, 0));

				await That(Act).DoesNotThrow()
					.Because("the range covers only two hours, not the rest of the day");
			}

			[Fact]
			public async Task WhenSubjectAndMaximumAreMaxValue_ShouldFail()
			{
				TimeOnly subject = TimeOnly.MaxValue;
				TimeOnly minimum = CurrentTime();
				TimeOnly maximum = TimeOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectAndMinimumAreMinValue_ShouldFail()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly minimum = TimeOnly.MinValue;
				TimeOnly maximum = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNotBetweenMinimumAndMaximum_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly minimum = EarlierTime();
				TimeOnly maximum = LaterTime();

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsEarlierThanMinimum_ShouldSucceed()
			{
				TimeOnly subject = EarlierTime();
				TimeOnly minimum = CurrentTime();
				TimeOnly maximum = LaterTime();

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLaterThanMaximum_ShouldSucceed()
			{
				TimeOnly subject = LaterTime();
				TimeOnly minimum = EarlierTime();
				TimeOnly maximum = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsSameAsMaximum_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly minimum = EarlierTime();
				TimeOnly maximum = subject;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSameAsMinimum_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly minimum = subject;
				TimeOnly maximum = LaterTime();

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenMaximumValueIsOutsideTheTolerance_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly minimum = TimeOnly.MinValue;
				TimeOnly maximum = LaterTime(2);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly minimum = EarlierTime(2);
				TimeOnly maximum = TimeOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly minimum = TimeOnly.MinValue;
				TimeOnly? maximum = LaterTime(2);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly? minimum = EarlierTime(2);
				TimeOnly maximum = TimeOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenToleranceNarrowsTheRangeToNothing_ShouldSucceed()
			{
				TimeOnly subject = new(12, 0);
				TimeOnly minimum = TimeOnly.MinValue;
				TimeOnly maximum = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum)
						.Within(12.Hours());

				await That(Act).DoesNotThrow()
					.Because("the tolerance narrows the range here, so nothing is left to be between");
			}

			[Fact]
			public async Task WhenValueIsWithinTheMaximumTolerance_ShouldFail()
			{
				TimeOnly subject = EarlierTime(3);
				TimeOnly minimum = TimeOnly.MinValue;
				TimeOnly maximum = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenValueIsWithinTheMinimumTolerance_ShouldFail()
			{
				TimeOnly subject = LaterTime(3);
				TimeOnly minimum = CurrentTime();
				TimeOnly maximum = TimeOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
#endif
