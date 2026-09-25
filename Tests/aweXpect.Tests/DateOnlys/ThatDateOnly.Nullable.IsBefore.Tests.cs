#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed partial class Nullable
	{
		public sealed class IsBefore
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = null;

					async Task Act()
						=> await That(subject).IsBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is before <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
				{
					DateOnly? subject = DateOnly.MaxValue;
					DateOnly expected = DateOnly.MaxValue;

					async Task Act()
						=> await That(subject).IsBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is before 9999-12-31,
						             but it was 9999-12-31
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly expected = DateOnly.MinValue;

					async Task Act()
						=> await That(subject).IsBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is before 0001-01-01,
						             but it was 0001-01-01
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsLater_ShouldFail()
				{
					DateOnly? subject = LaterTime();
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is before {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)} which differs by 1 day
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateOnly? expected = CurrentTime();
					DateOnly? subject = null;

					async Task Act()
						=> await That(subject).IsBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is before {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = subject;

					async Task Act()
						=> await That(subject).IsBefore(expected)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is before {Formatter.Format(expected)}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectsIsEarlier_ShouldSucceed()
				{
					DateOnly? subject = EarlierTime();
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = CurrentTime();
					DateOnly? expected = EarlierTime(4);

					async Task Act()
						=> await That(subject).IsBefore(expected)
							.Within(3.Days());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is before {Formatter.Format(expected)} ± 3 days,
						              but it was {Formatter.Format(subject)} which differs by 4 days
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateOnly? subject = DateOnly.MinValue;
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsBefore(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
				{
					DateOnly? subject = CurrentTime();

					object Act()
						=> That(subject).IsBefore(EarlierTime())
							.Within(23.Hours());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("tolerance").And
						.WithMessage("Tolerance must be a whole number of days").AsPrefix()
						.Because("a date has no time of day, so the remainder is rejected as soon as it is specified instead of when the expectation is awaited");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateOnly? subject = LaterTime(3);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsBefore(expected)
							.Within(3.Days())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is before {Formatter.Format(expected)} ± 3 days, because we want to test the failure,
						              but it was {Formatter.Format(subject)} which differs by 3 days
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateOnly? subject = LaterTime(2);
					DateOnly? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsBefore(expected)
							.Within(3.Days());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
