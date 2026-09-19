namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsOnOrBefore
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = null;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? expected = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? expected = DateTime.MinValue;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsLater_ShouldFail()
				{
					DateTime? subject = LaterTime();
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = subject;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectsIsEarlier_ShouldSucceed()
				{
					DateTime? subject = EarlierTime();
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime expected = LaterTime(-4)!.Value;

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = LaterTime(4);
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is on or before {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateTime? subject = LaterTime(3);
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrBefore(expected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
