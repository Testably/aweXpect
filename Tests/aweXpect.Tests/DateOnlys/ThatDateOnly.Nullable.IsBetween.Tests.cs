#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsBetween
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenMaximumIsNull_AndNegated_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = subject;
					DateOnly? maximum = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsBetween(minimum).And(maximum));

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and <null>,
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("nothing can be ordered against a null bound, so the negation fails as well");
				}

				[Test]
				public async Task WhenMaximumIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = subject;
					DateOnly? maximum = null;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
				}

				[Test]
				public async Task WhenMinimumAndMaximumAreEqual_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(subject).And(subject);

					await That(Act).DoesNotThrow()
						.Because("a range with equal bounds is still a valid range");
				}

				[Test]
				public async Task WhenMinimumIsNull_AndNegated_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = null;
					DateOnly? maximum = subject;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsBetween(minimum).And(maximum));

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between <null> and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("nothing can be ordered against a null bound, so the negation fails as well");
				}

				[Test]
				public async Task WhenMinimumIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = null;
					DateOnly? maximum = subject;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between <null> and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectAndMaximumAreMaxValue_ShouldSucceed()
				{
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly? minimum = CurrentTime();
					DateOnly? maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectAndMinimumAreMinValue_ShouldSucceed()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly? minimum = DateOnly.MinValue;
					DateOnly? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsBetweenMinimumAndMaximum_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = EarlierTime();
					DateOnly? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsEarlierThanMinimum_ShouldFail()
				{
					DateOnly? subject = EarlierTime();
					DateOnly? minimum = CurrentTime();
					DateOnly? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}, which differs by -1 day from the minimum
						              """);
				}

				[Test]
				public async Task WhenSubjectIsLaterThanMaximum_ShouldFail()
				{
					DateOnly? subject = LaterTime();
					DateOnly? minimum = EarlierTime();
					DateOnly? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}, which differs by 1 day from the maximum
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNullAndMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = null;

					async Task Act()
						=> await That(subject).IsBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("the range is rejected for its own sake, independently of the subject");
				}

				[Test]
				public async Task WhenSubjectIsSameAsMaximum_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = EarlierTime();
					DateOnly? maximum = subject;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsSameAsMinimum_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = subject;
					DateOnly? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class WithinTests
			{
				[Test]
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(LaterTime()).And(EarlierTime())
							.Within(3.Days());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("a tolerance must not turn an inverted range into a satisfiable one");
				}

				[Test]
				public async Task WhenMaximumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = LaterTime(4);
					DateOnly minimum = DateOnly.MinValue;
					DateOnly? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 3 days,
						              but it was {Formatter.Format(subject)}, which differs by 4 days from the maximum
						              """);
				}

				[Test]
				public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = EarlierTime(4);
					DateOnly? minimum = CurrentTime();
					DateOnly maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 3 days,
						              but it was {Formatter.Format(subject)}, which differs by -4 days from the minimum
						              """);
				}

				[Test]
				public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly minimum = DateOnly.MinValue;
					DateOnly? maximum = LaterTime(-4);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 3 days,
						              but it was {Formatter.Format(subject)}, which differs by 4 days from the maximum
						              """);
				}

				[Test]
				public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? minimum = EarlierTime(-4);
					DateOnly maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 3 days,
						              but it was {Formatter.Format(subject)}, which differs by -4 days from the minimum
						              """);
				}

				[Test]
				public async Task WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					object Act()
						=> That(subject).IsBetween(EarlierTime()).And(LaterTime())
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
				}

				[Test]
				public async Task WhenValueIsWithinTheMaximumTolerance_ShouldSucceed()
				{
					DateOnly? subject = LaterTime(3);
					DateOnly minimum = DateOnly.MinValue;
					DateOnly? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenValueIsWithinTheMinimumTolerance_ShouldSucceed()
				{
					DateOnly? subject = EarlierTime(3);
					DateOnly? minimum = CurrentTime();
					DateOnly maximum = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(DateOnly.MinValue).And(DateOnly.MaxValue)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Test]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MinValue;

					async Task Act()
						=> await That(subject).IsBetween(DateOnly.MinValue).And(DateOnly.MaxValue)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}
			}
		}
	}
}
#endif
