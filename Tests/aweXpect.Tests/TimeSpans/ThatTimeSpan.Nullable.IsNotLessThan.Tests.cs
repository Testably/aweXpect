namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed partial class Nullable
	{
		public sealed class IsNotLessThan
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
				{
					TimeSpan? subject = TimeSpan.MaxValue;
					TimeSpan unexpected = TimeSpan.MaxValue;

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					TimeSpan? subject = TimeSpan.MinValue;
					TimeSpan unexpected = TimeSpan.MinValue;

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsEarlier_ShouldFail()
				{
					TimeSpan? subject = EarlierTime();
					TimeSpan? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not less than {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)} which differs by -0:01
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					TimeSpan? subject = null;

					async Task Act()
						=> await That(subject).IsNotLessThan(TimeSpan.Zero);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not less than 0:00,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldSucceed()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectsIsLater_ShouldSucceed()
				{
					TimeSpan? subject = LaterTime();
					TimeSpan? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not less than <null>, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenNullableUnexpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					TimeSpan? subject = CurrentTime();
					TimeSpan? unexpected = LaterTime(4);

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected)
							.Within(3.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not less than {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
						              but it was {Formatter.Format(subject)} which differs by -0:04
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					TimeSpan? subject = TimeSpan.MaxValue;
					TimeSpan? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotLessThan(expected)
							.Within(1.Seconds());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					TimeSpan? subject = EarlierTime(4);
					TimeSpan? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not less than {Formatter.Format(unexpected)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by -0:04
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
				{
					TimeSpan? subject = LaterTime(2);
					TimeSpan? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotLessThan(unexpected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not less than {Formatter.Format(unexpected)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by 0:02
						              """)
						.Because("the tolerance widens the unnegated expectation and so narrows its negation");
				}
			}
		}
	}
}
