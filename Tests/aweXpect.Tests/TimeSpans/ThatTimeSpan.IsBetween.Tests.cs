namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsBetween
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMaximumIsNull_AndNegated_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? minimum = subject;
				TimeSpan? maximum = null;

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
				TimeSpan subject = CurrentTime();
				TimeSpan? minimum = subject;
				TimeSpan? maximum = null;

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
				TimeSpan subject = CurrentTime();

				async Task Act()
					=> await That(subject).IsBetween(LaterTime()).And(EarlierTime());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			public async Task WhenMinimumAndMaximumAreEqual_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();

				async Task Act()
					=> await That(subject).IsBetween(subject).And(subject);

				await That(Act).DoesNotThrow()
					.Because("a range with equal bounds is still a valid range");
			}

			[Test]
			public async Task WhenMinimumIsNull_AndNegated_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? minimum = null;
				TimeSpan? maximum = subject;

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
				TimeSpan subject = CurrentTime();
				TimeSpan? minimum = null;
				TimeSpan? maximum = subject;

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
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan minimum = CurrentTime();
				TimeSpan maximum = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectAndMinimumAreMinValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan minimum = TimeSpan.MinValue;
				TimeSpan maximum = CurrentTime();

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsBetweenMinimumAndMaximum_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan minimum = EarlierTime();
				TimeSpan maximum = LaterTime();

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsEarlierThanMinimum_ShouldFail()
			{
				TimeSpan subject = EarlierTime();
				TimeSpan minimum = CurrentTime();
				TimeSpan maximum = LaterTime();

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}, which differs by -0:01 from the minimum
					              """);
			}

			[Test]
			public async Task WhenSubjectIsLaterThanMaximum_ShouldFail()
			{
				TimeSpan subject = LaterTime();
				TimeSpan minimum = EarlierTime();
				TimeSpan maximum = CurrentTime();

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}, which differs by 0:01 from the maximum
					              """);
			}

			[Test]
			public async Task WhenSubjectIsSameAsMaximum_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan minimum = EarlierTime();
				TimeSpan maximum = subject;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsSameAsMinimum_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan minimum = subject;
				TimeSpan maximum = LaterTime();

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
				TimeSpan subject = CurrentTime();

				async Task Act()
					=> await That(subject).IsBetween(LaterTime()).And(EarlierTime())
						.Within(3.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
					.Because("a tolerance must not turn an inverted range into a satisfiable one");
			}

			[Test]
			public async Task WhenMaximumValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = LaterTime(4);
				TimeSpan minimum = TimeSpan.MinValue;
				TimeSpan maximum = CurrentTime();

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:04 from the maximum
					              """);
			}

			[Test]
			public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = EarlierTime(4);
				TimeSpan minimum = CurrentTime();
				TimeSpan maximum = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:04 from the minimum
					              """);
			}

			[Test]
			public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan minimum = TimeSpan.MinValue;
				TimeSpan? maximum = LaterTime(-4);

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:04 from the maximum
					              """);
			}

			[Test]
			public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? minimum = EarlierTime(-4);
				TimeSpan maximum = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:04 from the minimum
					              """);
			}

			[Test]
			public async Task WhenValueIsWithinTheMaximumTolerance_ShouldSucceed()
			{
				TimeSpan subject = LaterTime(3);
				TimeSpan minimum = TimeSpan.MinValue;
				TimeSpan maximum = CurrentTime();

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsWithinTheMinimumTolerance_ShouldSucceed()
			{
				TimeSpan subject = EarlierTime(3);
				TimeSpan minimum = CurrentTime();
				TimeSpan maximum = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(maximum)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				TimeSpan subject = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsBetween(TimeSpan.MinValue).And(TimeSpan.MaxValue)
						.Within(1.Seconds());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Test]
			public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
			{
				TimeSpan subject = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsBetween(TimeSpan.MinValue).And(TimeSpan.MaxValue)
						.Within(1.Seconds());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}
		}
	}
}
