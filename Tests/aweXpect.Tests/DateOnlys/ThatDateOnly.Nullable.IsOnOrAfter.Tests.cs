#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsOnOrAfter
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = null;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
				{
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly expected = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly expected = DateOnly.MinValue;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsEarlier_ShouldFail()
				{
					DateOnly? subject = EarlierTime();
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateOnly? expected = CurrentTime();
					DateOnly? subject = null;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldSucceed()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = subject;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectsIsLater_ShouldSucceed()
				{
					DateOnly? subject = LaterTime();
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = EarlierTime(-4);

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)} ± 3 days,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(LaterTime())
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("Tolerance must be a whole number of days").AsPrefix()
						.Because("a date has no time of day, so the remainder would be dropped without notice");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = EarlierTime(4);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Days())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)} ± 3 days, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = EarlierTime(3);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
