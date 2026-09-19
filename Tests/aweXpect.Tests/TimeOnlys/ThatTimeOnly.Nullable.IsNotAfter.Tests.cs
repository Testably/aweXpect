#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsNotAfter
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
				{
					TimeOnly? subject = TimeOnly.MaxValue;
					TimeOnly unexpected = TimeOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					TimeOnly? subject = TimeOnly.MinValue;
					TimeOnly unexpected = TimeOnly.MinValue;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsLater_ShouldFail()
				{
					TimeOnly? subject = LaterTime();
					TimeOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					TimeOnly? expected = CurrentTime();
					TimeOnly? subject = null;

					async Task Act()
						=> await That(subject).IsNotAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldSucceed()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectsIsEarlier_ShouldSucceed()
				{
					TimeOnly? subject = EarlierTime();
					TimeOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after <null>, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenNullableUnexpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					TimeOnly? subject = CurrentTime();
					TimeOnly? unexpected = EarlierTime(4);

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Within(3.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenToleranceWouldWrapAroundMidnight_ShouldSucceed()
				{
					TimeOnly? subject = new TimeOnly(0, 30);
					TimeOnly? unexpected = TimeOnly.MinValue;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Within(1.Hours());

					await That(Act).DoesNotThrow()
						.Because("a tolerance must never make an expectation fail that passes without it");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					TimeOnly? subject = LaterTime(4);
					TimeOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after {Formatter.Format(unexpected)} ± 0:03,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					TimeOnly? subject = LaterTime(3);
					TimeOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
