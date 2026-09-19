#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatDateOnly
{
	public sealed class IsNotBefore
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				DateOnly subject = DateOnly.MaxValue;
				DateOnly unexpected = DateOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				DateOnly subject = DateOnly.MinValue;
				DateOnly unexpected = DateOnly.MinValue;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				DateOnly subject = EarlierTime();
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				DateOnly subject = CurrentTime();
				DateOnly unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectsIsLater_ShouldSucceed()
			{
				DateOnly subject = LaterTime();
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				DateOnly subject = CurrentTime();
				DateOnly? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenNullableUnexpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateOnly subject = CurrentTime();
				DateOnly? unexpected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected)
						.Within(3.Days())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)} ± 3 days, because we want to test the failure,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				DateOnly subject = DateOnly.MaxValue;
				DateOnly expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Fact]
			public async Task Within_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException()
			{
				DateOnly subject = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(LaterTime())
						.Within(23.Hours());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("tolerance").And
					.WithMessage("Tolerance must be a whole number of days").AsPrefix()
					.Because("a date has no time of day, so the remainder would be dropped without notice");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateOnly subject = EarlierTime(4);
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected)
						.Within(3.Days());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)} ± 3 days,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				DateOnly subject = EarlierTime(3);
				DateOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected)
						.Within(3.Days());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
