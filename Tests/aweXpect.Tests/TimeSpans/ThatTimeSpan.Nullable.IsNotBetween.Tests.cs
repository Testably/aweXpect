namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
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
					TimeSpan? subject = CurrentTime();
					TimeSpan? minimum = subject;
					TimeSpan? maximum = null;

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
					TimeSpan? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
				}

				[Test]
				public async Task WhenMinimumAndMaximumAreEqual_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();

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
					TimeSpan? subject = CurrentTime();
					TimeSpan? minimum = null;
					TimeSpan? maximum = subject;

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
					TimeSpan? subject = TimeSpan.MaxValue;
					TimeSpan? minimum = CurrentTime();
					TimeSpan maximum = TimeSpan.MaxValue;

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
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan minimum = TimeSpan.MinValue;
					TimeSpan? maximum = CurrentTime();

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
					TimeSpan? subject = EarlierTime();
					TimeSpan? minimum = CurrentTime();
					TimeSpan? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsLaterThanMaximum_ShouldSucceed()
				{
					TimeSpan? subject = LaterTime();
					TimeSpan? minimum = EarlierTime();
					TimeSpan? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNotBetweenMinimumAndMaximum_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? minimum = EarlierTime();
					TimeSpan? maximum = LaterTime();

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
					TimeSpan? subject = CurrentTime();
					TimeSpan? minimum = EarlierTime();
					TimeSpan? maximum = subject;

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
					TimeSpan? subject = CurrentTime();
					TimeSpan? minimum = subject;
					TimeSpan? maximum = LaterTime();

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
					TimeSpan? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(LaterTime()).And(EarlierTime())
							.Within(3.Seconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("a tolerance must not turn an inverted range into a satisfiable one");
				}

				[Test]
				public async Task WhenMaximumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan minimum = TimeSpan.MinValue;
					TimeSpan maximum = EarlierTime(4)!.Value;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan minimum = LaterTime(4)!.Value;
					TimeSpan maximum = TimeSpan.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan minimum = TimeSpan.MinValue;
					TimeSpan? maximum = EarlierTime(4);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? minimum = LaterTime(4);
					TimeSpan maximum = TimeSpan.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenValueIsWithinTheMaximumTolerance_ShouldFail()
				{
					TimeSpan? subject = LaterTime(3);
					TimeSpan minimum = TimeSpan.MinValue;
					TimeSpan? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
						              but it was {Formatter.Format(subject)}, which differs by 0:03 from the maximum
						              """);
				}

				[Test]
				public async Task WhenValueIsWithinTheMinimumTolerance_ShouldFail()
				{
					TimeSpan? subject = EarlierTime(3);
					TimeSpan? minimum = CurrentTime();
					TimeSpan maximum = TimeSpan.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
						              but it was {Formatter.Format(subject)}, which differs by -0:03 from the minimum
						              """);
				}

				[Test]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					TimeSpan? subject = TimeSpan.MaxValue;
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(TimeSpan.MinValue).And(expected)
							.Within(1.Seconds());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Test]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(expected).And(TimeSpan.MaxValue)
							.Within(1.Seconds());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenSubjectIsInsideTheRange_ShouldSucceed()
				{
					TimeSpan? subject = 5.Seconds();

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotBetween(4.Seconds()).And(6.Seconds()));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsOutsideTheRange_ShouldFail()
				{
					TimeSpan? subject = 5.Seconds();

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotBetween(6.Seconds()).And(7.Seconds()));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is between 0:06 and 0:07,
						             but it was 0:05, which differs by -0:01 from the minimum
						             """);
				}
			}
		}
	}
}
