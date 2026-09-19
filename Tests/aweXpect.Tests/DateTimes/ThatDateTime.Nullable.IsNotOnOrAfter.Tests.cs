namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsNotOnOrAfter
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? unexpected = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not on or after 9999-12-31T23:59:59.9999999,
						             but it was 9999-12-31T23:59:59.9999999
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? unexpected = DateTime.MinValue;

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not on or after 0001-01-01T00:00:00.0000000,
						             but it was 0001-01-01T00:00:00.0000000
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsLater_ShouldFail()
				{
					DateTime? subject = LaterTime();
					DateTime? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not on or after {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not on or after {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not on or after {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectsIsEarlier_ShouldSucceed()
				{
					DateTime? subject = EarlierTime();
					DateTime? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not on or after <null>, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenUnexpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime unexpected = EarlierTime(4)!.Value;

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected)
							.Within(3.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not on or after {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = LaterTime(3);
					DateTime? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not on or after {Formatter.Format(unexpected)} ± 0:03,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateTime? subject = LaterTime(2);
					DateTime? unexpected = CurrentTime();

					async Task Act()
						=> await That(subject).IsNotOnOrAfter(unexpected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
