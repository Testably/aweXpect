namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsNotBetween
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenBoundsWithMixedKindsAreInvertedInTicks_ShouldThrowArgumentOutOfRangeException()
				{
					DateTime? subject = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

					async Task Act()
						=> await That(subject)
							.IsNotBetween(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc))
							.And(new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Local));

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("an unspecified subject is compared with both bounds in ticks, so the negation would succeed vacuously");
				}

				[Test]
				[Arguments(DateTimeKind.Utc, DateTimeKind.Unspecified, DateTimeKind.Unspecified)]
				[Arguments(DateTimeKind.Local, DateTimeKind.Unspecified, DateTimeKind.Local)]
				[Arguments(DateTimeKind.Unspecified, DateTimeKind.Utc, DateTimeKind.Local)]
				public async Task WhenKindIsUnspecified_ShouldSucceed(
					DateTimeKind subjectKind, DateTimeKind minimumKind, DateTimeKind maximumKind)
				{
					DateTime? subject = DateTime.SpecifyKind(LaterTime(2)!.Value, subjectKind);
					DateTime? minimum = DateTime.SpecifyKind(EarlierTime()!.Value, minimumKind);
					DateTime? maximum = DateTime.SpecifyKind(LaterTime()!.Value, maximumKind);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(DateTimeKind.Utc, DateTimeKind.Local, DateTimeKind.Unspecified, DateTimeKind.Local)]
				[Arguments(DateTimeKind.Utc, DateTimeKind.Unspecified, DateTimeKind.Local, DateTimeKind.Local)]
				[Arguments(DateTimeKind.Local, DateTimeKind.Utc, DateTimeKind.Utc, DateTimeKind.Utc)]
				public async Task WhenKindsAreIncompatible_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind minimumKind, DateTimeKind maximumKind,
					DateTimeKind incompatibleKind)
				{
					DateTime? subject = DateTime.SpecifyKind(LaterTime(2)!.Value, subjectKind);
					DateTime? minimum = DateTime.SpecifyKind(EarlierTime()!.Value, minimumKind);
					DateTime? maximum = DateTime.SpecifyKind(LaterTime()!.Value, maximumKind);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it had kind {subjectKind}, which cannot be compared with {incompatibleKind}
						              """);
				}

				[Test]
				public async Task WhenMaximumHasAnIncompatibleKindAndFewerTicksThanTheMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Local);
					DateTime? minimum = EarlierTime(1, DateTimeKind.Local);
					DateTime? maximum = EarlierTime(2, DateTimeKind.Utc);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("the subject is compared with the bounds in ticks, so a range inverted in ticks can never contain it");
				}

				[Test]
				public async Task WhenMaximumIsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = subject;
					DateTime? maximum = null;

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
					DateTime? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
				}

				[Test]
				public async Task WhenMinimumAndMaximumAreEqual_ShouldFail()
				{
					DateTime? subject = CurrentTime();

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
					DateTime? subject = CurrentTime();
					DateTime? minimum = null;
					DateTime? maximum = subject;

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
					DateTime? subject = DateTime.MaxValue;
					DateTime? minimum = CurrentTime();
					DateTime maximum = DateTime.MaxValue;

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
					DateTime? subject = DateTime.MinValue;
					DateTime minimum = DateTime.MinValue;
					DateTime? maximum = CurrentTime();

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
					DateTime? subject = EarlierTime();
					DateTime? minimum = CurrentTime();
					DateTime? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsLaterThanMaximum_ShouldSucceed()
				{
					DateTime? subject = LaterTime();
					DateTime? minimum = EarlierTime();
					DateTime? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNotBetweenMinimumAndMaximum_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = EarlierTime();
					DateTime? maximum = LaterTime();

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
					DateTime? subject = CurrentTime();
					DateTime? minimum = EarlierTime();
					DateTime? maximum = subject;

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
					DateTime? subject = CurrentTime();
					DateTime? minimum = subject;
					DateTime? maximum = LaterTime();

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
					DateTime? subject = CurrentTime();

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
					DateTime? subject = CurrentTime();
					DateTime minimum = DateTime.MinValue;
					DateTime maximum = EarlierTime(4)!.Value;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime minimum = LaterTime(4)!.Value;
					DateTime maximum = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime minimum = DateTime.MinValue;
					DateTime? maximum = EarlierTime(4);

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = LaterTime(4);
					DateTime maximum = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsNotBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenValueIsWithinTheMaximumTolerance_ShouldFail()
				{
					DateTime? subject = LaterTime(3);
					DateTime minimum = DateTime.MinValue;
					DateTime? maximum = CurrentTime();

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
					DateTime? subject = EarlierTime(3);
					DateTime? minimum = CurrentTime();
					DateTime maximum = DateTime.MaxValue;

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
					DateTime? subject = DateTime.MaxValue;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotBetween(DateTime.MinValue).And(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Test]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? expected = DateTime.MinValue.AddDays(2);

					async Task Act()
						=> await That(subject).IsNotBetween(expected).And(DateTime.MaxValue)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}
			}
		}
	}
}
