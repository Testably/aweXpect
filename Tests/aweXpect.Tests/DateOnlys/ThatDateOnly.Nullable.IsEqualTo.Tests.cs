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
				[Test]
				public async Task WhenOnlyExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenOnlySubjectIsNull_ShouldFail()
				{
					DateOnly? subject = null;
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Test]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					DateOnly? subject = null;
					DateOnly? expected = null;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = LaterTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}, which differs by -1 day
						              """);
				}

				[Test]
				public async Task WhenSubjectIsTheSame_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = subject;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = null;
					DateOnly? expected = null;

					object Act()
						=> That(subject).IsEqualTo(expected)
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("the expectation is malformed no matter which values it is applied to");
				}

				[Test]
				[Arguments(3, 2, true)]
				[Arguments(5, 3, true)]
				[Arguments(2, 2, false)]
				[Arguments(0, 2, false)]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateOnly? subject = EarlierTime(actualDifference);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsEqualTo(expected)
							.Within(tolerance.Days())
							.Because("we want to test the failure");

					await That(Act).Throws<FailException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)} ± {tolerance} days, because we want to test the failure,
						              but it was {Formatter.Format(subject)}, which differs by -{actualDifference} days
						              """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenSubjectIsDifferent_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(new DateOnly(2010, 11, 13)));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsTheSame_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(new DateOnly(2010, 11, 12)));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 2010-11-12,
						             but it was 2010-11-12
						             """);
				}
			}
		}
	}
}
#endif
