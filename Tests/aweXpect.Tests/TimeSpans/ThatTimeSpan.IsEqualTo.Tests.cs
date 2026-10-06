using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Because("we want to test the null-case");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>, because we want to test the null-case,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)}, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -0:01
					              """);
			}

			[Test]
			public async Task WhenSubjectIsMaxValueAndExpectedIsMinValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to TimeSpan.MinValue,
					             but it was TimeSpan.MaxValue
					             """)
					.Because("a difference that exceeds the range of a time span must fail instead of overflow");
			}

			[Test]
			public async Task WhenSubjectIsMaxValueAndExpectedIsSlightlySmaller_ShouldShowTheDifference()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MaxValue - 1.Seconds();

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was TimeSpan.MaxValue, which differs by 0:01
					              """)
					.Because("a difference within the range of a time span is shown even at the limits");
			}

			[Test]
			public async Task WhenSubjectIsMinValueAndExpectedIsMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to TimeSpan.MaxValue,
					             but it was TimeSpan.MinValue
					             """)
					.Because("a difference that exceeds the range of a time span must fail instead of overflow");
			}

			[Test]
			public async Task WhenSubjectIsTheExpectedValue_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTheDefaultToleranceIsSet_ShouldMentionIt()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(4);

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(3.Seconds());
					await That(subject).IsEqualTo(expected);
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:04
					              """)
					.Because("the applied default tolerance is part of the expectation");
			}

			[Test]
			public async Task Within_MaximumTolerance_WhenSubjectIsMinValueAndExpectedIsMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(TimeSpan.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to TimeSpan.MaxValue ± {Formatter.Format(TimeSpan.MaxValue)},
					              but it was TimeSpan.MinValue
					              """)
					.Because("the two values are further apart than the largest possible tolerance");
			}

			[Test]
			public async Task Within_MaximumTolerance_WhenSubjectIsZero_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.Zero;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(TimeSpan.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("the largest possible tolerance covers the distance from zero to the maximum time span");
			}

			[Test]
			public async Task Within_NegativeTolerance_ShouldThrowArgumentOutOfRangeException()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			public async Task Within_WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).DoesNotThrow()
					.Because("a tolerance must not make equal values at the type limits throw");
			}

			[Test]
			public async Task Within_WhenSubjectIsMinValueAndExpectedIsMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to TimeSpan.MaxValue ± 0:03,
					             but it was TimeSpan.MinValue
					             """)
					.Because("a difference that exceeds the range of a time span must fail instead of overflow");
			}

			[Test]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldBeAccepted()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(23.Hours());

				await That(Act).DoesNotThrow()
					.Because("only a date without a time of day has to reject a sub-day remainder");
			}

			[Test]
			public async Task Within_WhenValuesAreEarlierWithinTheTolerance_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = EarlierTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Within_WhenValuesAreLaterWithinTheTolerance_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -0:04
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(6.Seconds()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(5.Seconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 0:05,
					             but it was 0:05
					             """);
			}
		}
	}
}
