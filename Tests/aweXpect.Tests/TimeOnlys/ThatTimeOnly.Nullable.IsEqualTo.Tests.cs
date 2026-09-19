#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenOnlyExpectedIsNull_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenOnlySubjectIsNull_ShouldFail()
				{
					TimeOnly? subject = null;
					TimeOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					TimeOnly? subject = null;
					TimeOnly? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? expected = LaterTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					TimeOnly? expected = CurrentTime();
					TimeOnly? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsTheSame_ShouldSucceed()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? expected = subject;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(3, 2, true)]
				[InlineData(5, 3, true)]
				[InlineData(2, 2, false)]
				[InlineData(0, 2, false)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int toleranceSeconds, bool expectToThrow)
				{
					TimeSpan tolerance = toleranceSeconds.Seconds();
					TimeOnly? subject = EarlierTime(actualDifference);
					TimeOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.Within(tolerance)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesWrapAroundMidnight_ShouldSucceed()
				{
					TimeOnly? subject = TimeOnly.MinValue;
					TimeOnly? expected = new TimeOnly(23, 59);

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.Within(1.Minutes());

					await That(Act).DoesNotThrow()
						.Because("equality uses the shortest distance around the clock face");
				}
			}
		}
	}
}
#endif
