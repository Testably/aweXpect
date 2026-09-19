namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed partial class Nullable
	{
		public sealed class IsNotBetween
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenMaximumIsNull_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? minimum = subject;
					DateTimeOffset? maximum = null;

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
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateTimeOffset? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
				}

				[Fact]
				public async Task WhenMinimumAndMaximumAreEqual_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(subject).And(subject);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(subject)} and {Formatter.Format(subject)},
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("a range with equal bounds is still a valid range");
				}

				[Fact]
				public async Task WhenMinimumIsNull_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? minimum = null;
					DateTimeOffset? maximum = subject;

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
				public async Task WhenSubjectAndMaximumAreMaxValue_ShouldFail()
				{
					DateTimeOffset? subject = DateTimeOffset.MaxValue;
					DateTimeOffset? minimum = CurrentTime();
					DateTimeOffset maximum = DateTimeOffset.MaxValue;

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
					DateTimeOffset? subject = DateTimeOffset.MinValue;
					DateTimeOffset minimum = DateTimeOffset.MinValue;
					DateTimeOffset? maximum = CurrentTime();

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
					DateTimeOffset? subject = EarlierTime();
					DateTimeOffset? minimum = CurrentTime();
					DateTimeOffset? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsLaterThanMaximum_ShouldSucceed()
				{
					DateTimeOffset? subject = LaterTime();
					DateTimeOffset? minimum = EarlierTime();
					DateTimeOffset? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNotBetweenMinimumAndMaximum_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? minimum = EarlierTime();
					DateTimeOffset? maximum = LaterTime();

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
				public async Task WhenSubjectIsSameAsMaximum_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? minimum = EarlierTime();
					DateTimeOffset? maximum = subject;

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
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? minimum = subject;
					DateTimeOffset? maximum = LaterTime();

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
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateTimeOffset? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(LaterTime()).And(EarlierTime())
							.Within(3.Seconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("a tolerance must not turn an inverted range into a satisfiable one");
				}

				[Fact]
				public async Task WhenMaximumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset minimum = DateTimeOffset.MinValue;
					DateTimeOffset? maximum = LaterTime(2);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? minimum = EarlierTime(2);
					DateTimeOffset maximum = DateTimeOffset.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset minimum = DateTimeOffset.MinValue;
					DateTimeOffset? maximum = LaterTime(2);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? minimum = EarlierTime(2);
					DateTimeOffset maximum = DateTimeOffset.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenValueIsWithinTheMaximumTolerance_ShouldFail()
				{
					DateTimeOffset? subject = EarlierTime(3);
					DateTimeOffset minimum = DateTimeOffset.MinValue;
					DateTimeOffset? maximum = CurrentTime();

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
					DateTimeOffset? subject = LaterTime(3);
					DateTimeOffset? minimum = CurrentTime();
					DateTimeOffset maximum = DateTimeOffset.MaxValue;

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
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					DateTimeOffset? subject = DateTimeOffset.MaxValue;
					DateTimeOffset? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(DateTimeOffset.MinValue).And(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateTimeOffset? subject = DateTimeOffset.MinValue;
					DateTimeOffset? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(expected).And(DateTimeOffset.MaxValue)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}
			}
		}
	}
}
