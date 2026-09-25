#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class IsBefore
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly? expected = null;

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
				TimeOnly subject = TimeOnly.MaxValue;
				TimeOnly expected = TimeOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is before 23:59:59.9999999,
					             but it was 23:59:59.9999999
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly expected = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is before 00:00:00.0000000,
					             but it was 00:00:00.0000000
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsLater_ShouldFail()
			{
				TimeOnly subject = LaterTime();
				TimeOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by 0:01
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly expected = subject;

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
				TimeOnly subject = EarlierTime();
				TimeOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly? expected = EarlierTime(4);

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly expected = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(1.Hours());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must never wrap around midnight");
			}

			[Fact]
			public async Task Within_WhenSubjectIsBeforeMidnightAndExpectedIsAfter_ShouldFail()
			{
				TimeOnly subject = new(23, 0);
				TimeOnly expected = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(2.Hours());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is before 00:00:00.0000000 ± 2:00:00,
					             but it was 23:00:00.0000000, which differs by 23:00:00
					             """)
					.Because("ordering is linear, so midnight stays a boundary for is before");
			}

			[Fact]
			public async Task Within_WhenToleranceWouldWrapAroundMidnight_ShouldSucceed()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly expected = new(0, 30);

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(1.Hours());

				await That(Act).DoesNotThrow()
					.Because("a tolerance must never make an expectation fail that passes without it");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				TimeOnly subject = LaterTime(3);
				TimeOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by 0:03
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				TimeOnly subject = LaterTime(2);
				TimeOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
