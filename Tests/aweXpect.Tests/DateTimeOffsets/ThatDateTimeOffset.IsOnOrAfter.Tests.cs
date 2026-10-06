namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsOnOrAfter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = null;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset expected = DateTimeOffset.MaxValue;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset expected = DateTimeOffset.MinValue;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				DateTimeOffset subject = EarlierTime();
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by -0:01
					              """);
			}

			[Test]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset expected = subject;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectsIsLater_ShouldSucceed()
			{
				DateTimeOffset subject = LaterTime();
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = EarlierTime(-4);

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:04
					              """);
			}

			[Test]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Test]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = EarlierTime(4);
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(3.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:04
					              """);
			}

			[Test]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				DateTimeOffset subject = EarlierTime(3);
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
