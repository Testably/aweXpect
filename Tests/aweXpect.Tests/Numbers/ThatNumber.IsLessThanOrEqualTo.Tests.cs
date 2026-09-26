namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsLessThanOrEqualTo
	{
		public sealed class Tests
		{
			[Theory]
			[AutoData]
			public async Task ForByte_WhenExpectedIsNull_ShouldFail(
				byte subject)
			{
				byte? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((byte)2, (byte)1, ", which differs by 1")]
			public async Task ForByte_WhenValueIsGreaterThanExpected_ShouldFail(byte subject,
				byte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2)]
			[InlineData((byte)0, (byte)0)]
			public async Task ForByte_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(byte subject,
				byte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForDecimal_WhenExpectedIsNull_ShouldFail(decimal subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2.0, 1.1, ", which differs by 0.9")]
			[InlineData(3.03, -5.8, ", which differs by 8.83")]
			public async Task ForDecimal_WhenValueIsGreaterThanExpected_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(0.0, 0.0)]
			public async Task ForDecimal_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal? expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.0, 2.0F)]
			public async Task ForDouble_WhenExpectedIsLargerFloat_ShouldSucceed(double subject,
				float expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 2.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForDouble_WhenExpectedIsNull_ShouldFail(double subject)
			{
				double? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double subject = double.NaN;
				double expected = 0.0;

				async Task Act() => await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("a NaN subject is a failing value, not a programming mistake");
			}

			[Theory]
			[InlineData(2.0, 1.1, ", which differs by 0.9")]
			[InlineData(3.03, -5.8, ", which differs by 8.83")]
			public async Task ForDouble_WhenValueIsGreaterThanExpected_ShouldFail(
				double subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(0.0, 0.0)]
			public async Task ForDouble_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				double subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float subject = 2.0f;
				float expected = float.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForFloat_WhenExpectedIsNull_ShouldFail(float subject)
			{
				float? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldFail()
			{
				float subject = float.NaN;
				float expected = 0.0f;

				async Task Act() => await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("a NaN subject is a failing value, not a programming mistake");
			}

			[Theory]
			[InlineData((float)2.0, (float)1.1, ", which differs by 0.9")]
			[InlineData((float)3.03, (float)-5.8, ", which differs by 8.83")]
			public async Task ForFloat_WhenValueIsGreaterThanExpected_ShouldFail(
				float subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1)]
			[InlineData((float)0.0, (float)0.0)]
			public async Task ForFloat_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				float subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half subject = (Half)2.0f;
				Half expected = Half.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Theory]
			[AutoData]
			public async Task ForInt_WhenExpectedIsNull_ShouldFail(
				int subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(-1, -2)]
			public async Task ForInt_WhenValueIsGreaterThanExpected_ShouldFail(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by {Formatter.Format(subject - expected)}
					              """);
			}

			[Theory]
			[InlineData(1, 2)]
			[InlineData(0, 0)]
			public async Task ForInt_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Theory]
			[AutoData]
			public async Task ForInt128_WhenExpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 1, ", which differs by 1")]
			public async Task ForInt128_WhenValueIsGreaterThanExpected_ShouldFail(
				int subjectValue, int expectedValue, string expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2)]
			[InlineData(0, 0)]
			public async Task ForInt128_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[InlineData(1L, 2)]
			public async Task ForLong_WhenExpectedIsLargerInt_ShouldSucceed(long subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForLong_WhenExpectedIsNull_ShouldFail(
				long subject)
			{
				long? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((long)-1, (long)-2, ", which differs by 1")]
			public async Task ForLong_WhenValueIsGreaterThanExpected_ShouldFail(long subject,
				long? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((long)1, (long)2)]
			[InlineData((long)0, (long)0)]
			public async Task ForLong_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(long subject,
				long? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)2, (byte)1, ", which differs by 1")]
			public async Task ForNullableByte_WhenValueIsGreaterThanExpected_ShouldFail(
				byte? subject, byte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2)]
			[InlineData((byte)0, (byte)0)]
			public async Task ForNullableByte_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				byte? subject, byte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				byte? expected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDecimal_WhenExpectedIsNull_ShouldFail(decimal? subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(2.1, 1.1, ", which differs by 1.0")]
			[InlineData(3.03, -5.8, ", which differs by 8.83")]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanExpected_ShouldFail(
				double? subjectValue, double? expectedValue, string expectedDifference)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(0.0, 0.0)]
			public async Task ForNullableDecimal_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				double? subjectValue, double? expectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = 2.0;
				double? expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDouble_WhenExpectedIsNull_ShouldFail(double? subject)
			{
				double? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenSubjectIsNullAndExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = null;
				double? expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("the NaN expected value is rejected before the subject is considered");
			}

			[Theory]
			[InlineData(2.1, 1.1, ", which differs by 1.0")]
			[InlineData(3.03, -5.8, ", which differs by 8.83")]
			public async Task ForNullableDouble_WhenValueIsGreaterThanExpected_ShouldFail(
				double? subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(0.0, 0.0)]
			public async Task ForNullableDouble_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				double? subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float? subject = 2.0f;
				float? expected = float.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Theory]
			[AutoData]
			public async Task ForNullableFloat_WhenExpectedIsNull_ShouldFail(float? subject)
			{
				float? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((float)2.1, (float)1.1, ", which differs by 0.9999999")]
			[InlineData((float)3.03, (float)-5.8, ", which differs by 8.83")]
			public async Task ForNullableFloat_WhenValueIsGreaterThanExpected_ShouldFail(
				float? subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1)]
			[InlineData((float)0.0, (float)0.0)]
			public async Task ForNullableFloat_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				float? subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableHalf_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half? subject = (Half)2.0f;
				Half? expected = Half.NaN;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Theory]
			[InlineData(-1, -2, ", which differs by 1")]
			public async Task ForNullableInt_WhenValueIsGreaterThanExpected_ShouldFail(
				int? subject, int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1, 2)]
			[InlineData(0, 0)]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				int? expected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}
#if NET8_0_OR_GREATER
			[Theory]
			[AutoData]
			public async Task ForNullableInt128_WhenExpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(2, 1, ", which differs by 1")]
			public async Task ForNullableInt128_WhenValueIsGreaterThanExpected_ShouldFail(
				int subjectValue, int expectedValue, string expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2)]
			[InlineData(0, 0)]
			public async Task ForNullableInt128_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[InlineData((long)-1, (long)-2, ", which differs by 1")]
			public async Task ForNullableLong_WhenValueIsGreaterThanExpected_ShouldFail(
				long? subject, long? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((long)1, (long)2)]
			[InlineData((long)0, (long)0)]
			public async Task ForNullableLong_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				long? subject, long? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				long? expected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((sbyte)-1, (sbyte)-2, ", which differs by 1")]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanExpected_ShouldFail(
				sbyte? subject, sbyte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2)]
			[InlineData((sbyte)0, (sbyte)0)]
			public async Task ForNullableSbyte_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				sbyte? subject, sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				sbyte? expected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((short)-1, (short)-2, ", which differs by 1")]
			public async Task ForNullableShort_WhenValueIsGreaterThanExpected_ShouldFail(
				short? subject, short? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((short)1, (short)2)]
			[InlineData((short)0, (short)0)]
			public async Task ForNullableShort_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				short? subject, short? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				short? expected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((uint)2, (uint)1, ", which differs by 1")]
			public async Task ForNullableUint_WhenValueIsGreaterThanExpected_ShouldFail(
				uint? subject, uint? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2)]
			[InlineData((uint)0, (uint)0)]
			public async Task ForNullableUint_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				uint? subject, uint? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				uint? expected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((ulong)2, (ulong)1, ", which differs by 1")]
			public async Task ForNullableUlong_WhenValueIsGreaterThanExpected_ShouldFail(
				ulong? subject, ulong? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2)]
			[InlineData((ulong)0, (ulong)0)]
			public async Task ForNullableUlong_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				ulong? subject, ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				ulong? expected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[InlineData((ushort)2, (ushort)1, ", which differs by 1")]
			public async Task ForNullableUshort_WhenValueIsGreaterThanExpected_ShouldFail(
				ushort? subject, ushort? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2)]
			[InlineData((ushort)0, (ushort)0)]
			public async Task ForNullableUshort_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				ushort? subject, ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				ushort? expected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForSbyte_WhenExpectedIsNull_ShouldFail(
				sbyte subject)
			{
				sbyte? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((sbyte)-1, (sbyte)-2, ", which differs by 1")]
			public async Task ForSbyte_WhenValueIsGreaterThanExpected_ShouldFail(sbyte subject,
				sbyte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2)]
			[InlineData((sbyte)0, (sbyte)0)]
			public async Task ForSbyte_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(sbyte subject,
				sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForShort_WhenExpectedIsNull_ShouldFail(
				short subject)
			{
				short? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((short)-1, (short)-2, ", which differs by 1")]
			public async Task ForShort_WhenValueIsGreaterThanExpected_ShouldFail(short subject,
				short? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((short)1, (short)2)]
			[InlineData((short)0, (short)0)]
			public async Task ForShort_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(short subject,
				short? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUint_WhenExpectedIsNull_ShouldFail(
				uint subject)
			{
				uint? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((uint)2, (uint)1, ", which differs by 1")]
			public async Task ForUint_WhenValueIsGreaterThanExpected_ShouldFail(uint subject,
				uint? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2)]
			[InlineData((uint)0, (uint)0)]
			public async Task ForUint_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(uint subject,
				uint? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUlong_WhenExpectedIsNull_ShouldFail(
				ulong subject)
			{
				ulong? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ulong)2, (ulong)1, ", which differs by 1")]
			public async Task ForUlong_WhenValueIsGreaterThanExpected_ShouldFail(ulong subject,
				ulong? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2)]
			[InlineData((ulong)0, (ulong)0)]
			public async Task ForUlong_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(ulong subject,
				ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUshort_WhenExpectedIsNull_ShouldFail(
				ushort subject)
			{
				ushort? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ushort)2, (ushort)1, ", which differs by 1")]
			public async Task ForUshort_WhenValueIsGreaterThanExpected_ShouldFail(ushort subject,
				ushort? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2)]
			[InlineData((ushort)0, (ushort)0)]
			public async Task ForUshort_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(
				ushort subject,
				ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 2.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThanOrEqualTo(expected));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("negating the expectation cannot make a NaN expected value meaningful");
			}

			[Theory]
			[AutoData]
			public async Task ForInt_WhenExpectedIsNull_ShouldFail(
				int subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThanOrEqualTo(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}


			[Theory]
			[InlineData(1, 2, ", which differs by -1")]
			[InlineData(0, 0, "")]
			public async Task ForInt_WhenValueIsLessThanOrEqualToExpected_ShouldFail(int subject,
				int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThanOrEqualTo(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(-1, -2)]
			public async Task ForInt_WhenValueIsGreaterThanExpected_ShouldSucceed(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThanOrEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableInt_WhenExpectedIsNull_ShouldFail(
				int? subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThanOrEqualTo(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}


			[Theory]
			[InlineData(1, 2, ", which differs by -1")]
			[InlineData(0, 0, "")]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToExpected_ShouldFail(int? subject,
				int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThanOrEqualTo(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than or equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(-1, -2)]
			public async Task ForNullableInt_WhenValueIsGreaterThanExpected_ShouldSucceed(int? subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThanOrEqualTo(expected));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
