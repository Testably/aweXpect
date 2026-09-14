namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotBetween
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(null, (byte)1)]
			[InlineData((byte)1, null)]
			public async Task ForByte_WhenMinimumOrMaximumIsNull_ShouldSucceed(byte? minimum, byte? maximum)
			{
				byte subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)2, (byte)1, (byte)3)]
			[InlineData((byte)2, (byte)2, (byte)4)]
			[InlineData((byte)2, (byte)1, (byte)2)]
			public async Task ForByte_WhenValueIsInRangeExpected_ShouldFail(byte subject,
				byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((byte)2, (byte)0, (byte)1)]
			[InlineData((byte)0, (byte)1, (byte)2)]
			public async Task ForByte_WhenValueIsOutsideTheRange_ShouldSucceed(byte subject,
				byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, 1.1)]
			[InlineData(1.1, null)]
			public async Task ForDecimal_WhenMinimumOrMaximumIsNull_ShouldSucceed(double? minimumValue,
				double? maximumValue)
			{
				decimal subject = 2;
				decimal? minimum = minimumValue is null ? null : new decimal(minimumValue.Value);
				decimal? maximum = maximumValue is null ? null : new decimal(maximumValue.Value);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2.0, 1.0, 3.0)]
			[InlineData(2.0, 2.0, 4.0)]
			[InlineData(2.0, 1.0, 2.0)]
			public async Task ForDecimal_WhenValueIsInRangeExpected_ShouldFail(
				double subjectValue, double expectedMinimum, double expectedMaximum)
			{
				decimal subject = new(subjectValue);
				decimal? minimum = new(expectedMinimum);
				decimal? maximum = new(expectedMaximum);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData(2.0, 0.0, 1.9)]
			[InlineData(0.0, 0.1, 2.0)]
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

			[Theory]
			[InlineData(null, 1.1)]
			[InlineData(1.1, null)]
			public async Task ForDouble_WhenMinimumOrMaximumIsNull_ShouldSucceed(double? minimum, double? maximum)
			{
				double subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2.0, 1.0, 3.0)]
			[InlineData(2.0, 2.0, 4.0)]
			[InlineData(2.0, 1.0, 2.0)]
			public async Task ForDouble_WhenValueIsInRangeExpected_ShouldFail(
				double subject, double minimum, double maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData(2.0, 0.0, 1.9)]
			[InlineData(0.0, 0.1, 2.0)]
			public async Task ForDouble_WhenValueIsOutsideTheRange_ShouldSucceed(
				double subject, double minimum, double maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, 1.0F)]
			[InlineData(1.0F, null)]
			public async Task ForFloat_WhenMinimumOrMaximumIsNull_ShouldSucceed(float? minimum, float? maximum)
			{
				float subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)2.0, (float)1.0, (float)3.0)]
			[InlineData((float)2.0, (float)2.0, (float)4.0)]
			[InlineData((float)2.0, (float)1.0, (float)2.0)]
			public async Task ForFloat_WhenValueIsInRangeExpected_ShouldFail(
				float subject, float minimum, float maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((float)2.0, (float)0.0, (float)1.9)]
			[InlineData((float)0.0, (float)0.1, (float)2.0)]
			public async Task ForFloat_WhenValueIsOutsideTheRange_ShouldSucceed(
				float subject, float minimum, float maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForInt_WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				int subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(2).And(1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Theory]
			[InlineData(null, 1)]
			[InlineData(1, null)]
			public async Task ForInt_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				int? minimum, int? maximum)
			{
				int subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2, 1, 3)]
			[InlineData(2, 2, 4)]
			[InlineData(2, 1, 2)]
			public async Task ForInt_WhenValueIsInRangeExpected_ShouldFail(int subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData(2, 0, 1)]
			[InlineData(0, 1, 2)]
			public async Task ForInt_WhenValueIsOutsideTheRange_ShouldSucceed(int subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(null, 1)]
			[InlineData(1, null)]
			public async Task ForInt128_WhenMinimumOrMaximumIsNull_ShouldSucceed(int? minimumValue, int? maximumValue)
			{
				Int128 subject = 2;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 1, 3)]
			[InlineData(2, 2, 4)]
			[InlineData(2, 1, 2)]
			public async Task ForInt128_WhenValueIsInRangeExpected_ShouldFail(
				int subjectValue, int minimumValue, int maximumValue)
			{
				Int128 subject = subjectValue;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 0, 1)]
			[InlineData(0, 1, 2)]
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

			[Theory]
			[InlineData(null, (long)1)]
			[InlineData((long)1, null)]
			public async Task ForLong_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				long? minimum, long? maximum)
			{
				long subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)2, (long)1, (long)3)]
			[InlineData((long)2, (long)2, (long)4)]
			[InlineData((long)2, (long)1, (long)2)]
			public async Task ForLong_WhenValueIsInRangeExpected_ShouldFail(long subject,
				long? minimum, long maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((long)2, (long)0, (long)1)]
			[InlineData((long)0, (long)1, (long)2)]
			public async Task ForLong_WhenValueIsOutsideTheRange_ShouldSucceed(long subject,
				long? minimum, long maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)2, (byte)1, (byte)3)]
			[InlineData((byte)2, (byte)2, (byte)4)]
			[InlineData((byte)2, (byte)1, (byte)2)]
			public async Task ForNullableByte_WhenValueIsInRangeExpected_ShouldFail(
				byte? subject, byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail()
			{
				byte? subject = null;
				byte minimum = 1;
				byte maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((byte)2, (byte)0, (byte)1)]
			[InlineData((byte)0, (byte)1, (byte)2)]
			public async Task ForNullableByte_WhenValueIsOutsideTheRange_ShouldSucceed(
				byte? subject, byte? minimum, byte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, 1.1)]
			[InlineData(1.1, null)]
			public async Task ForNullableDecimal_WhenMinimumOrMaximumIsNull_ShouldSucceed(double? minimumValue,
				double? maximumValue)
			{
				decimal subject = 2;
				decimal? minimum = minimumValue == null ? null : new decimal(minimumValue.Value);
				decimal? maximum = maximumValue == null ? null : new decimal(maximumValue.Value);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2.0, 1.0, 3.0)]
			[InlineData(2.0, 2.0, 4.0)]
			[InlineData(2.0, 1.0, 2.0)]
			public async Task ForNullableDecimal_WhenValueIsInRangeExpected_ShouldFail(
				double? subjectValue, double? minimumValue, double? maximumValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? minimum = minimumValue == null ? null : new decimal(minimumValue.Value);
				decimal? maximum = maximumValue == null ? null : new decimal(maximumValue.Value);

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData(2.0, 0.0, 1.9)]
			[InlineData(0.0, 0.1, 2.0)]
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

			[Theory]
			[InlineData(null, 1.1)]
			[InlineData(1.1, null)]
			public async Task ForNullableDouble_WhenMinimumOrMaximumIsNull_ShouldSucceed(double? minimum,
				double? maximum)
			{
				double subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2.0, 1.0, 3.0)]
			[InlineData(2.0, 2.0, 4.0)]
			[InlineData(2.0, 1.0, 2.0)]
			public async Task ForNullableDouble_WhenValueIsInRangeExpected_ShouldFail(
				double? subject, double? minimum, double? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData(2.0, 0.0, 1.9)]
			[InlineData(0.0, 0.1, 2.0)]
			public async Task ForNullableDouble_WhenValueIsOutsideTheRange_ShouldSucceed(
				double? subject, double? minimum, double? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, 1.1F)]
			[InlineData(1.1F, null)]
			public async Task ForNullableFloat_WhenMinimumOrMaximumIsNull_ShouldSucceed(float? minimum, float? maximum)
			{
				float subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)2.0, (float)1.0, (float)3.0)]
			[InlineData((float)2.0, (float)2.0, (float)4.0)]
			[InlineData((float)2.0, (float)1.0, (float)2.0)]
			public async Task ForNullableFloat_WhenValueIsInRangeExpected_ShouldFail(
				float? subject, float? minimum, float? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((float)2.0, (float)0.0, (float)1.9)]
			[InlineData((float)0.0, (float)0.1, (float)2.0)]
			public async Task ForNullableFloat_WhenValueIsOutsideTheRange_ShouldSucceed(
				float? subject, float? minimum, float? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_WhenMaximumIsSmallerThanMinimum_ShouldThrowArgumentOutOfRangeException()
			{
				int? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(2).And(1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("maximum").And
					.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
			}

			[Theory]
			[InlineData(null, 1)]
			[InlineData(1, null)]
			public async Task ForNullableInt_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				int? minimum, int? maximum)
			{
				int? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2, 1, 3)]
			[InlineData(2, 2, 4)]
			[InlineData(2, 1, 2)]
			public async Task ForNullableInt_WhenValueIsInRangeExpected_ShouldFail(
				int? subject, int? minimum, int? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail()
			{
				int? subject = null;
				int minimum = 1;
				int maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData(2, 0, 1)]
			[InlineData(0, 1, 2)]
			public async Task ForNullableInt_WhenValueIsOutsideTheRange_ShouldSucceed(
				int? subject, int? minimum, int? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}
#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(null, 1)]
			[InlineData(1, null)]
			public async Task ForNullableInt128_WhenMinimumOrMaximumIsNull_ShouldSucceed(int? minimumValue,
				int? maximumValue)
			{
				Int128? subject = 2;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 1, 3)]
			[InlineData(2, 2, 4)]
			[InlineData(2, 1, 2)]
			public async Task ForNullableInt128_WhenValueIsInRangeExpected_ShouldFail(
				int? subjectValue, int? minimumValue, int? maximumValue)
			{
				Int128? subject = subjectValue;
				Int128? minimum = minimumValue;
				Int128? maximum = maximumValue;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 0, 1)]
			[InlineData(0, 1, 2)]
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

			[Theory]
			[InlineData(null, (long)1)]
			[InlineData((long)1, null)]
			public async Task ForNullableLong_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				long? minimum, long? maximum)
			{
				long? subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)2, (long)1, (long)3)]
			[InlineData((long)2, (long)2, (long)4)]
			[InlineData((long)2, (long)1, (long)2)]
			public async Task ForNullableLong_WhenValueIsInRangeExpected_ShouldFail(
				long? subject, long? minimum, long? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail()
			{
				long? subject = null;
				long minimum = 1;
				long maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((long)2, (long)0, (long)1)]
			[InlineData((long)0, (long)1, (long)2)]
			public async Task ForNullableLong_WhenValueIsOutsideTheRange_ShouldSucceed(
				long? subject, long? minimum, long? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)2, (sbyte)1, (sbyte)3)]
			[InlineData((sbyte)2, (sbyte)2, (sbyte)4)]
			[InlineData((sbyte)2, (sbyte)1, (sbyte)2)]
			public async Task ForNullableSbyte_WhenValueIsInRangeExpected_ShouldFail(
				sbyte? subject, sbyte? minimum, sbyte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail()
			{
				sbyte? subject = null;
				sbyte minimum = 1;
				sbyte maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((sbyte)2, (sbyte)0, (sbyte)1)]
			[InlineData((sbyte)0, (sbyte)1, (sbyte)2)]
			public async Task ForNullableSbyte_WhenValueIsOutsideTheRange_ShouldSucceed(
				sbyte? subject, sbyte? minimum, sbyte? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)2, (short)1, (short)3)]
			[InlineData((short)2, (short)2, (short)4)]
			[InlineData((short)2, (short)1, (short)2)]
			public async Task ForNullableShort_WhenValueIsInRangeExpected_ShouldFail(
				short? subject, short? minimum, short? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail()
			{
				short? subject = null;
				short minimum = 1;
				short maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((short)2, (short)0, (short)1)]
			[InlineData((short)0, (short)1, (short)2)]
			public async Task ForNullableShort_WhenValueIsOutsideTheRange_ShouldSucceed(
				short? subject, short? minimum, short? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)2, (uint)1, (uint)3)]
			[InlineData((uint)2, (uint)2, (uint)4)]
			[InlineData((uint)2, (uint)1, (uint)2)]
			public async Task ForNullableUint_WhenValueIsInRangeExpected_ShouldFail(
				uint? subject, uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail()
			{
				uint? subject = null;
				uint minimum = 1;
				uint maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((uint)2, (uint)0, (uint)1)]
			[InlineData((uint)0, (uint)1, (uint)2)]
			public async Task ForNullableUint_WhenValueIsOutsideTheRange_ShouldSucceed(
				uint? subject, uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)2, (ulong)1, (ulong)3)]
			[InlineData((ulong)2, (ulong)2, (ulong)4)]
			[InlineData((ulong)2, (ulong)1, (ulong)2)]
			public async Task ForNullableUlong_WhenValueIsInRangeExpected_ShouldFail(
				ulong? subject, ulong? minimum, ulong? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail()
			{
				ulong? subject = null;
				ulong minimum = 1;
				ulong maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((ulong)2, (ulong)0, (ulong)1)]
			[InlineData((ulong)0, (ulong)1, (ulong)2)]
			public async Task ForNullableUlong_WhenValueIsOutsideTheRange_ShouldSucceed(
				ulong? subject, ulong? minimum, ulong? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)2, (ushort)1, (ushort)3)]
			[InlineData((ushort)2, (ushort)2, (ushort)4)]
			[InlineData((ushort)2, (ushort)1, (ushort)2)]
			public async Task ForNullableUshort_WhenValueIsInRangeExpected_ShouldFail(
				ushort? subject, ushort? minimum, ushort? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail()
			{
				ushort? subject = null;
				ushort minimum = 1;
				ushort maximum = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {Formatter.Format(minimum)} and {Formatter.Format(maximum)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((ushort)2, (ushort)0, (ushort)1)]
			[InlineData((ushort)0, (ushort)1, (ushort)2)]
			public async Task ForNullableUshort_WhenValueIsOutsideTheRange_ShouldSucceed(
				ushort? subject, ushort? minimum, ushort? maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, (sbyte)1)]
			[InlineData((sbyte)1, null)]
			public async Task ForSbyte_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				sbyte? minimum, sbyte? maximum)
			{
				sbyte subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)2, (sbyte)1, (sbyte)3)]
			[InlineData((sbyte)2, (sbyte)2, (sbyte)4)]
			[InlineData((sbyte)2, (sbyte)1, (sbyte)2)]
			public async Task ForSbyte_WhenValueIsInRangeExpected_ShouldFail(sbyte subject,
				sbyte? minimum, sbyte maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((sbyte)2, (sbyte)0, (sbyte)1)]
			[InlineData((sbyte)0, (sbyte)1, (sbyte)2)]
			public async Task ForSbyte_WhenValueIsOutsideTheRange_ShouldSucceed(sbyte subject,
				sbyte? minimum, sbyte maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, (short)1)]
			[InlineData((short)1, null)]
			public async Task ForShort_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				short? minimum, short? maximum)
			{
				short subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)2, (short)1, (short)3)]
			[InlineData((short)2, (short)2, (short)4)]
			[InlineData((short)2, (short)1, (short)2)]
			public async Task ForShort_WhenValueIsInRangeExpected_ShouldFail(short subject,
				short? minimum, short maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((short)2, (short)0, (short)1)]
			[InlineData((short)0, (short)1, (short)2)]
			public async Task ForShort_WhenValueIsOutsideTheRange_ShouldSucceed(short subject,
				short? minimum, short maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, (uint)1)]
			[InlineData((uint)1, null)]
			public async Task ForUint_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				uint? minimum, uint? maximum)
			{
				uint subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)2, (uint)1, (uint)3)]
			[InlineData((uint)2, (uint)2, (uint)4)]
			[InlineData((uint)2, (uint)1, (uint)2)]
			public async Task ForUint_WhenValueIsInRangeExpected_ShouldFail(uint subject,
				uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((uint)2, (uint)0, (uint)1)]
			[InlineData((uint)0, (uint)1, (uint)2)]
			public async Task ForUint_WhenValueIsOutsideTheRange_ShouldSucceed(uint subject,
				uint? minimum, uint maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, (ulong)1)]
			[InlineData((ulong)1, null)]
			public async Task ForUlong_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				ulong? minimum, ulong? maximum)
			{
				ulong subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)2, (ulong)1, (ulong)3)]
			[InlineData((ulong)2, (ulong)2, (ulong)4)]
			[InlineData((ulong)2, (ulong)1, (ulong)2)]
			public async Task ForUlong_WhenValueIsInRangeExpected_ShouldFail(ulong subject,
				ulong? minimum, ulong maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((ulong)2, (ulong)0, (ulong)1)]
			[InlineData((ulong)0, (ulong)1, (ulong)2)]
			public async Task ForUlong_WhenValueIsOutsideTheRange_ShouldSucceed(ulong subject,
				ulong? minimum, ulong maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(null, (ushort)1)]
			[InlineData((ushort)1, null)]
			public async Task ForUshort_WhenMinimumOrMaximumIsNull_ShouldSucceed(
				ushort? minimum, ushort? maximum)
			{
				ushort subject = 2;

				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)2, (ushort)1, (ushort)3)]
			[InlineData((ushort)2, (ushort)2, (ushort)4)]
			[InlineData((ushort)2, (ushort)1, (ushort)2)]
			public async Task ForUshort_WhenValueIsInRangeExpected_ShouldFail(
				ushort subject,
				ushort? minimum, ushort maximum)
			{
				async Task Act()
					=> await That(subject).IsNotBetween(minimum).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData((ushort)2, (ushort)0, (ushort)1)]
			[InlineData((ushort)0, (ushort)1, (ushort)2)]
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
			[Theory]
			[InlineData(null, 1)]
			[InlineData(1, null)]
			public async Task ForInt_WhenMinimumOrMaximumIsNull_ShouldFail(
				int? minimum, int? maximum)
			{
				int subject = 2;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}


			[Theory]
			[InlineData(2, 1, 3)]
			[InlineData(2, 2, 4)]
			[InlineData(2, 1, 2)]
			public async Task ForInt_WhenValueIsInRangeExpected_ShouldSucceed(int subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2, 0, 1)]
			[InlineData(0, 1, 2)]
			public async Task ForInt_WhenValueIsOutsideTheRange_ShouldFail(int subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}

			[Theory]
			[InlineData(null, 1)]
			[InlineData(1, null)]
			public async Task ForNullableInt_WhenMinimumIsNull_ShouldFail(
				int? minimum, int? maximum)
			{
				int subject = 2;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}


			[Theory]
			[InlineData(2, 1, 3)]
			[InlineData(2, 2, 4)]
			[InlineData(2, 1, 2)]
			public async Task ForNullableInt_WhenValueIsInRangeExpected_ShouldSucceed(int? subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2, 0, 1)]
			[InlineData(0, 1, 2)]
			public async Task ForNullableInt_WhenValueIsOutsideTheRange_ShouldFail(int? subject,
				int? minimum, int maximum)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsNotBetween(minimum).And(maximum));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is between {ValueFormatters.Format(Formatter, minimum)} and {ValueFormatters.Format(Formatter, maximum)},
					              but it was {ValueFormatters.Format(Formatter, subject)}
					              """);
			}
		}
	}
}
