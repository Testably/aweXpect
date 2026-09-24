namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class IsNotBefore
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified)]
			[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc)]
			[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified)]
			[InlineData(DateTimeKind.Unspecified, DateTimeKind.Local)]
			public async Task WhenKindIsUnspecified_ShouldSucceed(
				DateTimeKind subjectKind, DateTimeKind unexpectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(LaterTime(), subjectKind);
				DateTime unexpected = DateTime.SpecifyKind(CurrentTime(), unexpectedKind);

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(DateTimeKind.Utc, DateTimeKind.Local)]
			[InlineData(DateTimeKind.Local, DateTimeKind.Utc)]
			public async Task WhenKindsAreIncompatible_ShouldFail(
				DateTimeKind subjectKind, DateTimeKind unexpectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(LaterTime(), subjectKind);
				DateTime unexpected = DateTime.SpecifyKind(CurrentTime(), unexpectedKind);

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)},
					              but it had Kind {subjectKind}, which cannot be compared with {unexpectedKind}
					              """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				DateTime subject = DateTime.MaxValue;
				DateTime unexpected = DateTime.MaxValue;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				DateTime subject = DateTime.MinValue;
				DateTime unexpected = DateTime.MinValue;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				DateTime subject = EarlierTime();
				DateTime unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)} which differs by -0:01
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				DateTime subject = CurrentTime();
				DateTime unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectsIsLater_ShouldSucceed()
			{
				DateTime subject = LaterTime();
				DateTime unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Fact]
			public async Task Within_WhenNullableUnexpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime? unexpected = LaterTime(4);

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected)
						.Within(3.Seconds())
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)} ± 0:03, because we want to test the failure,
					              but it was {Formatter.Format(subject)} which differs by -0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				DateTime subject = DateTime.MaxValue;
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTime subject = EarlierTime(4);
				DateTime unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)} which differs by -0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldFail()
			{
				DateTime subject = LaterTime(2);
				DateTime unexpected = CurrentTime();

				async Task Act()
					=> await That(subject).IsNotBefore(unexpected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not before {Formatter.Format(unexpected)} ± 0:03,
					              but it was {Formatter.Format(subject)} which differs by 0:02
					              """)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}
		}
	}
}
