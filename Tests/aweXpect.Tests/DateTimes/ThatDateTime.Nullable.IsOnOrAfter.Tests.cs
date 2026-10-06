namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsOnOrAfter
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = null;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				[Arguments(DateTimeKind.Utc, DateTimeKind.Unspecified)]
				[Arguments(DateTimeKind.Unspecified, DateTimeKind.Utc)]
				[Arguments(DateTimeKind.Local, DateTimeKind.Unspecified)]
				[Arguments(DateTimeKind.Unspecified, DateTimeKind.Local)]
				public async Task WhenKindIsUnspecified_ShouldSucceed(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime? expected = DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind);

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(DateTimeKind.Utc, DateTimeKind.Local)]
				[Arguments(DateTimeKind.Local, DateTimeKind.Utc)]
				public async Task WhenKindsAreIncompatible_AndNegated_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime? expected = DateTime.SpecifyKind(LaterTime()!.Value, expectedKind);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsOnOrAfter(expected));

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not on or after {Formatter.Format(expected)},
						              but it had kind {subjectKind}, which cannot be compared with {expectedKind}
						              """)
						.Because("values of incompatible kinds cannot be ordered, so the negation fails as well");
				}

				[Test]
				[Arguments(DateTimeKind.Utc, DateTimeKind.Local)]
				[Arguments(DateTimeKind.Local, DateTimeKind.Utc)]
				public async Task WhenKindsAreIncompatible_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind expectedKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime? expected = DateTime.SpecifyKind(CurrentTime()!.Value, expectedKind);

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)},
						              but it had kind {subjectKind}, which cannot be compared with {expectedKind}
						              """);
				}

				[Test]
				public async Task WhenSubjectAndExpectedAreMaxValue_ShouldSucceed()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? expected = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectAndExpectedAreMinValue_ShouldSucceed()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? expected = DateTime.MinValue;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsEarlier_ShouldFail()
				{
					DateTime? subject = EarlierTime();
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}, which differs by -0:01
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					DateTime? subject = null;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)},
						              but it was <null>
						              """);
				}

				[Test]
				public async Task WhenSubjectIsSame_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? expected = subject;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime? expected = CurrentTime(DateTimeKind.Local);

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Because("a Local and a Utc value cannot be ordered without guessing the offset");

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)}, because a Local and a Utc value cannot be ordered without guessing the offset,
						              but it had kind Utc, which cannot be compared with Local
						              """);
				}

				[Test]
				public async Task WhenSubjectsIsLater_ShouldSucceed()
				{
					DateTime? subject = LaterTime();
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task Within_WhenExpectedValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime expected = EarlierTime(-4)!.Value;

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)}, which differs by -0:04
						              """);
				}

				[Test]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Test]
				public async Task Within_WhenSubjectOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Utc);
					DateTime? expected = CurrentTime(DateTimeKind.Local);

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Seconds())
							.Because("a tolerance cannot bridge incompatible Kinds");

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)} ± 0:03, because a tolerance cannot bridge incompatible Kinds,
						              but it had kind Utc, which cannot be compared with Local
						              """);
				}

				[Test]
				public async Task Within_WhenValuesAreOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = EarlierTime(4);
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is on or after {Formatter.Format(expected)} ± 0:03,
						              but it was {Formatter.Format(subject)}, which differs by -0:04
						              """);
				}

				[Test]
				public async Task Within_WhenValuesAreWithinTheTolerance_ShouldSucceed()
				{
					DateTime? subject = EarlierTime(3);
					DateTime? expected = CurrentTime();

					async Task Act()
						=> await That(subject).IsOnOrAfter(expected)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
