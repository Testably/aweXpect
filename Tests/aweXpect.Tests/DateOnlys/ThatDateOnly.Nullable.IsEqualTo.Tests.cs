#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
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
					DateOnly? subject = CurrentTime();
					DateOnly? expected = null;

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
					DateOnly? subject = null;
					DateOnly? expected = CurrentTime();

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
					DateOnly? subject = null;
					DateOnly? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedIsNull_ShouldSucceed()
				{
					DateOnly? subject = null;
					DateOnly? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = LaterTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}, which differs by -1 day
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateOnly? expected = CurrentTime();
					DateOnly? subject = null;

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
					DateOnly? subject = CurrentTime();
					DateOnly? expected = subject;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = null;
					DateOnly? expected = null;

					object Act()
						=> That(subject).IsEqualTo(expected)
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("Tolerance must be a whole number of days").AsPrefix()
						.Because("the expectation is malformed no matter which values it is applied to");
				}

				[Theory]
				[InlineData(3, 2, true)]
				[InlineData(5, 3, true)]
				[InlineData(2, 2, false)]
				[InlineData(0, 2, false)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateOnly? subject = EarlierTime(actualDifference);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.Within(tolerance.Days())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)} ± {tolerance} days, because we want to test the failure,
						              but it was {Formatter.Format(subject)}, which differs by -{actualDifference} days
						              """);
				}
			}
		}
	}
}
#endif
