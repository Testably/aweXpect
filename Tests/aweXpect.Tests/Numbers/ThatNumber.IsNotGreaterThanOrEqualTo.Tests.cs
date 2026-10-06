namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotGreaterThanOrEqualTo
	{
		public sealed class Tests
		{
			[Test]
			[AutoArguments]
			public async Task ForByte_WhenUnexpectedIsNull_ShouldFail(
				byte subject)
			{
				byte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((byte)2, (byte)1, ", which differs by 1")]
			[Arguments((byte)0, (byte)0, "")]
			public async Task ForByte_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				byte subject,
				byte? unexpected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((byte)1, (byte)2)]
			public async Task ForByte_WhenValueIsLessThanUnexpected_ShouldSucceed(byte subject,
				byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForDecimal_WhenUnexpectedIsNull_ShouldFail(decimal subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, ", which differs by 1.0")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForDecimal_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				double subjectValue, double unexpectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal? unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.0, 2.1)]
			[Arguments(-3.03, 5.8)]
			public async Task ForDecimal_WhenValueIsLessThanUnexpected_ShouldSucceed(
				double subjectValue, double unexpectedValue)
			{
				decimal subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double subject = double.NaN;
				double unexpected = 0.0;

				async Task Act() => await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than or equal to any value");
			}

			[Test]
			public async Task ForDouble_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 2.0;
				double unexpected = double.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForDouble_WhenUnexpectedIsNull_ShouldFail(double subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 1.0F, "1.0")]
			public async Task ForDouble_WhenUnexpectedIsSmallerFloat_ShouldFail(double subject,
				float unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, ", which differs by 1.0")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForDouble_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				double subject, double? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.0, 2.1)]
			[Arguments(-3.03, 5.8)]
			public async Task ForDouble_WhenValueIsLessThanUnexpected_ShouldSucceed(
				double subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldSucceed()
			{
				float subject = float.NaN;
				float unexpected = 0.0f;

				async Task Act() => await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than or equal to any value");
			}

			[Test]
			public async Task ForFloat_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float subject = 2.0f;
				float unexpected = float.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForFloat_WhenUnexpectedIsNull_ShouldFail(float subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.1, (float)1.1, ", which differs by 0.9999999")]
			[Arguments((float)0.0, (float)0.0, "")]
			public async Task ForFloat_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				float subject, float? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((float)1.0, (float)2.1)]
			[Arguments((float)-3.03, (float)5.8)]
			public async Task ForFloat_WhenValueIsLessThanUnexpected_ShouldSucceed(
				float subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenSubjectIsNaN_ShouldSucceed()
			{
				Half subject = Half.NaN;
				Half unexpected = (Half)0.0f;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than or equal to any value");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half subject = (Half)2.0f;
				Half unexpected = Half.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Test]
			[AutoArguments]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldFail(
				int subject)
			{
				int? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2, 1, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForInt_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(int subject,
				int? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(-2, -1)]
			public async Task ForInt_WhenValueIsLessThanUnexpected_ShouldSucceed(int subject,
				int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			[AutoArguments]
			public async Task ForInt128_WhenUnexpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForInt128_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				int subjectValue, int unexpectedValue, string expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2)]
			public async Task ForInt128_WhenValueIsLessThanUnexpected_ShouldSucceed(
				int subjectValue, int unexpectedValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			[AutoArguments]
			public async Task ForLong_WhenUnexpectedIsNull_ShouldFail(
				long subject)
			{
				long? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2L, 1, 1)]
			public async Task ForLong_WhenUnexpectedIsSmallerInt_ShouldFail(long subject, int unexpected,
				int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)2, (long)1, ", which differs by 1")]
			[Arguments((long)0, (long)0, "")]
			public async Task ForLong_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				long subject,
				long? unexpected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)-2, (long)-1)]
			public async Task ForLong_WhenValueIsLessThanUnexpected_ShouldSucceed(long subject,
				long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)2, (byte)1, ", which differs by 1")]
			[Arguments((byte)0, (byte)0, "")]
			public async Task ForNullableByte_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				byte? subject, byte? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((byte)1, (byte)2)]
			public async Task ForNullableByte_WhenValueIsLessThanUnexpected_ShouldSucceed(
				byte? subject, byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				byte? unexpected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableDecimal_WhenUnexpectedIsNull_ShouldFail(decimal? subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, ", which differs by 1.0")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				double? subjectValue, double? unexpectedValue, string expectedDifference)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected = unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			[Arguments(-3.03, 5.8)]
			public async Task ForNullableDecimal_WhenValueIsLessThanUnexpected_ShouldSucceed(
				double? subjectValue, double? unexpectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected = unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double? subject = double.NaN;
				double? unexpected = 0.0;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than or equal to any value");
			}

			[Test]
			public async Task ForNullableDouble_WhenSubjectIsNullAndUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = null;
				double? unexpected = double.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("the NaN unexpected value is rejected before the subject is considered");
			}

			[Test]
			public async Task ForNullableDouble_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = 2.0;
				double? unexpected = double.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableDouble_WhenUnexpectedIsNull_ShouldFail(double? subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, ", which differs by 1.0")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForNullableDouble_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				double? subject, double? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			[Arguments(-3.03, 5.8)]
			public async Task ForNullableDouble_WhenValueIsLessThanUnexpected_ShouldSucceed(
				double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableFloat_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float? subject = 2.0f;
				float? unexpected = float.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableFloat_WhenUnexpectedIsNull_ShouldFail(float? subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.1, (float)1.1, ", which differs by 0.9999999")]
			[Arguments((float)0.0, (float)0.0, "")]
			public async Task ForNullableFloat_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				float? subject, float? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((float)1.1, (float)2.1)]
			[Arguments((float)-3.03, (float)5.8)]
			public async Task ForNullableFloat_WhenValueIsLessThanUnexpected_ShouldSucceed(
				float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half? subject = (Half)2.0f;
				Half? unexpected = Half.NaN;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Test]
			[Arguments(2, 1, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForNullableInt_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				int? subject, int? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(-2, -1)]
			public async Task ForNullableInt_WhenValueIsLessThanUnexpected_ShouldSucceed(
				int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				int? unexpected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			[AutoArguments]
			public async Task ForNullableInt128_WhenUnexpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForNullableInt128_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				int subjectValue, int unexpectedValue, string expectedDifference)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2)]
			public async Task ForNullableInt128_WhenValueIsLessThanUnexpected_ShouldSucceed(
				int subjectValue, int unexpectedValue)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			[Arguments((long)2, (long)1, ", which differs by 1")]
			[Arguments((long)0, (long)0, "")]
			public async Task ForNullableLong_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				long? subject, long? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)-2, (long)-1)]
			public async Task ForNullableLong_WhenValueIsLessThanUnexpected_ShouldSucceed(
				long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				long? unexpected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1, ", which differs by 1")]
			[Arguments((sbyte)0, (sbyte)0, "")]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				sbyte? subject, sbyte? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((sbyte)-2, (sbyte)-1)]
			public async Task ForNullableSbyte_WhenValueIsLessThanUnexpected_ShouldSucceed(
				sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				sbyte? unexpected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((short)2, (short)1, ", which differs by 1")]
			[Arguments((short)0, (short)0, "")]
			public async Task ForNullableShort_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				short? subject, short? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((short)-2, (short)-1)]
			public async Task ForNullableShort_WhenValueIsLessThanUnexpected_ShouldSucceed(
				short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				short? unexpected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1, ", which differs by 1")]
			[Arguments((uint)0, (uint)0, "")]
			public async Task ForNullableUint_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				uint? subject, uint? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((uint)1, (uint)2)]
			public async Task ForNullableUint_WhenValueIsLessThanUnexpected_ShouldSucceed(
				uint? subject, uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				uint? unexpected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, ", which differs by 1")]
			[Arguments((ulong)0, (ulong)0, "")]
			public async Task ForNullableUlong_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				ulong? subject, ulong? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ulong)1, (ulong)2)]
			public async Task ForNullableUlong_WhenValueIsLessThanUnexpected_ShouldSucceed(
				ulong? subject, ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				ulong? unexpected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, ", which differs by 1")]
			[Arguments((ushort)0, (ushort)0, "")]
			public async Task ForNullableUshort_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				ushort? subject, ushort? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ushort)1, (ushort)2)]
			public async Task ForNullableUshort_WhenValueIsLessThanUnexpected_ShouldSucceed(
				ushort? subject, ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				ushort? unexpected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForSbyte_WhenUnexpectedIsNull_ShouldFail(
				sbyte subject)
			{
				sbyte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1, ", which differs by 1")]
			[Arguments((sbyte)0, (sbyte)0, "")]
			public async Task ForSbyte_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				sbyte subject,
				sbyte? unexpected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((sbyte)-2, (sbyte)-1)]
			public async Task ForSbyte_WhenValueIsLessThanUnexpected_ShouldSucceed(sbyte subject,
				sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForShort_WhenUnexpectedIsNull_ShouldFail(
				short subject)
			{
				short? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((short)2, (short)1, ", which differs by 1")]
			[Arguments((short)0, (short)0, "")]
			public async Task ForShort_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				short subject,
				short? unexpected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((short)-2, (short)-1)]
			public async Task ForShort_WhenValueIsLessThanUnexpected_ShouldSucceed(short subject,
				short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUint_WhenUnexpectedIsNull_ShouldFail(
				uint subject)
			{
				uint? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1, ", which differs by 1")]
			[Arguments((uint)0, (uint)0, "")]
			public async Task ForUint_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				uint subject,
				uint? unexpected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((uint)1, (uint)2)]
			public async Task ForUint_WhenValueIsLessThanUnexpected_ShouldSucceed(uint subject,
				uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUlong_WhenUnexpectedIsNull_ShouldFail(
				ulong subject)
			{
				ulong? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, ", which differs by 1")]
			[Arguments((ulong)0, (ulong)0, "")]
			public async Task ForUlong_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				ulong subject,
				ulong? unexpected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ulong)1, (ulong)2)]
			public async Task ForUlong_WhenValueIsLessThanUnexpected_ShouldSucceed(ulong subject,
				ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUshort_WhenUnexpectedIsNull_ShouldFail(
				ushort subject)
			{
				ushort? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, ", which differs by 1")]
			[Arguments((ushort)0, (ushort)0, "")]
			public async Task ForUshort_WhenValueIsGreaterThanOrEqualToUnexpected_ShouldFail(
				ushort subject,
				ushort? unexpected,
				string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ushort)1, (ushort)2)]
			public async Task ForUshort_WhenValueIsLessThanUnexpected_ShouldSucceed(ushort subject,
				ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithinTests
		{
			[Test]
			[Arguments(9.5, ", which differs by -0.5")]
			[Arguments(9.75, ", which differs by -0.25")]
			[Arguments(10.0, "")]
			public async Task ForDouble_WhenInsideTolerance_ShouldFail(double subject, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(10.0).Within(0.5);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to 10.0 ± 0.5,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(9.25)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(10.0).Within(0.5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				async Task Act()
					=> await That(double.NaN).IsNotGreaterThanOrEqualTo(10.0).Within(0.5);

				await That(Act).DoesNotThrow()
					.Because("no tolerance brings NaN within reach of a value");
			}

			[Test]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsNotGreaterThanOrEqualTo(12.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			public async Task ForDouble_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(5.0).IsNotGreaterThanOrEqualTo(double.NaN).Within(1.0);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("no tolerance can bring a value within reach of NaN");
			}

			[Test]
			[Arguments(5)]
			[Arguments(6)]
			[Arguments(7)]
			[Arguments(8)]
			[Arguments(9)]
			[Arguments(10)]
			[Arguments(11)]
			[Arguments(12)]
			[Arguments(13)]
			[Arguments(14)]
			[Arguments(15)]
			public async Task ForInt_ShouldBeTheExactInverseOfTheExpectation(int subject)
			{
				Exception? negation = await Catch.ExceptionAsync(async ()
					=> await That(subject).IsNotGreaterThanOrEqualTo(10).Within(2));
				Exception? inverse = await Catch.ExceptionAsync(async ()
					=> await That(subject).DoesNotComplyWith(it => it.IsGreaterThanOrEqualTo(10).Within(2)));

				await That(negation?.Message).IsEqualTo(inverse?.Message)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}

			[Test]
			[Arguments(8, ", which differs by -2")]
			[Arguments(9, ", which differs by -1")]
			[Arguments(10, "")]
			[Arguments(15, ", which differs by 5")]
			public async Task ForInt_WhenInsideTolerance_ShouldFail(int subject, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(10).Within(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than or equal to 10 ± 2,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(7)]
			[Arguments(0)]
			public async Task ForInt_WhenOutsideTolerance_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(10).Within(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldFail()
			{
				int? unexpected = null;

				async Task Act()
					=> await That(5).IsNotGreaterThanOrEqualTo(unexpected).Within(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that 5
					             is not greater than or equal to <null> ± 2,
					             but it was 5
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			[Arguments(9.25)]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldSucceed(double? subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(10.0).Within(0.5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableInt_WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThanOrEqualTo(10).Within(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than or equal to 10 ± 2,
					             but it was <null>
					             """);
			}
		}
	}
}
