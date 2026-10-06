#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenOnlySubjectIsNull_ShouldSucceed()
				{
					DateOnly? subject = null;
					DateOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenOnlyUnexpectedIsNull_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
				{
					DateOnly? subject = null;
					DateOnly? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to <null>,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsDifferent_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? unexpected = LaterTime();

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					DateOnly? unexpected = CurrentTime();
					DateOnly? subject = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsTheSame_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					object Act()
						=> That(subject).IsNotEqualTo(LaterTime())
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
				}

				[Test]
				[Arguments(3, 2, false)]
				[Arguments(5, 3, false)]
				[Arguments(2, 2, true)]
				[Arguments(0, 2, true)]
				public async Task Within_WhenValuesAreInsideTheTolerance_ShouldFail(
					int actualDifference, int tolerance, bool expectToThrow)
				{
					DateOnly? subject = EarlierTime(actualDifference);
					DateOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected)
							.Within(tolerance.Days())
							.Because("we want to test the failure");

					string difference = actualDifference == 0
						? ""
						: $", which differs by -{actualDifference} days";

					await That(Act).Throws<FailException>()
						.OnlyIf(expectToThrow)
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)} ± {tolerance} days, because we want to test the failure,
						              but it was {Formatter.Format(subject)}{difference}
						              """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotEqualTo(new DateOnly(2010, 11, 13)));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 2010-11-13,
						             but it was 2010-11-12, which differs by -1 day
						             """);
				}

				[Test]
				public async Task WhenSubjectIsTheSame_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotEqualTo(new DateOnly(2010, 11, 12)));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
