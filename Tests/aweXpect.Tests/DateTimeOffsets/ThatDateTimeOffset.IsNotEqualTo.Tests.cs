namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset unexpected = DateTimeOffset.MaxValue;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 9999-12-31T23:59:59.9999999+00:00, because we want to test the failure,
					             but it was 9999-12-31T23:59:59.9999999+00:00
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset unexpected = DateTimeOffset.MinValue;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 0001-01-01T00:00:00.0000000+00:00, because we want to test the failure,
					             but it was 0001-01-01T00:00:00.0000000+00:00
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = LaterTime();

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)}, because we want to test the failure,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task Within_NegativeTolerance_ShouldThrowArgumentOutOfRangeException()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = LaterTime(3);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by -0:03
					              """);
			}
		}
	}
}
