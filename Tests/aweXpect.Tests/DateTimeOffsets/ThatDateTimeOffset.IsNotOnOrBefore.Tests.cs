namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsNotOnOrBefore
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset unexpected = DateTimeOffset.MaxValue;

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not on or before 9999-12-31T23:59:59.9999999+00:00,
					             but it was 9999-12-31T23:59:59.9999999+00:00
					             """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset unexpected = DateTimeOffset.MinValue;

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not on or before 0001-01-01T00:00:00.0000000+00:00,
					             but it was 0001-01-01T00:00:00.0000000+00:00
					             """);
			}

			[Test]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				DateTimeOffset subject = EarlierTime();
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by -0:01
					              """);
			}

			[Test]
			public async Task WhenSubjectIsSame_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset unexpected = subject;

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
				DateTimeOffset subject = LaterTime();
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = null;

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
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected)
						.Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -0:03
					              """);
			}

			[Test]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Test]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = EarlierTime(3);
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:03
					              """);
			}

			[Test]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = LaterTime(2);
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotOnOrBefore(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not on or before {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:02
					              """)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}
		}
	}
}
