#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsNotBetween
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenMaximumIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = subject;
					DateOnly? maximum = null;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
				}

				[Test]
				public async Task WhenMinimumAndMaximumAreEqual_ShouldFail()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(subject).And(subject);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(subject)} and {Formatter.Format(subject)},
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("a range with equal bounds is still a valid range");
				}

				[Test]
				public async Task WhenMinimumIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = null;
					DateOnly? maximum = subject;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between <null> and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectAndMaximumAreMaxValue_ShouldFail()
				{
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly? minimum = CurrentTime();
					DateOnly maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectAndMinimumAreMinValue_ShouldFail()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly minimum = DateOnly.MinValue;
					DateOnly? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsEarlierThanMinimum_ShouldSucceed()
				{
					DateOnly? subject = EarlierTime();
					DateOnly? minimum = CurrentTime();
					DateOnly? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsLaterThanMaximum_ShouldSucceed()
				{
					DateOnly? subject = LaterTime();
					DateOnly? minimum = EarlierTime();
					DateOnly? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNotBetweenMinimumAndMaximum_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = EarlierTime();
					DateOnly? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsSameAsMaximum_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = EarlierTime();
					DateOnly? maximum = subject;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsSameAsMinimum_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = subject;
					DateOnly? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}

			public sealed class WithinTests
			{
				[Test]
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(LaterTime()).And(EarlierTime())
							.Within(3.Days());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("a tolerance must not turn an inverted range into a satisfiable one");
				}

				[Test]
				public async Task WhenMaximumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly minimum = DateOnly.MinValue;
					DateOnly maximum = EarlierTime(4)!.Value;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly minimum = LaterTime(4)!.Value;
					DateOnly maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly minimum = DateOnly.MinValue;
					DateOnly? maximum = EarlierTime(4);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = LaterTime(4);
					DateOnly maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					object Act()
						=> That(subject).IsNotBetween(EarlierTime()).And(LaterTime())
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
				}

				[Test]
				public async Task WhenValueIsWithinTheMaximumTolerance_ShouldFail()
				{
					DateOnly? subject = LaterTime(3);
					DateOnly minimum = DateOnly.MinValue;
					DateOnly? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 3 days,
						              but it was {Formatter.Format(subject)}, which differs by 3 days from the maximum
						              """);
				}

				[Test]
				public async Task WhenValueIsWithinTheMinimumTolerance_ShouldFail()
				{
					DateOnly? subject = EarlierTime(3);
					DateOnly? minimum = CurrentTime();
					DateOnly maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 3 days,
						              but it was {Formatter.Format(subject)}, which differs by -3 days from the minimum
						              """);
				}

				[Test]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(DateOnly.MinValue).And(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Test]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(expected).And(DateOnly.MaxValue)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenSubjectIsInsideTheRange_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotBetween(new DateOnly(2010, 11, 11)).And(new DateOnly(2010, 11, 13)));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsOutsideTheRange_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotBetween(new DateOnly(2010, 11, 13)).And(new DateOnly(2010, 11, 14)));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is between 2010-11-13 and 2010-11-14,
						             but it was 2010-11-12, which differs by -1 day from the minimum
						             """);
				}
			}
		}
	}
}
#endif
