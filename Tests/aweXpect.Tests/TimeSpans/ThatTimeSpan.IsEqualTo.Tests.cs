namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Because("we want to test the null-case");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>, because we want to test the null-case,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)}, because we want to test the failure,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsMaxValueAndExpectedIsMinValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to the minimum time span,
					             but it was the maximum time span
					             """)
					.Because("a difference that exceeds the range of a time span must fail instead of overflow");
			}

			[Fact]
			public async Task WhenSubjectIsMinValueAndExpectedIsMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to the maximum time span,
					             but it was the minimum time span
					             """)
					.Because("a difference that exceeds the range of a time span must fail instead of overflow");
			}

			[Fact]
			public async Task WhenSubjectIsTheExpectedValue_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_MaximumTolerance_WhenSubjectIsMinValueAndExpectedIsMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(TimeSpan.MaxValue);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to the maximum time span ± {Formatter.Format(TimeSpan.MaxValue)},
					              but it was the minimum time span
					              """)
					.Because("the two values are further apart than the largest possible tolerance");
			}

			[Fact]
			public async Task Within_MaximumTolerance_WhenSubjectIsZero_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.Zero;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(TimeSpan.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("the largest possible tolerance covers the distance from zero to the maximum time span");
			}

			[Fact]
			public async Task Within_NegativeTolerance_ShouldThrowArgumentOutOfRangeException()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Fact]
			public async Task Within_WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).DoesNotThrow()
					.Because("a tolerance must not make equal values at the type limits throw");
			}

			[Fact]
			public async Task Within_WhenSubjectIsMinValueAndExpectedIsMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to the maximum time span ± 0:03,
					             but it was the minimum time span
					             """)
					.Because("a difference that exceeds the range of a time span must fail instead of overflow");
			}

			[Fact]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldBeAccepted()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(23.Hours());

				await That(Act).DoesNotThrow()
					.Because("only a date without a time of day has to reject a sub-day remainder");
			}

			[Fact]
			public async Task Within_WhenValuesAreEarlierWithinTheTolerance_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = EarlierTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenValuesAreLaterWithinTheTolerance_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
