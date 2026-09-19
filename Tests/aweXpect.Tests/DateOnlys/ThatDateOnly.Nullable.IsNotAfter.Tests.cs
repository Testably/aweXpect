#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
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
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly unexpected = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly unexpected = DateOnly.MinValue;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsLater_ShouldFail()
				{
					DateOnly? subject = LaterTime();
					DateOnly? unexpected = CurrentTime();

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
					DateOnly? expected = CurrentTime();
					DateOnly? subject = null;

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
					DateOnly? subject = CurrentTime();
					DateOnly? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectsIsEarlier_ShouldSucceed()
				{
					DateOnly? subject = EarlierTime();
					DateOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? unexpected = null;

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
					DateOnly? subject = CurrentTime();
					DateOnly? unexpected = EarlierTime(4);

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Within(3.Days())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after {Formatter.Format(unexpected)} ± 3 days, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(EarlierTime())
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("Tolerance must be a whole number of days").AsPrefix()
						.Because("a date has no time of day, so the remainder would be dropped without notice");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = LaterTime(4);
					DateOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Within(3.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after {Formatter.Format(unexpected)} ± 3 days,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = LaterTime(3);
					DateOnly? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotAfter(unexpected)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
