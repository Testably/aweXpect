namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed class IsBefore
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime? expected = null;

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(DateTimeKind.Unspecified, DateTimeKind.Local)]
			[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc)]
			[InlineData(DateTimeKind.Unspecified, DateTimeKind.Unspecified)]
			[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified)]
			[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified)]
			[InlineData(DateTimeKind.Local, DateTimeKind.Local)]
			[InlineData(DateTimeKind.Utc, DateTimeKind.Utc)]
			public async Task WhenKindsAreCompatible_ShouldSucceed(DateTimeKind subjectKind, DateTimeKind expectedKind)
			{
				DateTime subject = EarlierTime(1, subjectKind);
				DateTime expected = CurrentTime(expectedKind);

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Because("an Unspecified Kind matches any other Kind and equal Kinds are comparable");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
			{
				DateTime subject = DateTime.MaxValue;
				DateTime expected = DateTime.MaxValue;

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is before 9999-12-31T23:59:59.9999999,
					             but it was 9999-12-31T23:59:59.9999999
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
			{
				DateTime subject = DateTime.MinValue;
				DateTime expected = DateTime.MinValue;

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is before 0001-01-01T00:00:00.0000000,
					             but it was 0001-01-01T00:00:00.0000000
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsLater_ShouldFail()
			{
				DateTime subject = LaterTime();
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsSame_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime expected = subject;

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectOnlyDiffersInKind_ShouldFail()
			{
				DateTime subject = EarlierTime(1, DateTimeKind.Utc);
				DateTime expected = CurrentTime(DateTimeKind.Local);

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Because("a Local and a Utc value cannot be ordered without guessing the offset");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)}, because a Local and a Utc value cannot be ordered without guessing the offset,
					              but it differed in the Kind property
					              """);
			}

			[Fact]
			public async Task WhenSubjectsIsEarlier_ShouldSucceed()
			{
				DateTime subject = EarlierTime();
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_WhenNullableExpectedValueIsOutsideTheTolerance_ShouldFail()
			{
				DateTime subject = CurrentTime();
				DateTime? expected = LaterTime(-3);

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
			{
				DateTime subject = DateTime.MinValue;
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(1.Days());

				await That(Act).DoesNotThrow()
					.Because("a widening tolerance must not make the assertion throw at the type limits");
			}

			[Fact]
			public async Task Within_WhenSubjectOnlyDiffersInKind_ShouldFail()
			{
				DateTime subject = EarlierTime(1, DateTimeKind.Utc);
				DateTime expected = CurrentTime(DateTimeKind.Local);

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(3.Seconds())
						.Because("a tolerance cannot bridge incompatible Kinds");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)} ± 0:03, because a tolerance cannot bridge incompatible Kinds,
					              but it differed in the Kind property
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
			{
				DateTime subject = LaterTime(3);
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(3.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is before {Formatter.Format(expected)} ± 0:03,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
			{
				DateTime subject = LaterTime(2);
				DateTime expected = CurrentTime();

				async Task Act()
					=> await That(subject).IsBefore(expected)
						.Within(3.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
