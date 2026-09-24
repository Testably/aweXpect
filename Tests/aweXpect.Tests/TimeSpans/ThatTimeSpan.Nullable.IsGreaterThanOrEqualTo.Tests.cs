namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed partial class Nullable
	{
		public sealed class IsGreaterThanOrEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? expected = null;

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is greater than or equal to <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
				{
					TimeSpan? subject = TimeSpan.MaxValue;
					TimeSpan expected = TimeSpan.MaxValue;

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan expected = TimeSpan.MinValue;

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsEarlier_ShouldFail()
				{
					TimeSpan? subject = EarlierTime();
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is greater than or equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)} which differs by -0:01
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					TimeSpan? subject = null;

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(TimeSpan.Zero);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is greater than or equal to 0:00,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? expected = subject;

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectsIsLater_ShouldSucceed()
				{
					TimeSpan? subject = LaterTime();
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? expected = EarlierTime(-4);

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is greater than or equal to {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by -0:04
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					TimeSpan? subject = TimeSpan.MaxValue;
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected)
							.Within(1.Seconds());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					TimeSpan? subject = EarlierTime(4);
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is greater than or equal to {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by -0:04
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					TimeSpan? subject = EarlierTime(3);
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsGreaterThanOrEqualTo(expected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
