namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsAfter
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_AndNegated_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsAfter(expected));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after <null>,
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("nothing can be ordered against null, so the negation fails as well");
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = null;

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after <null>,
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
					DateTime? subject = DateTime.SpecifyKind(LaterTime()!.Value, subjectKind);
					DateTime? expected = DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind);

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Utc)]
				public async Task WhenKindsAreIncompatible_AndNegated_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime? expected = DateTime.SpecifyKind(LaterTime()!.Value, expectedKind);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsAfter(expected));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not after {Formatter.Format(expected)},
						              but it had Kind {subjectKind}, which cannot be compared with {expectedKind}
						              """)
						.Because("values of incompatible kinds cannot be ordered, so the negation fails as well");
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Utc)]
				public async Task WhenKindsAreIncompatible_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(LaterTime()!.Value, subjectKind);
					DateTime? expected = DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind);

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)},
						              but it had Kind {subjectKind}, which cannot be compared with {expectedKind}
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldFail()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? expected = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is after 9999-12-31T23:59:59.9999999,
						             but it was 9999-12-31T23:59:59.9999999
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldFail()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? expected = DateTime.MinValue;

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is after 0001-01-01T00:00:00.0000000,
						             but it was 0001-01-01T00:00:00.0000000
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsEarlier_ShouldFail()
				{
					DateTime? subject = EarlierTime();
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)} which differs by -0:01
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsSame_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = subject;

					async Task Act()
						=> await That(subject).IsAfter(expected)
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)}, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = LaterTime(1, DateTimeKind.Utc);
					DateTime? expected = CurrentTime(DateTimeKind.Local);

					async Task Act()
						=> await That(subject).IsAfter(expected)
							.Because("a Local and a Utc value cannot be ordered without guessing the offset");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)}, because a Local and a Utc value cannot be ordered without guessing the offset,
						              but it had Kind Utc, which cannot be compared with Local
						              """);
				}

				[Fact]
				public async Task WhenSubjectsIsLater_ShouldSucceed()
				{
					DateTime? subject = LaterTime();
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime expected = EarlierTime(-3)!.Value;

					async Task Act()
						=> await That(subject).IsAfter(expected)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by -0:03
						              """);
				}

				[Fact]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsAfter(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenSubjectOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = LaterTime(1, DateTimeKind.Utc);
					DateTime? expected = CurrentTime(DateTimeKind.Local);

					async Task Act()
						=> await That(subject).IsAfter(expected)
							.Within(3.Seconds())
							.Because("a tolerance cannot bridge incompatible Kinds");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)} ± 0:03, because a tolerance cannot bridge incompatible Kinds,
						              but it had Kind Utc, which cannot be compared with Local
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = EarlierTime(3);
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsAfter(expected)
							.Within(3.Seconds())
							.Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is after {Formatter.Format(expected)} ± 0:03, because we want to test the failure,
						              but it was {Formatter.Format(subject)} which differs by -0:03
						              """);
				}

				[Fact]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateTime? subject = EarlierTime(2);
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsAfter(expected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
