namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsLessThanOrEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MaxValue;
				TimeSpan expected = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLater_ShouldFail()
			{
				TimeSpan subject = LaterTime();
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by 0:01
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan expected = subject;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectsIsEarlier_ShouldSucceed()
			{
				TimeSpan subject = EarlierTime();
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = CurrentTime();
				TimeSpan? expected = LaterTime(-4);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
			{
				TimeSpan subject = TimeSpan.MinValue;
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected)
						.Within(1.Seconds());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				TimeSpan subject = LaterTime(4);
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}, which differs by 0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				TimeSpan subject = LaterTime(3);
				TimeSpan expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
