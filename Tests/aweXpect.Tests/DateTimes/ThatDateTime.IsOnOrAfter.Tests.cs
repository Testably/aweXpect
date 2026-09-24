namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class IsOnOrAfter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime? expected = null;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified)]
			[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc)]
			[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified)]
			[InlineData(DateTimeKind.Unspecified, DateTimeKind.Local)]
			public async Task WhenKindIsUnspecified_ShouldSucceed(
				DateTimeKind subjectKind, DateTimeKind expectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), subjectKind);
				DateTime expected = DateTime.SpecifyKind(CurrentTime(), expectedKind);

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(DateTimeKind.Utc, DateTimeKind.Local)]
			[InlineData(DateTimeKind.Local, DateTimeKind.Utc)]
			public async Task WhenKindsAreIncompatible_ShouldFail(
				DateTimeKind subjectKind, DateTimeKind expectedKind)
			{
				DateTime subject = DateTime.SpecifyKind(CurrentTime(), subjectKind);
				DateTime expected = DateTime.SpecifyKind(CurrentTime(), expectedKind);

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)},
					              but it had Kind {subjectKind}, which cannot be compared with {expectedKind}
					              """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
			{
				DateTime subject = DateTime.MaxValue;
				DateTime expected = DateTime.MaxValue;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
			{
				DateTime subject = DateTime.MinValue;
				DateTime expected = DateTime.MinValue;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsEarlier_ShouldFail()
			{
				DateTime subject = EarlierTime();
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by -0:01
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldSucceed()
			{
				DateTime subject = CurrentTime();
				DateTime expected = subject;

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectOnlyDiffersInKind_ShouldFail()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime expected = CurrentTime(DateTimeKind.Local);

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Because("a Local and a Utc value cannot be ordered without guessing the offset");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)}, because a Local and a Utc value cannot be ordered without guessing the offset,
					              but it had Kind Utc, which cannot be compared with Local
					              """);
			}

			[Fact]
			public async Task WhenSubjectsIsLater_ShouldSucceed()
			{
				DateTime subject = LaterTime();
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime? expected = EarlierTime(-4);

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)} which differs by -0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
			{
				DateTime subject = DateTime.MaxValue;
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Fact]
			public async Task Within_WhenSubjectOnlyDiffersInKind_ShouldFail()
			{
				DateTime subject = CurrentTime(DateTimeKind.Utc);
				DateTime expected = CurrentTime(DateTimeKind.Local);

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(3.Seconds())
						.Because("a tolerance cannot bridge incompatible Kinds");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)} ± 0:03, because a tolerance cannot bridge incompatible Kinds,
					              but it had Kind Utc, which cannot be compared with Local
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTime subject = EarlierTime(4);
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is on or after {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)} which differs by -0:04
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				DateTime subject = EarlierTime(3);
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsOnOrAfter(expected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
