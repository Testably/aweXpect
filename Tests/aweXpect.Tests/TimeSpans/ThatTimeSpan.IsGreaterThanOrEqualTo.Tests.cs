namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsGreaterThanOrEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				TimeSpan subject = EarlierTime();
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by -0:01
					              """);
			}

			[Test]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan expected = subject;

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectsIsLater_ShouldSucceed()
			{
				TimeSpan subject = LaterTime();
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = EarlierTime(-4);

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than or equal to {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:04
					              """);
			}

			[Test]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected)
						.Within(1.Seconds());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Test]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = EarlierTime(4);
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than or equal to {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:04
					              """);
			}

			[Test]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				TimeSpan subject = EarlierTime(3);
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsGreaterThanOrEqualTo(expected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsGreater_ShouldFail()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsGreaterThanOrEqualTo(4.Seconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than or equal to 0:04,
					             but it was 0:05, which differs by 0:01
					             """);
			}

			[Test]
			public async Task WhenSubjectIsLess_ShouldSucceed()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsGreaterThanOrEqualTo(6.Seconds()));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
