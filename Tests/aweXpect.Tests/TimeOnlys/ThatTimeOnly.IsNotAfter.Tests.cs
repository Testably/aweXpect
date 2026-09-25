#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	public sealed class IsNotAfter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				TimeOnly subject = TimeOnly.MaxValue;
				TimeOnly unexpected = TimeOnly.MaxValue;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				TimeOnly subject = TimeOnly.MinValue;
				TimeOnly unexpected = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLater_ShouldFail()
			{
				TimeOnly subject = LaterTime();
				TimeOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by 0:01
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				TimeOnly subject = CurrentTime();
				TimeOnly unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectsIsEarlier_ShouldSucceed()
			{
				TimeOnly subject = EarlierTime();
				TimeOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				TimeOnly subject = CurrentTime();
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
				TimeOnly subject = CurrentTime();
				TimeOnly? unexpected = EarlierTime(4);

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected)
						.Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)}, which differs by 0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenToleranceWouldWrapAroundMidnight_ShouldFail()
			{
				TimeOnly subject = new(0, 30);
				TimeOnly unexpected = TimeOnly.MinValue;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected)
						.Within(1.Hours());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)} ± 1:00:00,
					              but it was {Formatter.Format(subject)}, which differs by 30:00
					              """)
					.Because("the ordering does not wrap around midnight, so the tolerance extends the unnegated expectation to every time on that side");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				TimeOnly subject = LaterTime(4);
				TimeOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
			{
				TimeOnly subject = EarlierTime(2);
				TimeOnly unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by -0:02
					              """)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}
		}
	}
}
#endif
