#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
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
					TimeOnly? subject = CurrentTime();
					TimeOnly? expected = null;

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
					TimeOnly? subject = TimeOnly.MaxValue;
					TimeOnly expected = TimeOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					TimeOnly? subject = TimeOnly.MinValue;
					TimeOnly expected = TimeOnly.MinValue;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsEarlier_ShouldFail()
				{
					TimeOnly? subject = EarlierTime();
					TimeOnly? expected = CurrentTime();

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
					TimeOnly? expected = CurrentTime();
					TimeOnly? subject = null;

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
					TimeOnly? subject = CurrentTime();
					TimeOnly? expected = subject;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectsIsLater_ShouldSucceed()
				{
					TimeOnly? subject = LaterTime();
					TimeOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? expected = EarlierTime(-4);

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenToleranceWouldWrapAroundMidnight_ShouldSucceed()
				{
					TimeOnly? subject = new TimeOnly(23, 30);
					TimeOnly? expected = new TimeOnly(23, 0);

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(1.Hours());

					await That(Act).DoesNotThrow()
						.Because("a tolerance must never make an expectation fail that passes without it");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					TimeOnly? subject = EarlierTime(4);
					TimeOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)} ± 0:03, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly? subject = EarlierTime(3);
					TimeOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
