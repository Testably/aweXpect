namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotBetween
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(null, (byte)1, ", which differs by 1 from the maximum")]
			[Arguments((byte)1, null, "")]
			public async Task ForByte_WhenMinimumOrMaximumIsNull_ShouldFail(
				byte? minimum, byte? maximum, string differenceSuffix)
			{
				byte subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((byte)2, (byte)1, (byte)3)]
			[Arguments((byte)2, (byte)2, (byte)4)]
			[Arguments((byte)2, (byte)1, (byte)2)]
			public async Task ForByte_WhenValueIsInRangeExpected_ShouldFail(byte subject,
				byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((byte)2, (byte)0, (byte)1)]
			[Arguments((byte)0, (byte)1, (byte)2)]
			public async Task ForByte_WhenValueIsOutsideTheRange_ShouldSucceed(byte subject,
				byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, 1.1, ", which differs by 0.9 from the maximum")]
			[Arguments(1.1, null, "")]
			public async Task ForDecimal_WhenMinimumOrMaximumIsNull_ShouldFail(double? minimumValue,
				double? maximumValue, string differenceSuffix)
			{
				decimal subject = 2;
				decimal? minimum = minimumValue is null ? null : new decimal(minimumValue.Value);
				decimal? maximum = maximumValue is null ? null : new decimal(maximumValue.Value);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments(2.0, 1.0, 3.0)]
			[Arguments(2.0, 2.0, 4.0)]
			[Arguments(2.0, 1.0, 2.0)]
			public async Task ForDecimal_WhenValueIsInRangeExpected_ShouldFail(
				double subjectValue, double expectedMinimum, double expectedMaximum)
			{
				decimal subject = new(subjectValue);
				decimal? minimum = new(expectedMinimum);
				decimal? maximum = new(expectedMaximum);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 0.0, 1.9)]
			[Arguments(0.0, 0.1, 2.0)]
			public async Task ForDecimal_WhenValueIsOutsideTheRange_ShouldSucceed(
				double subjectValue, double minimumValue, double maximumValue)
			{
				decimal subject = new(subjectValue);
				decimal minimum = new(minimumValue);
				decimal maximum = new(maximumValue);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.NaN, 1.0, "minimum")]
			[Arguments(1.0, double.NaN, "maximum")]
			public async Task ForDouble_WhenMinimumOrMaximumIsNaN_ShouldThrowArgumentOutOfRangeException(
				double minimum, double maximum, string paramName)
			{
				double subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName(paramName).And
					.WithMessage($"The {paramName} must not be NaN.").AsPrefix();
			}

			[Test]
			[Arguments(null, 1.1, ", which differs by 0.9 from the maximum")]
			[Arguments(1.1, null, "")]
			public async Task ForDouble_WhenMinimumOrMaximumIsNull_ShouldFail(
				double? minimum, double? maximum, string differenceSuffix)
			{
				double subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments(2.0, 1.0, 3.0)]
			[Arguments(2.0, 2.0, 4.0)]
			[Arguments(2.0, 1.0, 2.0)]
			public async Task ForDouble_WhenValueIsInRangeExpected_ShouldFail(
				double subject, double minimum, double maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 0.0, 1.9)]
			[Arguments(0.0, 0.1, 2.0)]
			public async Task ForDouble_WhenValueIsOutsideTheRange_ShouldSucceed(
				double subject, double minimum, double maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, 1.0F, ", which differs by 1.0 from the maximum")]
			[Arguments(1.0F, null, "")]
			public async Task ForFloat_WhenMinimumOrMaximumIsNull_ShouldFail(
				float? minimum, float? maximum, string differenceSuffix)
			{
				float subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((float)2.0, (float)1.0, (float)3.0)]
			[Arguments((float)2.0, (float)2.0, (float)4.0)]
			[Arguments((float)2.0, (float)1.0, (float)2.0)]
			public async Task ForFloat_WhenValueIsInRangeExpected_ShouldFail(
				float subject, float minimum, float maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.0, (float)0.0, (float)1.9)]
			[Arguments((float)0.0, (float)0.1, (float)2.0)]
			public async Task ForFloat_WhenValueIsOutsideTheRange_ShouldSucceed(
				float subject, float minimum, float maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForInt_WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				int subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(2).And(1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			[Arguments(null, 1, ", which differs by 1 from the maximum")]
			[Arguments(1, null, "")]
			public async Task ForInt_WhenMinimumOrMaximumIsNull_ShouldFail(
				int? minimum, int? maximum, string differenceSuffix)
			{
				int subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments(2, 1, 3)]
			[Arguments(2, 2, 4)]
			[Arguments(2, 1, 2)]
			public async Task ForInt_WhenValueIsInRangeExpected_ShouldFail(int subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForInt_WhenValueIsInsideTheToleranceWidenedRange_ShouldShowTheDifference()
			{
				int subject = 5;

				async Task Act()
					=> await That(subject).IsNotBetween(1).And(4).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not between 1 and 4 ± 1,
					             but it was 5, which differs by 1 from the maximum
					             """)
					.Because("inside the tolerance the difference to the nearer bound explains the failure, like for the time types");
			}

			[Test]
			[Arguments(2, 0, 1)]
			[Arguments(0, 1, 2)]
			public async Task ForInt_WhenValueIsOutsideTheRange_ShouldSucceed(int subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(null, 1, ", which differs by 1 from the maximum")]
			[Arguments(1, null, "")]
			public async Task ForInt128_WhenMinimumOrMaximumIsNull_ShouldFail(
				int? minimumValue, int? maximumValue, string differenceSuffix)
			{
				Int128 subject = 2;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, 3)]
			[Arguments(2, 2, 4)]
			[Arguments(2, 1, 2)]
			public async Task ForInt128_WhenValueIsInRangeExpected_ShouldFail(
				int subjectValue, int minimumValue, int maximumValue)
			{
				Int128 subject = subjectValue;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 0, 1)]
			[Arguments(0, 1, 2)]
			public async Task ForInt128_WhenValueIsOutsideTheRange_ShouldSucceed(
				int subjectValue, int minimumValue, int maximumValue)
			{
				Int128 subject = subjectValue;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			[Arguments(null, (long)1, ", which differs by 1 from the maximum")]
			[Arguments((long)1, null, "")]
			public async Task ForLong_WhenMinimumOrMaximumIsNull_ShouldFail(
				long? minimum, long? maximum, string differenceSuffix)
			{
				long subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((long)2, (long)1, (long)3)]
			[Arguments((long)2, (long)2, (long)4)]
			[Arguments((long)2, (long)1, (long)2)]
			public async Task ForLong_WhenValueIsInRangeExpected_ShouldFail(long subject,
				long? minimum, long maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((long)2, (long)0, (long)1)]
			[Arguments((long)0, (long)1, (long)2)]
			public async Task ForLong_WhenValueIsOutsideTheRange_ShouldSucceed(long subject,
				long? minimum, long maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (byte)1, ", which differs by 1 from the maximum")]
			[Arguments((byte)1, null, "")]
			public async Task ForNullableByte_WhenMinimumOrMaximumIsNull_ShouldFail(
				byte? minimum, byte? maximum, string differenceSuffix)
			{
				byte? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((byte)2, (byte)1, (byte)3)]
			[Arguments((byte)2, (byte)2, (byte)4)]
			[Arguments((byte)2, (byte)1, (byte)2)]
			public async Task ForNullableByte_WhenValueIsInRangeExpected_ShouldFail(
				byte? subject, byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail()
			{
				byte? subject = null;
				byte minimum = 1;
				byte maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((byte)2, (byte)0, (byte)1)]
			[Arguments((byte)0, (byte)1, (byte)2)]
			public async Task ForNullableByte_WhenValueIsOutsideTheRange_ShouldSucceed(
				byte? subject, byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, 1.1, ", which differs by 0.9 from the maximum")]
			[Arguments(1.1, null, "")]
			public async Task ForNullableDecimal_WhenMinimumOrMaximumIsNull_ShouldFail(double? minimumValue,
				double? maximumValue, string differenceSuffix)
			{
				decimal? subject = 2;
				decimal? minimum = minimumValue == null ? null : new decimal(minimumValue.Value);
				decimal? maximum = maximumValue == null ? null : new decimal(maximumValue.Value);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments(2.0, 1.0, 3.0)]
			[Arguments(2.0, 2.0, 4.0)]
			[Arguments(2.0, 1.0, 2.0)]
			public async Task ForNullableDecimal_WhenValueIsInRangeExpected_ShouldFail(
				double? subjectValue, double? minimumValue, double? maximumValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? minimum = minimumValue == null ? null : new decimal(minimumValue.Value);
				decimal? maximum = maximumValue == null ? null : new decimal(maximumValue.Value);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 0.0, 1.9)]
			[Arguments(0.0, 0.1, 2.0)]
			public async Task ForNullableDecimal_WhenValueIsOutsideTheRange_ShouldSucceed(
				double? subjectValue, double? minimumValue, double? maximumValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? minimum = minimumValue == null ? null : new decimal(minimumValue.Value);
				decimal? maximum = maximumValue == null ? null : new decimal(maximumValue.Value);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, 1.1, ", which differs by 0.9 from the maximum")]
			[Arguments(1.1, null, "")]
			public async Task ForNullableDouble_WhenMinimumOrMaximumIsNull_ShouldFail(double? minimum,
				double? maximum, string differenceSuffix)
			{
				double? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments(2.0, 1.0, 3.0)]
			[Arguments(2.0, 2.0, 4.0)]
			[Arguments(2.0, 1.0, 2.0)]
			public async Task ForNullableDouble_WhenValueIsInRangeExpected_ShouldFail(
				double? subject, double? minimum, double? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 0.0, 1.9)]
			[Arguments(0.0, 0.1, 2.0)]
			public async Task ForNullableDouble_WhenValueIsOutsideTheRange_ShouldSucceed(
				double? subject, double? minimum, double? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, 1.1F, ", which differs by 0.9 from the maximum")]
			[Arguments(1.1F, null, "")]
			public async Task ForNullableFloat_WhenMinimumOrMaximumIsNull_ShouldFail(
				float? minimum, float? maximum, string differenceSuffix)
			{
				float? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((float)2.0, (float)1.0, (float)3.0)]
			[Arguments((float)2.0, (float)2.0, (float)4.0)]
			[Arguments((float)2.0, (float)1.0, (float)2.0)]
			public async Task ForNullableFloat_WhenValueIsInRangeExpected_ShouldFail(
				float? subject, float? minimum, float? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.0, (float)0.0, (float)1.9)]
			[Arguments((float)0.0, (float)0.1, (float)2.0)]
			public async Task ForNullableFloat_WhenValueIsOutsideTheRange_ShouldSucceed(
				float? subject, float? minimum, float? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableInt_WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				int? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(2).And(1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Test]
			[Arguments(null, 1, ", which differs by 1 from the maximum")]
			[Arguments(1, null, "")]
			public async Task ForNullableInt_WhenMinimumOrMaximumIsNull_ShouldFail(
				int? minimum, int? maximum, string differenceSuffix)
			{
				int? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments(2, 1, 3)]
			[Arguments(2, 2, 4)]
			[Arguments(2, 1, 2)]
			public async Task ForNullableInt_WhenValueIsInRangeExpected_ShouldFail(
				int? subject, int? minimum, int? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableInt_WhenValueIsInsideTheToleranceWidenedRange_ShouldShowTheDifference()
			{
				int? subject = 0;

				async Task Act()
					=> await That(subject).IsNotBetween(1).And(4).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not between 1 and 4 ± 1,
					             but it was 0, which differs by -1 from the minimum
					             """)
					.Because("inside the tolerance the difference to the nearer bound explains the failure, like for the time types");
			}

			[Test]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail()
			{
				int? subject = null;
				int minimum = 1;
				int maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments(2, 0, 1)]
			[Arguments(0, 1, 2)]
			public async Task ForNullableInt_WhenValueIsOutsideTheRange_ShouldSucceed(
				int? subject, int? minimum, int? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}
#if NET8_0_OR_GREATER
			[Test]
			[Arguments(null, 1, ", which differs by 1 from the maximum")]
			[Arguments(1, null, "")]
			public async Task ForNullableInt128_WhenMinimumOrMaximumIsNull_ShouldFail(int? minimumValue,
				int? maximumValue, string differenceSuffix)
			{
				Int128? subject = 2;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, 3)]
			[Arguments(2, 2, 4)]
			[Arguments(2, 1, 2)]
			public async Task ForNullableInt128_WhenValueIsInRangeExpected_ShouldFail(
				int? subjectValue, int? minimumValue, int? maximumValue)
			{
				Int128? subject = subjectValue;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 0, 1)]
			[Arguments(0, 1, 2)]
			public async Task ForNullableInt128_WhenValueIsOutsideTheRange_ShouldSucceed(
				int? subjectValue, int? minimumValue, int? maximumValue)
			{
				Int128? subject = subjectValue;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			[Arguments(null, (long)1, ", which differs by 1 from the maximum")]
			[Arguments((long)1, null, "")]
			public async Task ForNullableLong_WhenMinimumOrMaximumIsNull_ShouldFail(
				long? minimum, long? maximum, string differenceSuffix)
			{
				long? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((long)2, (long)1, (long)3)]
			[Arguments((long)2, (long)2, (long)4)]
			[Arguments((long)2, (long)1, (long)2)]
			public async Task ForNullableLong_WhenValueIsInRangeExpected_ShouldFail(
				long? subject, long? minimum, long? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail()
			{
				long? subject = null;
				long minimum = 1;
				long maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((long)2, (long)0, (long)1)]
			[Arguments((long)0, (long)1, (long)2)]
			public async Task ForNullableLong_WhenValueIsOutsideTheRange_ShouldSucceed(
				long? subject, long? minimum, long? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (sbyte)1, ", which differs by 1 from the maximum")]
			[Arguments((sbyte)1, null, "")]
			public async Task ForNullableSbyte_WhenMinimumOrMaximumIsNull_ShouldFail(
				sbyte? minimum, sbyte? maximum, string differenceSuffix)
			{
				sbyte? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1, (sbyte)3)]
			[Arguments((sbyte)2, (sbyte)2, (sbyte)4)]
			[Arguments((sbyte)2, (sbyte)1, (sbyte)2)]
			public async Task ForNullableSbyte_WhenValueIsInRangeExpected_ShouldFail(
				sbyte? subject, sbyte? minimum, sbyte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail()
			{
				sbyte? subject = null;
				sbyte minimum = 1;
				sbyte maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)0, (sbyte)1)]
			[Arguments((sbyte)0, (sbyte)1, (sbyte)2)]
			public async Task ForNullableSbyte_WhenValueIsOutsideTheRange_ShouldSucceed(
				sbyte? subject, sbyte? minimum, sbyte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (short)1, ", which differs by 1 from the maximum")]
			[Arguments((short)1, null, "")]
			public async Task ForNullableShort_WhenMinimumOrMaximumIsNull_ShouldFail(
				short? minimum, short? maximum, string differenceSuffix)
			{
				short? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((short)2, (short)1, (short)3)]
			[Arguments((short)2, (short)2, (short)4)]
			[Arguments((short)2, (short)1, (short)2)]
			public async Task ForNullableShort_WhenValueIsInRangeExpected_ShouldFail(
				short? subject, short? minimum, short? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail()
			{
				short? subject = null;
				short minimum = 1;
				short maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((short)2, (short)0, (short)1)]
			[Arguments((short)0, (short)1, (short)2)]
			public async Task ForNullableShort_WhenValueIsOutsideTheRange_ShouldSucceed(
				short? subject, short? minimum, short? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (uint)1, ", which differs by 1 from the maximum")]
			[Arguments((uint)1, null, "")]
			public async Task ForNullableUint_WhenMinimumOrMaximumIsNull_ShouldFail(
				uint? minimum, uint? maximum, string differenceSuffix)
			{
				uint? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((uint)2, (uint)1, (uint)3)]
			[Arguments((uint)2, (uint)2, (uint)4)]
			[Arguments((uint)2, (uint)1, (uint)2)]
			public async Task ForNullableUint_WhenValueIsInRangeExpected_ShouldFail(
				uint? subject, uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail()
			{
				uint? subject = null;
				uint minimum = 1;
				uint maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)0, (uint)1)]
			[Arguments((uint)0, (uint)1, (uint)2)]
			public async Task ForNullableUint_WhenValueIsOutsideTheRange_ShouldSucceed(
				uint? subject, uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (ulong)1, ", which differs by 1 from the maximum")]
			[Arguments((ulong)1, null, "")]
			public async Task ForNullableUlong_WhenMinimumOrMaximumIsNull_ShouldFail(
				ulong? minimum, ulong? maximum, string differenceSuffix)
			{
				ulong? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, (ulong)3)]
			[Arguments((ulong)2, (ulong)2, (ulong)4)]
			[Arguments((ulong)2, (ulong)1, (ulong)2)]
			public async Task ForNullableUlong_WhenValueIsInRangeExpected_ShouldFail(
				ulong? subject, ulong? minimum, ulong? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail()
			{
				ulong? subject = null;
				ulong minimum = 1;
				ulong maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)0, (ulong)1)]
			[Arguments((ulong)0, (ulong)1, (ulong)2)]
			public async Task ForNullableUlong_WhenValueIsOutsideTheRange_ShouldSucceed(
				ulong? subject, ulong? minimum, ulong? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (ushort)1, ", which differs by 1 from the maximum")]
			[Arguments((ushort)1, null, "")]
			public async Task ForNullableUshort_WhenMinimumOrMaximumIsNull_ShouldFail(
				ushort? minimum, ushort? maximum, string differenceSuffix)
			{
				ushort? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, (ushort)3)]
			[Arguments((ushort)2, (ushort)2, (ushort)4)]
			[Arguments((ushort)2, (ushort)1, (ushort)2)]
			public async Task ForNullableUshort_WhenValueIsInRangeExpected_ShouldFail(
				ushort? subject, ushort? minimum, ushort? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail()
			{
				ushort? subject = null;
				ushort minimum = 1;
				ushort maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)0, (ushort)1)]
			[Arguments((ushort)0, (ushort)1, (ushort)2)]
			public async Task ForNullableUshort_WhenValueIsOutsideTheRange_ShouldSucceed(
				ushort? subject, ushort? minimum, ushort? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (sbyte)1, ", which differs by 1 from the maximum")]
			[Arguments((sbyte)1, null, "")]
			public async Task ForSbyte_WhenMinimumOrMaximumIsNull_ShouldFail(
				sbyte? minimum, sbyte? maximum, string differenceSuffix)
			{
				sbyte subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1, (sbyte)3)]
			[Arguments((sbyte)2, (sbyte)2, (sbyte)4)]
			[Arguments((sbyte)2, (sbyte)1, (sbyte)2)]
			public async Task ForSbyte_WhenValueIsInRangeExpected_ShouldFail(sbyte subject,
				sbyte? minimum, sbyte maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)0, (sbyte)1)]
			[Arguments((sbyte)0, (sbyte)1, (sbyte)2)]
			public async Task ForSbyte_WhenValueIsOutsideTheRange_ShouldSucceed(sbyte subject,
				sbyte? minimum, sbyte maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (short)1, ", which differs by 1 from the maximum")]
			[Arguments((short)1, null, "")]
			public async Task ForShort_WhenMinimumOrMaximumIsNull_ShouldFail(
				short? minimum, short? maximum, string differenceSuffix)
			{
				short subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((short)2, (short)1, (short)3)]
			[Arguments((short)2, (short)2, (short)4)]
			[Arguments((short)2, (short)1, (short)2)]
			public async Task ForShort_WhenValueIsInRangeExpected_ShouldFail(short subject,
				short? minimum, short maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((short)2, (short)0, (short)1)]
			[Arguments((short)0, (short)1, (short)2)]
			public async Task ForShort_WhenValueIsOutsideTheRange_ShouldSucceed(short subject,
				short? minimum, short maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (uint)1, ", which differs by 1 from the maximum")]
			[Arguments((uint)1, null, "")]
			public async Task ForUint_WhenMinimumOrMaximumIsNull_ShouldFail(
				uint? minimum, uint? maximum, string differenceSuffix)
			{
				uint subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((uint)2, (uint)1, (uint)3)]
			[Arguments((uint)2, (uint)2, (uint)4)]
			[Arguments((uint)2, (uint)1, (uint)2)]
			public async Task ForUint_WhenValueIsInRangeExpected_ShouldFail(uint subject,
				uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)0, (uint)1)]
			[Arguments((uint)0, (uint)1, (uint)2)]
			public async Task ForUint_WhenValueIsOutsideTheRange_ShouldSucceed(uint subject,
				uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (ulong)1, ", which differs by 1 from the maximum")]
			[Arguments((ulong)1, null, "")]
			public async Task ForUlong_WhenMinimumOrMaximumIsNull_ShouldFail(
				ulong? minimum, ulong? maximum, string differenceSuffix)
			{
				ulong subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, (ulong)3)]
			[Arguments((ulong)2, (ulong)2, (ulong)4)]
			[Arguments((ulong)2, (ulong)1, (ulong)2)]
			public async Task ForUlong_WhenValueIsInRangeExpected_ShouldFail(ulong subject,
				ulong? minimum, ulong maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)0, (ulong)1)]
			[Arguments((ulong)0, (ulong)1, (ulong)2)]
			public async Task ForUlong_WhenValueIsOutsideTheRange_ShouldSucceed(ulong subject,
				ulong? minimum, ulong maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(null, (ushort)1, ", which differs by 1 from the maximum")]
			[Arguments((ushort)1, null, "")]
			public async Task ForUshort_WhenMinimumOrMaximumIsNull_ShouldFail(
				ushort? minimum, ushort? maximum, string differenceSuffix)
			{
				ushort subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """)
					.Because("nothing can be ordered against a null bound, so the negation fails as well");
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, (ushort)3)]
			[Arguments((ushort)2, (ushort)2, (ushort)4)]
			[Arguments((ushort)2, (ushort)1, (ushort)2)]
			public async Task ForUshort_WhenValueIsInRangeExpected_ShouldFail(
				ushort subject,
				ushort? minimum, ushort maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)0, (ushort)1)]
			[Arguments((ushort)0, (ushort)1, (ushort)2)]
			public async Task ForUshort_WhenValueIsOutsideTheRange_ShouldSucceed(ushort subject,
				ushort? minimum, ushort maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments(null, 1, ", which differs by 1 from the maximum")]
			[Arguments(1, null, "")]
			public async Task ForInt_WhenMinimumOrMaximumIsNull_ShouldFail(
				int? minimum, int? maximum, string differenceSuffix)
			{
				int subject = 2;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """);
			}

			[Test]
			[Arguments(2, 1, 3)]
			[Arguments(2, 2, 4)]
			[Arguments(2, 1, 2)]
			public async Task ForInt_WhenValueIsInRangeExpected_ShouldSucceed(int subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(2, 0, 1, "1 from the maximum")]
			[Arguments(0, 1, 2, "-1 from the minimum")]
			public async Task ForInt_WhenValueIsOutsideTheRange_ShouldFail(int subject,
				int? minimum, int maximum, string difference)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}, which differs by {difference}
					              """);
			}

			[Test]
			[Arguments(null, 1, ", which differs by 1 from the maximum")]
			[Arguments(1, null, "")]
			public async Task ForNullableInt_WhenMinimumOrMaximumIsNull_ShouldFail(
				int? minimum, int? maximum, string differenceSuffix)
			{
				int? subject = 2;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}{differenceSuffix}
					              """);
			}

			[Test]
			[Arguments(2, 1, 3)]
			[Arguments(2, 2, 4)]
			[Arguments(2, 1, 2)]
			public async Task ForNullableInt_WhenValueIsInRangeExpected_ShouldSucceed(int? subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(2, 0, 1, "1 from the maximum")]
			[Arguments(0, 1, 2, "-1 from the minimum")]
			public async Task ForNullableInt_WhenValueIsOutsideTheRange_ShouldFail(int? subject,
				int? minimum, int maximum, string difference)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}, which differs by {difference}
					              """);
			}
		}
	}
}
