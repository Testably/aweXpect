namespace aweXpect.Tests;

public sealed partial class ThatDateTime
{
	public sealed partial class Nullable
	{
		public sealed class IsBetween
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified, DateTimeKind.Unspecified)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Unspecified, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Unspecified, DateTimeKind.Utc, DateTimeKind.Local)]
				public async Task WhenKindIsUnspecified_ShouldSucceed(
					DateTimeKind subjectKind, DateTimeKind minimumKind, DateTimeKind maximumKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime? minimum = DateTime.SpecifyKind(EarlierTime()!.Value, minimumKind);
					DateTime? maximum = DateTime.SpecifyKind(LaterTime()!.Value, maximumKind);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Local, DateTimeKind.Unspecified, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified, DateTimeKind.Local, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Utc, DateTimeKind.Utc, DateTimeKind.Utc)]
				public async Task WhenKindsAreIncompatible_AndNegated_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind minimumKind, DateTimeKind maximumKind,
					DateTimeKind incompatibleKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime? minimum = DateTime.SpecifyKind(LaterTime()!.Value, minimumKind);
					DateTime? maximum = DateTime.SpecifyKind(LaterTime(2)!.Value, maximumKind);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsBetween(minimum).And(maximum));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it had Kind {subjectKind}, which cannot be compared with {incompatibleKind}
						              """)
						.Because("values of incompatible kinds cannot be ordered, so the negation fails as well");
				}

				[Theory]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Local, DateTimeKind.Unspecified, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Utc, DateTimeKind.Unspecified, DateTimeKind.Local, DateTimeKind.Local)]
				[InlineData(DateTimeKind.Local, DateTimeKind.Utc, DateTimeKind.Utc, DateTimeKind.Utc)]
				public async Task WhenKindsAreIncompatible_ShouldFail(
					DateTimeKind subjectKind, DateTimeKind minimumKind, DateTimeKind maximumKind,
					DateTimeKind incompatibleKind)
				{
					DateTime? subject = DateTime.SpecifyKind(CurrentTime()!.Value, subjectKind);
					DateTime? minimum = DateTime.SpecifyKind(EarlierTime()!.Value, minimumKind);
					DateTime? maximum = DateTime.SpecifyKind(LaterTime()!.Value, maximumKind);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it had Kind {subjectKind}, which cannot be compared with {incompatibleKind}
						              """);
				}

				[Fact]
				public async Task WhenMaximumIsNull_AndNegated_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = subject;
					DateTime? maximum = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsBetween(minimum).And(maximum));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not between {Formatter.Format(minimum)} and <null>,
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("nothing can be ordered against a null bound, so the negation fails as well");
				}

				[Fact]
				public async Task WhenMaximumIsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = subject;
					DateTime? maximum = null;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and <null>,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateTime? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
				}

				[Fact]
				public async Task WhenMaximumOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Local);
					DateTime? minimum = EarlierTime(1, DateTimeKind.Local);
					DateTime? maximum = LaterTime(1, DateTimeKind.Utc);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Because("the subject must be comparable to both bounds");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}, because the subject must be comparable to both bounds,
						              but it had Kind Local, which cannot be compared with Utc
						              """);
				}

				[Fact]
				public async Task WhenMinimumAndMaximumAreEqual_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(subject).And(subject);

					await That(Act).DoesNotThrow()
						.Because("a range with equal bounds is still a valid range");
				}

				[Fact]
				public async Task WhenMinimumIsNull_AndNegated_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = null;
					DateTime? maximum = subject;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsBetween(minimum).And(maximum));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not between <null> and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """)
						.Because("nothing can be ordered against a null bound, so the negation fails as well");
				}

				[Fact]
				public async Task WhenMinimumIsNull_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = null;
					DateTime? maximum = subject;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between <null> and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenMinimumOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Local);
					DateTime? minimum = EarlierTime(1, DateTimeKind.Utc);
					DateTime? maximum = LaterTime(1, DateTimeKind.Local);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Because("the subject must be comparable to both bounds");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)}, because the subject must be comparable to both bounds,
						              but it had Kind Local, which cannot be compared with Utc
						              """);
				}

				[Fact]
				public async Task WhenSubjectAndMaximumAreMaxValue_ShouldSucceed()
				{
					DateTime? subject = DateTime.MaxValue;
					DateTime? minimum = CurrentTime();
					DateTime? maximum = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectAndMinimumAreMinValue_ShouldSucceed()
				{
					DateTime? subject = DateTime.MinValue;
					DateTime? minimum = DateTime.MinValue;
					DateTime? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsBetweenMinimumAndMaximum_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = EarlierTime();
					DateTime? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsEarlierThanMinimum_ShouldFail()
				{
					DateTime? subject = EarlierTime();
					DateTime? minimum = CurrentTime();
					DateTime? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)} which differs by -0:01 from the minimum
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsLaterThanMaximum_ShouldFail()
				{
					DateTime? subject = LaterTime();
					DateTime? minimum = EarlierTime();
					DateTime? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
						              but it was {Formatter.Format(subject)} which differs by 0:01 from the maximum
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNullAndMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateTime? subject = null;

					async Task Act()
						=> await That(subject).IsBetween(LaterTime()).And(EarlierTime());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("the range is rejected for its own sake, independently of the subject");
				}

				[Fact]
				public async Task WhenSubjectIsSameAsMaximum_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = EarlierTime();
					DateTime? maximum = subject;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsSameAsMinimum_ShouldSucceed()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = subject;
					DateTime? maximum = LaterTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectKindIsUnspecifiedAndTheBoundsDifferInKind_ShouldSucceed()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Unspecified);
					DateTime? minimum = EarlierTime(1, DateTimeKind.Local);
					DateTime? maximum = LaterTime(1, DateTimeKind.Utc);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum);

					await That(Act).DoesNotThrow()
						.Because("only the subject is compared against each bound, and Unspecified matches anything");
				}
			}

			public sealed class WithinTests
			{
				[Fact]
				public async Task WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
				{
					DateTime? subject = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(LaterTime()).And(EarlierTime())
							.Within(3.Seconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("maximum").And
						.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix()
						.Because("a tolerance must not turn an inverted range into a satisfiable one");
				}

				[Fact]
				public async Task WhenMaximumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = LaterTime(4);
					DateTime minimum = DateTime.MinValue;
					DateTime? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by 0:04 from the maximum
						              """);
				}

				[Fact]
				public async Task WhenMinimumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = EarlierTime(4);
					DateTime? minimum = CurrentTime();
					DateTime maximum = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by -0:04 from the minimum
						              """);
				}

				[Fact]
				public async Task WhenNullableMaximumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime minimum = DateTime.MinValue;
					DateTime? maximum = LaterTime(-4);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by 0:04 from the maximum
						              """);
				}

				[Fact]
				public async Task WhenNullableMinimumValueIsOutsideTheTolerance_ShouldFail()
				{
					DateTime? subject = CurrentTime();
					DateTime? minimum = EarlierTime(-4);
					DateTime maximum = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03,
						              but it was {Formatter.Format(subject)} which differs by -0:04 from the minimum
						              """);
				}

				[Fact]
				public async Task WhenSubjectOnlyDiffersInKind_ShouldFail()
				{
					DateTime? subject = CurrentTime(DateTimeKind.Local);
					DateTime? minimum = EarlierTime(1, DateTimeKind.Utc);
					DateTime? maximum = LaterTime(1, DateTimeKind.Utc);

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Seconds())
							.Because("a tolerance cannot bridge incompatible Kinds");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} ± 0:03, because a tolerance cannot bridge incompatible Kinds,
						              but it had Kind Local, which cannot be compared with Utc
						              """);
				}

				[Fact]
				public async Task WhenValueIsWithinTheMaximumTolerance_ShouldSucceed()
				{
					DateTime? subject = LaterTime(3);
					DateTime minimum = DateTime.MinValue;
					DateTime? maximum = CurrentTime();

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenValueIsWithinTheMinimumTolerance_ShouldSucceed()
				{
					DateTime? subject = EarlierTime(3);
					DateTime? minimum = CurrentTime();
					DateTime maximum = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(minimum).And(maximum)
							.Within(3.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Within_WhenSubjectIsMaxValue_ShouldNotOverflow()
				{
					DateTime? subject = DateTime.MaxValue;

					async Task Act()
						=> await That(subject).IsBetween(DateTime.MinValue).And(DateTime.MaxValue)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}

				[Fact]
				public async Task Within_WhenSubjectIsMinValue_ShouldNotOverflow()
				{
					DateTime? subject = DateTime.MinValue;

					async Task Act()
						=> await That(subject).IsBetween(DateTime.MinValue).And(DateTime.MaxValue)
							.Within(1.Days());

					await That(Act).DoesNotThrow()
						.Because("a widening tolerance must not make the assertion throw at the type limits");
				}
			}
		}
	}
}
