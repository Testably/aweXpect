namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset expected = DateTimeOffset.MaxValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset expected = DateTimeOffset.MinValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = LaterTime();

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
			public async Task WhenSubjectIsTheExpectedValue_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_NegativeTolerance_ShouldThrowArgumentOutOfRangeException()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Fact]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldBeAccepted()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(23.Hours());

				await That(Act).DoesNotThrow()
					.Because("only a date without a time of day has to reject a sub-day remainder");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = LaterTime(4);

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

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? expected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
