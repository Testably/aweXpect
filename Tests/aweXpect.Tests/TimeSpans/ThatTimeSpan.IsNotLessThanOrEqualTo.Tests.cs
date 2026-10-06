namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsNotLessThanOrEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan unexpected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to TimeSpan.MaxValue,
					             but it was TimeSpan.MaxValue
					             """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan unexpected = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to TimeSpan.MinValue,
					             but it was TimeSpan.MinValue
					             """);
			}

			[Test]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				TimeSpan subject = EarlierTime();
				TimeSpan unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by -0:01
					              """);
			}

			[Test]
			public async Task WhenSubjectIsSame_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectsIsLater_ShouldSucceed()
			{
				TimeSpan subject = LaterTime();
				TimeSpan unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to <null>, because we want to test the failure,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task Within_WhenNullableUnexpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? unexpected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected)
						.Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -0:03
					              """);
			}

			[Test]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(expected)
						.Within(1.Seconds());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Test]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = EarlierTime(3);
				TimeSpan unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:03
					              """);
			}

			[Test]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
			{
				TimeSpan subject = LaterTime(2);
				TimeSpan unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:02
					              """)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsGreater_ShouldFail()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLessThanOrEqualTo(4.Seconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to 0:04,
					             but it was 0:05, which differs by 0:01
					             """);
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				TimeSpan subject = 5.Seconds();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLessThanOrEqualTo(5.Seconds()));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
