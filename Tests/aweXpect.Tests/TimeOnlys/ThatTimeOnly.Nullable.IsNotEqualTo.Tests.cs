#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenOnlySubjectIsNull_ShouldSucceed()
				{
					TimeOnly? subject = null;
					TimeOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenOnlyUnexpectedIsNull_ShouldSucceed()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
				{
					TimeOnly? subject = null;
					TimeOnly? unexpected = null;

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
				public async Task WhenSubjectAndUnexpectedIsNull_ShouldFail()
				{
					TimeOnly? subject = null;
					TimeOnly? unexpected = null;

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
					TimeOnly? subject = CurrentTime();
					TimeOnly? unexpected = LaterTime();

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					TimeOnly? unexpected = CurrentTime();
					TimeOnly? subject = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsTheSame_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData(3, 2, false)]
				[InlineData(5, 3, false)]
				[InlineData(2, 2, true)]
				[InlineData(0, 2, true)]
				public async Task Within_WhenValuesAreInsideTheTolerance_ShouldFail(
					int actualDifference, int toleranceSeconds, bool expectToThrow)
				{
					TimeSpan tolerance = toleranceSeconds.Seconds();
					TimeOnly? subject = EarlierTime(actualDifference);
					TimeOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected)
							.Within(tolerance)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesWrapAroundMidnight_ShouldFail()
				{
					TimeOnly? subject = TimeOnly.MinValue;
					TimeOnly? unexpected = new TimeOnly(23, 59);

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected)
							.Within(1.Minutes());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 23:59:00.0000000 ± 1:00,
						             but it was 00:00:00.0000000
						             """)
						.Because("equality uses the shortest distance around the clock face");
				}
			}
		}
	}
}
#endif
