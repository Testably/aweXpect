namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotGreaterThan
	{
		public sealed class Tests
		{
			[Theory]
			[AutoData]
			public async Task ForByte_WhenUnexpectedIsNull_ShouldFail(
				byte subject)
			{
				byte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((byte)2, (byte)1, 1)]
			public async Task ForByte_WhenValueIsGreaterThanUnexpected_ShouldFail(byte subject,
				byte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2)]
			[InlineData((byte)0, (byte)0)]
			public async Task ForByte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(byte subject,
				byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForDecimal_WhenUnexpectedIsNull_ShouldFail(decimal subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2.1, 1.1, "1.0")]
			public async Task ForDecimal_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double subjectValue, double unexpectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal? unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.0, 2.1)]
			[InlineData(-3.03, 5.8)]
			[InlineData(0.0, 0.0)]
			public async Task ForDecimal_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double subjectValue, double unexpectedValue)
			{
				decimal subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double subject = double.NaN;
				double unexpected = 0.0;

				async Task Act() => await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than any value");
			}

			[Theory]
			[InlineData(5.0, double.NegativeInfinity)]
			[InlineData(double.PositiveInfinity, 1.0)]
			[InlineData(double.PositiveInfinity, double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectOrUnexpectedIsInfinity_ShouldFail(
				double subject, double unexpected)
			{
				async Task Act() => await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 2.0;
				double unexpected = double.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForDouble_WhenUnexpectedIsNull_ShouldFail(double subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2.0, 1.0F, "1.0")]
			public async Task ForDouble_WhenUnexpectedIsSmallerFloat_ShouldFail(double subject,
				float unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(2.1, 1.1, "1.0")]
			public async Task ForDouble_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double subject, double? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.0, 2.1)]
			[InlineData(-3.03, 5.8)]
			[InlineData(0.0, 0.0)]
			public async Task ForDouble_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldSucceed()
			{
				float subject = float.NaN;
				float unexpected = 0.0f;

				async Task Act() => await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than any value");
			}

			[Fact]
			public async Task ForFloat_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float subject = 2.0f;
				float unexpected = float.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForFloat_WhenUnexpectedIsNull_ShouldFail(float subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((float)2.1, (float)1.1, "0.9999999")]
			public async Task ForFloat_WhenValueIsGreaterThanUnexpected_ShouldFail(
				float subject, float? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((float)1.0, (float)2.1)]
			[InlineData((float)-3.03, (float)5.8)]
			[InlineData((float)0.0, (float)0.0)]
			public async Task ForFloat_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				float subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenSubjectIsNaN_ShouldSucceed()
			{
				Half subject = Half.NaN;
				Half unexpected = (Half)0.0f;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than any value");
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half subject = (Half)2.0f;
				Half unexpected = Half.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Theory]
			[AutoData]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldFail(
				int subject)
			{
				int? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2, 1, 1)]
			public async Task ForInt_WhenValueIsGreaterThanUnexpected_ShouldFail(int subject,
				int? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(-2, -1)]
			[InlineData(0, 0)]
			public async Task ForInt_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(int subject,
				int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Theory]
			[AutoData]
			public async Task ForInt128_WhenUnexpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 1, 1)]
			public async Task ForInt128_WhenValueIsGreaterThanUnexpected_ShouldFail(
				int subjectValue, int unexpectedValue, int expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2)]
			[InlineData(0, 0)]
			public async Task ForInt128_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				int subjectValue, int unexpectedValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[AutoData]
			public async Task ForLong_WhenUnexpectedIsNull_ShouldFail(
				long subject)
			{
				long? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2L, 1, 1)]
			public async Task ForLong_WhenUnexpectedIsSmallerInt_ShouldFail(long subject, int unexpected,
				int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((long)2, (long)1, 1)]
			public async Task ForLong_WhenValueIsGreaterThanUnexpected_ShouldFail(long subject,
				long? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((long)-2, (long)-1)]
			[InlineData((long)0, (long)0)]
			public async Task ForLong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(long subject,
				long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)2, (byte)1, 1)]
			public async Task ForNullableByte_WhenValueIsGreaterThanUnexpected_ShouldFail(
				byte? subject, byte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2)]
			[InlineData((byte)0, (byte)0)]
			public async Task ForNullableByte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				byte? subject, byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				byte? unexpected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDecimal_WhenUnexpectedIsNull_ShouldFail(decimal? subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2.1, 1.1, "1.0")]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double? subjectValue, double? unexpectedValue, string expectedDifference)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected = unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(-3.03, 5.8)]
			[InlineData(0.0, 0.0)]
			public async Task ForNullableDecimal_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double? subjectValue, double? unexpectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected = unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double? subject = double.NaN;
				double? unexpected = 0.0;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than any value");
			}

			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNullAndUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = null;
				double? unexpected = double.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("the NaN unexpected value is rejected before the subject is considered");
			}

			[Fact]
			public async Task ForNullableDouble_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = 2.0;
				double? unexpected = double.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDouble_WhenUnexpectedIsNull_ShouldFail(double? subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2.1, 1.1, "1.0")]
			public async Task ForNullableDouble_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double? subject, double? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(-3.03, 5.8)]
			[InlineData(0.0, 0.0)]
			public async Task ForNullableDouble_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableFloat_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float? subject = 2.0f;
				float? unexpected = float.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForNullableFloat_WhenUnexpectedIsNull_ShouldFail(float? subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((float)2.1, (float)1.1, "0.9999999")]
			public async Task ForNullableFloat_WhenValueIsGreaterThanUnexpected_ShouldFail(
				float? subject, float? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1)]
			[InlineData((float)-3.03, (float)5.8)]
			[InlineData((float)0.0, (float)0.0)]
			public async Task ForNullableFloat_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half? subject = (Half)2.0f;
				Half? unexpected = Half.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Theory]
			[InlineData(2, 1, 1)]
			public async Task ForNullableInt_WhenValueIsGreaterThanUnexpected_ShouldFail(
				int? subject, int? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(-2, -1)]
			[InlineData(0, 0)]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				int? unexpected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

#if NET8_0_OR_GREATER
			[Theory]
			[AutoData]
			public async Task ForNullableInt128_WhenUnexpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 1, 1)]
			public async Task ForNullableInt128_WhenValueIsGreaterThanUnexpected_ShouldFail(
				int subjectValue, int unexpectedValue, int expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2)]
			[InlineData(0, 0)]
			public async Task ForNullableInt128_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				int subjectValue, int unexpectedValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[InlineData((long)2, (long)1, 1)]
			public async Task ForNullableLong_WhenValueIsGreaterThanUnexpected_ShouldFail(
				long? subject, long? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((long)-2, (long)-1)]
			[InlineData((long)0, (long)0)]
			public async Task ForNullableLong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				long? unexpected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((sbyte)2, (sbyte)1, 1)]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanUnexpected_ShouldFail(
				sbyte? subject, sbyte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((sbyte)-2, (sbyte)-1)]
			[InlineData((sbyte)0, (sbyte)0)]
			public async Task ForNullableSbyte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				sbyte? unexpected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((short)2, (short)1, 1)]
			public async Task ForNullableShort_WhenValueIsGreaterThanUnexpected_ShouldFail(
				short? subject, short? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((short)-2, (short)-1)]
			[InlineData((short)0, (short)0)]
			public async Task ForNullableShort_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				short? unexpected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((uint)2, (uint)1, 1)]
			public async Task ForNullableUint_WhenValueIsGreaterThanUnexpected_ShouldFail(
				uint? subject, uint? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2)]
			[InlineData((uint)0, (uint)0)]
			public async Task ForNullableUint_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				uint? subject, uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				uint? unexpected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((ulong)2, (ulong)1, 1)]
			public async Task ForNullableUlong_WhenValueIsGreaterThanUnexpected_ShouldFail(
				ulong? subject, ulong? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2)]
			[InlineData((ulong)0, (ulong)0)]
			public async Task ForNullableUlong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				ulong? subject, ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				ulong? unexpected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((ushort)2, (ushort)1, 1)]
			public async Task ForNullableUshort_WhenValueIsGreaterThanUnexpected_ShouldFail(
				ushort? subject, ushort? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2)]
			[InlineData((ushort)0, (ushort)0)]
			public async Task ForNullableUshort_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				ushort? subject, ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				ushort? unexpected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForSbyte_WhenUnexpectedIsNull_ShouldFail(
				sbyte subject)
			{
				sbyte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((sbyte)2, (sbyte)1, 1)]
			public async Task ForSbyte_WhenValueIsGreaterThanUnexpected_ShouldFail(sbyte subject,
				sbyte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((sbyte)-2, (sbyte)-1)]
			[InlineData((sbyte)0, (sbyte)0)]
			public async Task ForSbyte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(sbyte subject,
				sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForShort_WhenUnexpectedIsNull_ShouldFail(
				short subject)
			{
				short? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((short)2, (short)1, 1)]
			public async Task ForShort_WhenValueIsGreaterThanUnexpected_ShouldFail(short subject,
				short? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((short)-2, (short)-1)]
			[InlineData((short)0, (short)0)]
			public async Task ForShort_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(short subject,
				short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUint_WhenUnexpectedIsNull_ShouldFail(
				uint subject)
			{
				uint? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((uint)2, (uint)1, 1)]
			public async Task ForUint_WhenValueIsGreaterThanUnexpected_ShouldFail(uint subject,
				uint? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2)]
			[InlineData((uint)0, (uint)0)]
			public async Task ForUint_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(uint subject,
				uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUlong_WhenUnexpectedIsNull_ShouldFail(
				ulong subject)
			{
				ulong? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ulong)2, (ulong)1, 1)]
			public async Task ForUlong_WhenValueIsGreaterThanUnexpected_ShouldFail(ulong subject,
				ulong? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2)]
			[InlineData((ulong)0, (ulong)0)]
			public async Task ForUlong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(ulong subject,
				ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUshort_WhenUnexpectedIsNull_ShouldFail(
				ushort subject)
			{
				ushort? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ushort)2, (ushort)1, 1)]
			public async Task ForUshort_WhenValueIsGreaterThanUnexpected_ShouldFail(ushort subject,
				ushort? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2)]
			[InlineData((ushort)0, (ushort)0)]
			public async Task ForUshort_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(ushort subject,
				ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithinTests
		{
			[Theory]
			[InlineData(9.75, ", which differs by -0.25")]
			[InlineData(10.0, "")]
			public async Task ForDouble_WhenInsideTolerance_ShouldFail(double subject, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than 10.0 ± 0.5,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenOnTheShiftedBound_ShouldSucceed()
			{
				double subject = 9.5;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow()
					.Because("subtracting the tolerance moves the strict bound of IsGreaterThan to 9.5, which is not greater than itself");
			}

			[Theory]
			[InlineData(9.25)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenSubjectAndUnexpectedArePositiveInfinity_ShouldSucceed()
			{
				double subject = double.PositiveInfinity;
				double unexpected = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(1.0);

				await That(Act).DoesNotThrow()
					.Because("a tolerance does not move an infinite bound, and infinity is not greater than itself");
			}

			[Fact]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				async Task Act()
					=> await That(double.NaN).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow()
					.Because("no tolerance brings NaN within reach of a value");
			}

			[Fact]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsNotGreaterThan(12.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Fact]
			public async Task ForDouble_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(5.0).IsNotGreaterThan(double.NaN).Within(1.0);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("no tolerance can bring a value within reach of NaN");
			}

			[Theory]
			[InlineData(5)]
			[InlineData(6)]
			[InlineData(7)]
			[InlineData(8)]
			[InlineData(9)]
			[InlineData(10)]
			[InlineData(11)]
			[InlineData(12)]
			[InlineData(13)]
			[InlineData(14)]
			[InlineData(15)]
			public async Task ForInt_ShouldBeTheExactInverseOfTheExpectation(int subject)
			{
				Exception? negation = await Record.ExceptionAsync(async ()
					=> await That(subject).IsNotGreaterThan(10).Within(2));
				Exception? inverse = await Record.ExceptionAsync(async ()
					=> await That(subject).DoesNotComplyWith(it => it.IsGreaterThan(10).Within(2)));

				await That(negation?.Message).IsEqualTo(inverse?.Message)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}

			[Theory]
			[InlineData(9, ", which differs by -1")]
			[InlineData(10, "")]
			[InlineData(15, ", which differs by 5")]
			public async Task ForInt_WhenInsideTolerance_ShouldFail(int subject, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than 10 ± 2,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Fact]
			public async Task ForInt_WhenOneStepPastTheShiftedBound_ShouldFail()
			{
				int subject = 9;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than 10 ± 2,
					             but it was 9, which differs by -1
					             """)
					.Because("9 is greater than the shifted bound 8, so IsGreaterThan(10).Within(2) holds");
			}

			[Fact]
			public async Task ForInt_WhenOnTheShiftedBound_ShouldSucceed()
			{
				int subject = 8;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).DoesNotThrow()
					.Because("subtracting the tolerance moves the strict bound of IsGreaterThan to 8, which is not greater than itself");
			}

			[Theory]
			[InlineData(7)]
			[InlineData(0)]
			public async Task ForInt_WhenOutsideTolerance_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForInt_WhenToleranceIsZeroAndValuesAreEqual_ShouldSucceed()
			{
				int subject = 10;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(0);

				await That(Act).DoesNotThrow()
					.Because("a zero tolerance keeps the strict inequality, like the form without a tolerance");
			}

			[Fact]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldFail()
			{
				int? unexpected = null;

				async Task Act()
					=> await That(5).IsNotGreaterThan(unexpected).Within(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that 5
					             is not greater than <null> ± 2,
					             but it was 5
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Theory]
			[InlineData(9.25)]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldSucceed(double? subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_WhenOnTheShiftedBound_ShouldSucceed()
			{
				int? subject = 8;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).DoesNotThrow()
					.Because("subtracting the tolerance moves the strict bound of IsGreaterThan to 8, which is not greater than itself");
			}

			[Fact]
			public async Task ForNullableInt_WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than 10 ± 2,
					             but it was <null>
					             """);
			}
		}
	}
}
