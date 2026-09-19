namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsOnOrBefore
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = null;

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or before <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset expected = DateTimeOffset.MaxValue;

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset expected = DateTimeOffset.MinValue;

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLater_ShouldFail()
			{
				DateTimeOffset subject = LaterTime();
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or before {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset expected = subject;

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectsIsEarlier_ShouldSucceed()
			{
				DateTimeOffset subject = EarlierTime();
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = LaterTime(-4);

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or before {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = LaterTime(4);
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or before {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				DateTimeOffset subject = LaterTime(3);
				DateTimeOffset expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrBefore(expected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
