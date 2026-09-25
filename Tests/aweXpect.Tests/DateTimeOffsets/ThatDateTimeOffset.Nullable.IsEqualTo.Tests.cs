namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed partial class Nullable
	{
		public sealed class IsEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					DateTimeOffset? subject = null;
					DateTimeOffset? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? expected = LaterTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)} which differs by -0:01
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsTheSame_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? expected = subject;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_NegativeTolerance_ShouldThrowArgumentOutOfRangeException()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? expected = LaterTime(4);

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(-1.Seconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
						.WithParamName("tolerance");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? expected = LaterTime(4);

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(3.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)} ± 0:03, because we want to test the failure,
						              but it was {Formatter.Format(subject)} which differs by -0:04
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateTimeOffset? subject = CurrentTime();
					DateTimeOffset? expected = LaterTime(3);

					async Task Act()
						=> await That(subject).IsEqualTo(expected).Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
