#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed class IsNotOnOrBefore
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
			{
				DateOnly subject = DateOnly.MaxValue;
				DateOnly unexpected = DateOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
			{
				DateOnly subject = DateOnly.MinValue;
				DateOnly unexpected = DateOnly.MinValue;

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				DateOnly subject = EarlierTime();
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by -1 day
					              """);
			}

			[Test]
			public async Task WhenSubjectIsSame_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectsIsLater_ShouldSucceed()
			{
				DateOnly subject = LaterTime();
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task Within_WhenNullableUnexpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly? unexpected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected)
						.Within(3.Days())
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)} ± 3 days, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -3 days
					              """);
			}

			[Test]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				DateOnly subject = DateOnly.MaxValue;
				DateOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Test]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
			{
				DateOnly subject = CurrentTime();

				object Act()
					=> That(subject).IsNotOnOrBefore(LaterTime())
						.Within(23.Hours());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("tolerance").And
					.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
					.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
			}

			[Test]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateOnly subject = EarlierTime(3);
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected)
						.Within(3.Days());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)} ± 3 days,
					              but it was {Formatter.Format(subject)}, which differs by -3 days
					              """);
			}

			[Test]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
			{
				DateOnly subject = LaterTime(2);
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected)
						.Within(3.Days());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)} ± 3 days,
					              but it was {Formatter.Format(subject)}, which differs by 2 days
					              """)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsLater_ShouldFail()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotOnOrBefore(new DateOnly(2010, 11, 11)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is on or before 2010-11-11,
					             but it was 2010-11-12, which differs by 1 day
					             """);
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				DateOnly subject = new(2010, 11, 12);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotOnOrBefore(new DateOnly(2010, 11, 12)));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
