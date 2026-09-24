namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? unexpected = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 9999-12-31T23:59:59.9999999, because we want to test the failure,
						             but it was 9999-12-31T23:59:59.9999999
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? unexpected = DateTime.MinValue;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 0001-01-01T00:00:00.0000000, because we want to test the failure,
						             but it was 0001-01-01T00:00:00.0000000
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTime? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to <null>,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? unexpected = LaterTime();

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsTheSame_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? unexpected = subject;

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
				public async Task WhenSubjectOnlyDiffersInKind_ShouldSucceed()
				{
					DateTime? subject = new(2024, 11, 1, 14, 15, 0, DateTimeKind.Utc);
					DateTime? unexpected = new(2024, 11, 1, 14, 15, 0, DateTimeKind.Local);

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected)
							.Because("we also test the kind property");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_NegativeTolerance_ShouldThrowArgumentOutOfRangeException()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = LaterTime(4);

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(-1.Seconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
						.WithParamName("tolerance");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = LaterTime(4);

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = LaterTime(3);

					async Task Act()
						=> await That(subject).IsNotEqualTo(expected).Within(3.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(expected)} ± 0:03, because we want to test the failure,
						              but it was {Formatter.Format(subject)} which differs by -0:03
						              """);
				}
			}
		}
	}
}
