#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsOnOrBefore
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = null;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
				{
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly expected = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly expected = DateOnly.MinValue;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsLater_ShouldFail()
				{
					DateOnly? subject = LaterTime();
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}, which differs by 1 day
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateOnly? expected = CurrentTime();
					DateOnly? subject = null;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Test]
				public async Task WhenSubjectIsSame_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = subject;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectsIsEarlier_ShouldSucceed()
				{
					DateOnly? subject = EarlierTime();
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = EarlierTime(4);

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(3.Days());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)} ± 3 days,
						              but it was {Formatter.Format(subject)}, which differs by 4 days
						              """);
				}

				[Test]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Test]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					object Act()
						=> That(subject).IsOnOrBefore(EarlierTime())
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
				}

				[Test]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = LaterTime(4);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(3.Days())
							.Because("we want to test the failure");

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)} ± 3 days, because we want to test the failure,
						              but it was {Formatter.Format(subject)}, which differs by 4 days
						              """);
				}

				[Test]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = LaterTime(3);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenSubjectIsLater_ShouldSucceed()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(new DateOnly(2010, 11, 11)));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsTheSame_ShouldFail()
				{
					DateOnly? subject = new(2010, 11, 12);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsOnOrBefore(new DateOnly(2010, 11, 12)));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not on or before 2010-11-12,
						             but it was 2010-11-12
						             """);
				}
			}
		}
	}
}
#endif
