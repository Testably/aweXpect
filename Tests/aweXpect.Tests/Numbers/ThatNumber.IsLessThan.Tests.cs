#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsLessThan
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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((byte)2, (byte)1, ", which differs by 1")]
			[Arguments((byte)0, (byte)0, "")]
			public async Task ForByte_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(byte subject,
				byte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((byte)1, (byte)2)]
			public async Task ForByte_WhenValueIsLessThanExpected_ShouldSucceed(byte subject,
				byte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForDecimal_WhenExpectedIsNull_ShouldFail(decimal subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 1.1, ", which differs by 0.9")]
			[Arguments(3.03, -5.8, ", which differs by 8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForDecimal_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			public async Task ForDecimal_WhenValueIsLessThanExpected_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal? expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1.0, 2.0F)]
			public async Task ForDouble_WhenExpectedIsLargerFloat_ShouldSucceed(double subject,
				float expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 2.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double subject = double.NaN;
				double expected = 0.0;

				async Task Act() => await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("a NaN subject is a failing value, not a programming mistake");
			}

			[Test]
			[Arguments(2.0, 1.1, ", which differs by 0.9")]
			[Arguments(3.03, -5.8, ", which differs by 8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForDouble_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				double subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			public async Task ForDouble_WhenValueIsLessThanExpected_ShouldSucceed(
				double subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float subject = 2.0f;
				float expected = float.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldFail()
			{
				float subject = float.NaN;
				float expected = 0.0f;

				async Task Act() => await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("a NaN subject is a failing value, not a programming mistake");
			}

			[Test]
			[Arguments((float)2.0, (float)1.1, ", which differs by 0.9")]
			[Arguments((float)3.03, (float)-5.8, ", which differs by 8.83")]
			[Arguments((float)0.0, (float)0.0, "")]
			public async Task ForFloat_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				float subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((float)1.1, (float)2.1)]
			public async Task ForFloat_WhenValueIsLessThanExpected_ShouldSucceed(
				float subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half subject = (Half)2.0f;
				Half expected = Half.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForInt_WhenValueIsEqualToExpected_ShouldOmitTheDifference()
			{
				int subject = 5;

				async Task Act()
					=> await That(subject).IsLessThan(5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than 5,
					             but it was 5
					             """)
					.Because("a difference of zero tells nothing");
			}

			[Test]
			public async Task ForInt_WhenValueIsGreaterThanExpected_ShouldIncludeTheDifference()
			{
				int subject = 7;

				async Task Act()
					=> await That(subject).IsLessThan(5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than 5,
					             but it was 7, which differs by 2
					             """);
			}

			[Test]
			[Arguments(-1, -2, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForInt_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(int subject,
				int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1, 2)]
			public async Task ForInt_WhenValueIsLessThanExpected_ShouldSucceed(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			[AutoArguments]
			public async Task ForInt128_WhenExpectedIsNull_ShouldFail(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForInt128_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				int subjectValue, int expectedValue, string expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2)]
			public async Task ForInt128_WhenValueIsLessThanExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			[Arguments(1L, 2)]
			public async Task ForLong_WhenExpectedIsLargerInt_ShouldSucceed(long subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForLong_WhenExpectedIsNull_ShouldFail(
				long subject)
			{
				long? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((long)-1, (long)-2, ", which differs by 1")]
			[Arguments((long)0, (long)0, "")]
			public async Task ForLong_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(long subject,
				long? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)1, (long)2)]
			public async Task ForLong_WhenValueIsLessThanExpected_ShouldSucceed(long subject,
				long? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				NFloat subject = 2;
				NFloat expected = NFloat.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}

			[Test]
			public async Task ForNFloat_WhenSubjectIsNaN_ShouldFail()
			{
				NFloat subject = NFloat.NaN;
				NFloat expected = 1;

				async Task Act() => await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than 1.0,
					             but it was NaN
					             """)
					.Because("NaN sorts below every value, but is not less than any of them");
			}
#endif

			[Test]
			[Arguments((byte)2, (byte)1, ", which differs by 1")]
			[Arguments((byte)0, (byte)0, "")]
			public async Task ForNullableByte_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				byte? subject, byte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((byte)1, (byte)2)]
			public async Task ForNullableByte_WhenValueIsLessThanExpected_ShouldSucceed(
				byte? subject, byte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				byte? expected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableChar_WhenValueIsGreaterThanExpected_ShouldShowTheDifferenceAsNumber()
			{
				char? subject = 'z';

				async Task Act()
					=> await That(subject).IsLessThan('a');

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than 'a',
					             but it was 'z', which differs by 25
					             """)
					.Because("a difference formatted as char would be an unreadable character");
			}
#endif

			[Test]
			[AutoArguments]
			public async Task ForNullableDecimal_WhenExpectedIsNull_ShouldFail(decimal? subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, ", which differs by 1.0")]
			[Arguments(3.03, -5.8, ", which differs by 8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				double? subjectValue, double? expectedValue, string expectedDifference)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			public async Task ForNullableDecimal_WhenValueIsLessThanExpected_ShouldSucceed(
				double? subjectValue, double? expectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = 2.0;
				double? expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task ForNullableDouble_WhenSubjectIsNullAndExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = null;
				double? expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("the NaN expected value is rejected before the subject is considered");
			}

			[Test]
			[Arguments(2.1, 1.1, ", which differs by 1.0")]
			[Arguments(3.03, -5.8, ", which differs by 8.83")]
			[Arguments(0.0, 0.0, "")]
			public async Task ForNullableDouble_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				double? subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			public async Task ForNullableDouble_WhenValueIsLessThanExpected_ShouldSucceed(
				double? subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float? subject = 2.0f;
				float? expected = float.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.1, (float)1.1, ", which differs by 0.9999999")]
			[Arguments((float)3.03, (float)-5.8, ", which differs by 8.83")]
			[Arguments((float)0.0, (float)0.0, "")]
			public async Task ForNullableFloat_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				float? subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((float)1.1, (float)2.1)]
			public async Task ForNullableFloat_WhenValueIsLessThanExpected_ShouldSucceed(
				float? subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half? subject = (Half)2.0f;
				Half? expected = Half.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("an ordering comparison against NaN can never pass, so it is a programming mistake");
			}
#endif

			[Test]
			[Arguments(-1, -2, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForNullableInt_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				int? subject, int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1, 2)]
			public async Task ForNullableInt_WhenValueIsLessThanExpected_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				int? expected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, ", which differs by 1")]
			[Arguments(0, 0, "")]
			public async Task ForNullableInt128_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				int subjectValue, int expectedValue, string expectedDifference)
			{
				Int128? subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2)]
			public async Task ForNullableInt128_WhenValueIsLessThanExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128? subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			[Arguments((long)-1, (long)-2, ", which differs by 1")]
			[Arguments((long)0, (long)0, "")]
			public async Task ForNullableLong_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				long? subject, long? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)1, (long)2)]
			public async Task ForNullableLong_WhenValueIsLessThanExpected_ShouldSucceed(
				long? subject, long? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				long? expected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((sbyte)-1, (sbyte)-2, ", which differs by 1")]
			[Arguments((sbyte)0, (sbyte)0, "")]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				sbyte? subject, sbyte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((sbyte)1, (sbyte)2)]
			public async Task ForNullableSbyte_WhenValueIsLessThanExpected_ShouldSucceed(
				sbyte? subject, sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				sbyte? expected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((short)-1, (short)-2, ", which differs by 1")]
			[Arguments((short)0, (short)0, "")]
			public async Task ForNullableShort_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				short? subject, short? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((short)1, (short)2)]
			public async Task ForNullableShort_WhenValueIsLessThanExpected_ShouldSucceed(
				short? subject, short? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				short? expected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1, ", which differs by 1")]
			[Arguments((uint)0, (uint)0, "")]
			public async Task ForNullableUint_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				uint? subject, uint? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((uint)1, (uint)2)]
			public async Task ForNullableUint_WhenValueIsLessThanExpected_ShouldSucceed(
				uint? subject, uint? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				uint? expected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, ", which differs by 1")]
			[Arguments((ulong)0, (ulong)0, "")]
			public async Task ForNullableUlong_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				ulong? subject, ulong? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ulong)1, (ulong)2)]
			public async Task ForNullableUlong_WhenValueIsLessThanExpected_ShouldSucceed(
				ulong? subject, ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				ulong? expected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, ", which differs by 1")]
			[Arguments((ushort)0, (ushort)0, "")]
			public async Task ForNullableUshort_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				ushort? subject, ushort? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ushort)1, (ushort)2)]
			public async Task ForNullableUshort_WhenValueIsLessThanExpected_ShouldSucceed(
				ushort? subject, ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				ushort? expected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
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
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((sbyte)-1, (sbyte)-2, ", which differs by 1")]
			[Arguments((sbyte)0, (sbyte)0, "")]
			public async Task ForSbyte_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(sbyte subject,
				sbyte? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((sbyte)1, (sbyte)2)]
			public async Task ForSbyte_WhenValueIsLessThanExpected_ShouldSucceed(sbyte subject,
				sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForShort_WhenExpectedIsNull_ShouldFail(
				short subject)
			{
				short? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((short)-1, (short)-2, ", which differs by 1")]
			[Arguments((short)0, (short)0, "")]
			public async Task ForShort_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(short subject,
				short? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((short)1, (short)2)]
			public async Task ForShort_WhenValueIsLessThanExpected_ShouldSucceed(short subject,
				short? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUint_WhenExpectedIsNull_ShouldFail(
				uint subject)
			{
				uint? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1, ", which differs by 1")]
			[Arguments((uint)0, (uint)0, "")]
			public async Task ForUint_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(uint subject,
				uint? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((uint)1, (uint)2)]
			public async Task ForUint_WhenValueIsLessThanExpected_ShouldSucceed(uint subject,
				uint? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUlong_WhenExpectedIsNull_ShouldFail(
				ulong subject)
			{
				ulong? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, ", which differs by 1")]
			[Arguments((ulong)0, (ulong)0, "")]
			public async Task ForUlong_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(ulong subject,
				ulong? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ulong)1, (ulong)2)]
			public async Task ForUlong_WhenValueIsLessThanExpected_ShouldSucceed(ulong subject,
				ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUshort_WhenExpectedIsNull_ShouldFail(
				ushort subject)
			{
				ushort? expected = null;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, ", which differs by 1")]
			[Arguments((ushort)0, (ushort)0, "")]
			public async Task ForUshort_WhenValueIsGreaterThanOrEqualToExpected_ShouldFail(
				ushort subject,
				ushort? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ushort)1, (ushort)2)]
			public async Task ForUshort_WhenValueIsLessThanExpected_ShouldSucceed(ushort subject,
				ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).DoesNotThrow();
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
						=> it.IsLessThan(expected));

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
						=> it.IsLessThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}


			[Test]
			[Arguments(1, 2, ", which differs by -1")]
			public async Task ForInt_WhenValueIsLessThanExpected_ShouldFail(int subject,
				int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(-1, -2)]
			[Arguments(0, 0)]
			public async Task ForInt_WhenValueIsGreaterThanOrEqualToExpected_ShouldSucceed(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThan(expected));

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNFloat_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				NFloat subject = 2;
				NFloat expected = NFloat.NaN;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThan(expected));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("negating the expectation cannot make a NaN expected value meaningful");
			}

			[Test]
			public async Task ForNFloat_WhenSubjectIsNaN_ShouldSucceed()
			{
				NFloat subject = NFloat.NaN;
				NFloat expected = 1;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThan(expected));

				await That(Act).DoesNotThrow()
					.Because("NaN is not less than any value");
			}
#endif

			[Test]
			[AutoArguments]
			public async Task ForNullableInt_WhenExpectedIsNull_ShouldFail(
				int? subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than <null>,
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}


			[Test]
			[Arguments(1, 2, ", which differs by -1")]
			public async Task ForNullableInt_WhenValueIsLessThanExpected_ShouldFail(int? subject,
				int? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThan(expected));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not less than {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			[Arguments(-1, -2)]
			[Arguments(0, 0)]
			public async Task ForNullableInt_WhenValueIsGreaterThanOrEqualToExpected_ShouldSucceed(int? subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsLessThan(expected));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
