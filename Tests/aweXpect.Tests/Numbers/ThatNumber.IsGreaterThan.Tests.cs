#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsGreaterThan
	{
		public sealed class Tests
		{
			[Test]
			[AutoArguments]
			public async Task ForByte_WhenExpectedIsNull_ShouldFail(
				byte subject)
			{
				byte? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((byte)2, (byte)1)]
			public async Task ForByte_WhenValueIsGreaterThanExpected_ShouldSucceed(byte subject,
				byte? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)1, (byte)2, ", which differs by -1")]
			[Arguments((byte)0, (byte)0, "")]
			public async Task ForByte_WhenValueIsLessThanOrEqualToExpected_ShouldFail(byte subject,
				byte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForChar_WhenValueIsLessThanExpected_ShouldShowTheDifferenceAsNumber()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsGreaterThan('b');

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 'b',
					             but it was 'a', which differs by -1
					             """)
					.Because("a difference formatted as char would be an unreadable character");
			}
#endif

			[Test]
			public async Task ForDecimal_WhenDifferenceIsNotRepresentable_ShouldOmitTheDifference()
			{
				decimal subject = decimal.MinValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(decimal.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(decimal.MaxValue)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("the difference overflows decimal, which has no wider type");
			}

			[Test]
			[AutoArguments]
			public async Task ForDecimal_WhenExpectedIsNull_ShouldFail(decimal subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1)]
			public async Task ForDecimal_WhenValueIsGreaterThanExpected_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal? expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1.0, 2.1, ", which differs by -1.1")]
			[Arguments(-3.03, 5.8, ", which differs by -8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForDecimal_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 2.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForDouble_WhenExpectedIsNull_ShouldFail(double subject)
			{
				double? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 1.0F)]
			public async Task ForDouble_WhenExpectedIsSmallerFloat_ShouldSucceed(double subject,
				float expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double subject = double.NaN;
				double expected = 0.0;

				async Task Act() => await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("a NaN subject is a failing value, not a programming mistake");
			}

			[Test]
			[Arguments(5.0, double.NegativeInfinity)]
			[Arguments(double.PositiveInfinity, 1.0)]
			[Arguments(double.PositiveInfinity, double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectOrExpectedIsInfinity_ShouldSucceed(
				double subject, double expected)
			{
				async Task Act() => await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(2.1, 1.1)]
			public async Task ForDouble_WhenValueIsGreaterThanExpected_ShouldSucceed(
				double subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1.0, 2.1, ", which differs by -1.1")]
			[Arguments(-3.03, 5.8, ", which differs by -8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForDouble_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				double subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			public async Task ForFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float subject = 2.0f;
				float expected = float.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForFloat_WhenExpectedIsNull_ShouldFail(float subject)
			{
				float? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldFail()
			{
				float subject = float.NaN;
				float expected = 0.0f;

				async Task Act() => await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("a NaN subject is a failing value, not a programming mistake");
			}

			[Test]
			[Arguments((float)2.1, (float)1.1)]
			public async Task ForFloat_WhenValueIsGreaterThanExpected_ShouldSucceed(
				float subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((float)1.0, (float)2.1, ", which differs by -1.1")]
			[Arguments((float)-3.03, (float)5.8, ", which differs by -8.83")]
			[Arguments((float)0.0, (float)0.0, "")]
			public async Task ForFloat_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				float subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half subject = (Half)2.0f;
				Half expected = Half.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Test]
			[AutoArguments]
			public async Task ForInt_WhenExpectedIsNull_ShouldFail(
				int subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForInt_WhenValueIsEqualToExpected_ShouldOmitTheDifference()
			{
				int subject = 5;

				async Task Act()
					=> await That(subject).IsGreaterThan(5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5,
					             but it was 5
					             """)
					.Because("a difference of zero tells nothing");
			}

			[Test]
			[Arguments(2, 1)]
			public async Task ForInt_WhenValueIsGreaterThanExpected_ShouldSucceed(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForInt_WhenValueIsLessThanExpected_ShouldIncludeTheDifference()
			{
				int subject = 3;

				async Task Act()
					=> await That(subject).IsGreaterThan(5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5,
					             but it was 3, which differs by -2
					             """);
			}

			[Test]
			[Arguments(-2, -1, ", which differs by -1")]
			[Arguments(0, 0, "")]
			public async Task ForInt_WhenValueIsLessThanOrEqualToExpected_ShouldFail(int subject,
				int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			[AutoArguments]
			public async Task ForInt128_WhenExpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1)]
			public async Task ForInt128_WhenValueIsGreaterThanExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2, ", which differs by -1")]
			[Arguments(0, 0, "")]
			public async Task ForInt128_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				int subjectValue, int expectedValue, string expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

			[Test]
			[AutoArguments]
			public async Task ForLong_WhenExpectedIsNull_ShouldFail(
				long subject)
			{
				long? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2L, 1)]
			public async Task ForLong_WhenExpectedIsSmallerInt_ShouldSucceed(long subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((long)2, (long)1)]
			public async Task ForLong_WhenValueIsGreaterThanExpected_ShouldSucceed(long subject,
				long? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((long)-2, (long)-1, ", which differs by -1")]
			[Arguments((long)0, (long)0, "")]
			public async Task ForLong_WhenValueIsLessThanOrEqualToExpected_ShouldFail(long subject,
				long? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				NFloat subject = 1;
				NFloat expected = NFloat.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Test]
			[Arguments((byte)2, (byte)1)]
			public async Task ForNullableByte_WhenValueIsGreaterThanExpected_ShouldSucceed(
				byte? subject, byte? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)1, (byte)2, ", which differs by -1")]
			[Arguments((byte)0, (byte)0, "")]
			public async Task ForNullableByte_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				byte? subject, byte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				byte? expected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableDecimal_WhenExpectedIsNull_ShouldFail(decimal? subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1)]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanExpected_ShouldSucceed(
				double? subjectValue, double? expectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1.1, 2.1, ", which differs by -1.0")]
			[Arguments(-3.03, 5.8, ", which differs by -8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForNullableDecimal_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				double? subjectValue, double? expectedValue, string expectedDifference)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			public async Task ForNullableDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = 2.0;
				double? expected = double.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableDouble_WhenExpectedIsNull_ShouldFail(double? subject)
			{
				double? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableDouble_WhenSubjectIsNullAndExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = null;
				double? expected = double.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("the NaN expected value is rejected before the subject is considered");
			}

			[Test]
			[Arguments(2.1, 1.1)]
			public async Task ForNullableDouble_WhenValueIsGreaterThanExpected_ShouldSucceed(
				double? subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1.1, 2.1, ", which differs by -1.0")]
			[Arguments(-3.03, 5.8, ", which differs by -8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForNullableDouble_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				double? subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			public async Task ForNullableFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float? subject = 2.0f;
				float? expected = float.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableFloat_WhenExpectedIsNull_ShouldFail(float? subject)
			{
				float? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.1, (float)1.1)]
			public async Task ForNullableFloat_WhenValueIsGreaterThanExpected_ShouldSucceed(
				float? subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((float)1.1, (float)2.1, ", which differs by -0.9999999")]
			[Arguments((float)-3.03, (float)5.8, ", which differs by -8.83")]
			[Arguments((float)0.0, (float)0.0, "")]
			public async Task ForNullableFloat_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				float? subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half? subject = (Half)2.0f;
				Half? expected = Half.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Test]
			[Arguments(2, 1)]
			public async Task ForNullableInt_WhenValueIsGreaterThanExpected_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(-2, -1, ", which differs by -1")]
			[Arguments(0, 0, "")]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				int? subject, int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				int? expected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			[AutoArguments]
			public async Task ForNullableInt128_WhenExpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128? subject = subjectValue;
				Int128? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1)]
			public async Task ForNullableInt128_WhenValueIsGreaterThanExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128? subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2, ", which differs by -1")]
			[Arguments(0, 0, "")]
			public async Task ForNullableInt128_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				int subjectValue, int expectedValue, string expectedDifference)
			{
				Int128? subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

			[Test]
			[Arguments((long)2, (long)1)]
			public async Task ForNullableLong_WhenValueIsGreaterThanExpected_ShouldSucceed(
				long? subject, long? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((long)-2, (long)-1, ", which differs by -1")]
			[Arguments((long)0, (long)0, "")]
			public async Task ForNullableLong_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				long? subject, long? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				long? expected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1)]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanExpected_ShouldSucceed(
				sbyte? subject, sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((sbyte)-2, (sbyte)-1, ", which differs by -1")]
			[Arguments((sbyte)0, (sbyte)0, "")]
			public async Task ForNullableSbyte_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				sbyte? subject, sbyte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				sbyte? expected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((short)2, (short)1)]
			public async Task ForNullableShort_WhenValueIsGreaterThanExpected_ShouldSucceed(
				short? subject, short? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((short)-2, (short)-1, ", which differs by -1")]
			[Arguments((short)0, (short)0, "")]
			public async Task ForNullableShort_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				short? subject, short? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				short? expected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1)]
			public async Task ForNullableUint_WhenValueIsGreaterThanExpected_ShouldSucceed(
				uint? subject, uint? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((uint)1, (uint)2, ", which differs by -1")]
			[Arguments((uint)0, (uint)0, "")]
			public async Task ForNullableUint_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				uint? subject, uint? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				uint? expected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1)]
			public async Task ForNullableUlong_WhenValueIsGreaterThanExpected_ShouldSucceed(
				ulong? subject, ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((ulong)1, (ulong)2, ", which differs by -1")]
			[Arguments((ulong)0, (ulong)0, "")]
			public async Task ForNullableUlong_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				ulong? subject, ulong? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				ulong? expected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1)]
			public async Task ForNullableUshort_WhenValueIsGreaterThanExpected_ShouldSucceed(
				ushort? subject, ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((ushort)1, (ushort)2, ", which differs by -1")]
			[Arguments((ushort)0, (ushort)0, "")]
			public async Task ForNullableUshort_WhenValueIsLessThanOrEqualToExpected_ShouldFail(
				ushort? subject, ushort? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				ushort? expected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForSbyte_WhenExpectedIsNull_ShouldFail(
				sbyte subject)
			{
				sbyte? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1)]
			public async Task ForSbyte_WhenValueIsGreaterThanExpected_ShouldSucceed(sbyte subject,
				sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((sbyte)-2, (sbyte)-1, ", which differs by -1")]
			[Arguments((sbyte)0, (sbyte)0, "")]
			public async Task ForSbyte_WhenValueIsLessThanOrEqualToExpected_ShouldFail(sbyte subject,
				sbyte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForShort_WhenExpectedIsNull_ShouldFail(
				short subject)
			{
				short? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((short)2, (short)1)]
			public async Task ForShort_WhenValueIsGreaterThanExpected_ShouldSucceed(short subject,
				short? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((short)-2, (short)-1, ", which differs by -1")]
			[Arguments((short)0, (short)0, "")]
			public async Task ForShort_WhenValueIsLessThanOrEqualToExpected_ShouldFail(short subject,
				short? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForUint_WhenExpectedIsNull_ShouldFail(
				uint subject)
			{
				uint? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1)]
			public async Task ForUint_WhenValueIsGreaterThanExpected_ShouldSucceed(uint subject,
				uint? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((uint)1, (uint)2, ", which differs by -1")]
			[Arguments((uint)0, (uint)0, "")]
			public async Task ForUint_WhenValueIsLessThanOrEqualToExpected_ShouldFail(uint subject,
				uint? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForUlong_WhenExpectedIsNull_ShouldFail(
				ulong subject)
			{
				ulong? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1)]
			public async Task ForUlong_WhenValueIsGreaterThanExpected_ShouldSucceed(ulong subject,
				ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((ulong)1, (ulong)2, ", which differs by -1")]
			[Arguments((ulong)0, (ulong)0, "")]
			public async Task ForUlong_WhenValueIsLessThanOrEqualToExpected_ShouldFail(ulong subject,
				ulong? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForUshort_WhenExpectedIsNull_ShouldFail(
				ushort subject)
			{
				ushort? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1)]
			public async Task ForUshort_WhenValueIsGreaterThanExpected_ShouldSucceed(ushort subject,
				ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((ushort)1, (ushort)2, ", which differs by -1")]
			[Arguments((ushort)0, (ushort)0, "")]
			public async Task ForUshort_WhenValueIsLessThanOrEqualToExpected_ShouldFail(ushort subject,
				ushort? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 2.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsGreaterThan(expected));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("negating the expectation cannot make a NaN expected value meaningful");
			}

			[Test]
			[AutoArguments]
			public async Task ForInt_WhenExpectedIsNull_ShouldFail(
				int subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsGreaterThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}


			[Test]
			[Arguments(2, 1)]
			public async Task ForInt_WhenValueIsGreaterThanExpected_ShouldFail(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsGreaterThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by 1
					              """);
			}

			[Test]
			[Arguments(-2, -1)]
			[Arguments(0, 0)]
			public async Task ForInt_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsGreaterThan(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableInt_WhenExpectedIsNull_ShouldFail(
				int? subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsGreaterThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}


			[Test]
			[Arguments(2, 1)]
			public async Task ForNullableInt_WhenValueIsGreaterThanExpected_ShouldFail(int? subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsGreaterThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}, which differs by 1
					              """);
			}

			[Test]
			[Arguments(-2, -1)]
			[Arguments(0, 0)]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToExpected_ShouldSucceed(int? subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsGreaterThan(expected));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
