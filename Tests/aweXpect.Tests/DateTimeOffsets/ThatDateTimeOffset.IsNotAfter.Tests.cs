namespace aweXpect.Tests;

public sealed partial class ThatDateTimeOffset
{
	public sealed class IsNotAfter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MaxValue;
				DateTimeOffset unexpected = DateTimeOffset.MaxValue;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset unexpected = DateTimeOffset.MinValue;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLater_ShouldFail()
			{
				DateTimeOffset subject = LaterTime();
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)} which differs by 0:01
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectsIsEarlier_ShouldSucceed()
			{
				DateTimeOffset subject = EarlierTime();
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Fact]
			public async Task Within_WhenNullableUnexpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = CurrentTime();
				DateTimeOffset? unexpected = EarlierTime(4);

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected)
						.Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)} which differs by 0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
			{
				DateTimeOffset subject = DateTimeOffset.MinValue;
				DateTimeOffset expected = DateTimeOffset.MinValue.AddDays(2);

				async Task Act()
					=> await That(subject).IsNotAfter(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = LaterTime(4);
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)} which differs by 0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
			{
				DateTimeOffset subject = EarlierTime(2);
				DateTimeOffset unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotAfter(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not after {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)} which differs by -0:02
					              """)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}
		}
	}
}
