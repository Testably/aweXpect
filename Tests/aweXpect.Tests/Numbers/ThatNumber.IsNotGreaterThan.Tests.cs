#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class IsNotGreaterThan
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
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((byte)2, (byte)1, 1)]
			public async Task ForByte_WhenValueIsGreaterThanUnexpected_ShouldFail(byte subject,
				byte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((byte)1, (byte)2)]
			[Arguments((byte)0, (byte)0)]
			public async Task ForByte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(byte subject,
				byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForDecimal_WhenUnexpectedIsNull_ShouldFail(decimal subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, "1.0")]
			public async Task ForDecimal_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double subjectValue, double unexpectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal? unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.0, 2.1)]
			[Arguments(-3.03, 5.8)]
			[Arguments(0.0, 0.0)]
			public async Task ForDecimal_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double subjectValue, double unexpectedValue)
			{
				decimal subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double subject = double.NaN;
				double unexpected = 0.0;

				async Task Act() => await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than any value");
			}

			[Test]
			[Arguments(5.0, double.NegativeInfinity)]
			[Arguments(double.PositiveInfinity, 1.0)]
			[Arguments(double.PositiveInfinity, double.NegativeInfinity)]
			public async Task ForDouble_WhenSubjectOrUnexpectedIsInfinity_ShouldFail(
				double subject, double unexpected)
			{
				async Task Act() => await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
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

			[Test]
			[AutoArguments]
			public async Task ForDouble_WhenUnexpectedIsNull_ShouldFail(double subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.0, 1.0F, "1.0")]
			public async Task ForDouble_WhenUnexpectedIsSmallerFloat_ShouldFail(double subject,
				float unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, "1.0")]
			public async Task ForDouble_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double subject, double? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.0, 2.1)]
			[Arguments(-3.03, 5.8)]
			[Arguments(0.0, 0.0)]
			public async Task ForDouble_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForFloat_WhenSubjectIsNaN_ShouldSucceed()
			{
				float subject = float.NaN;
				float unexpected = 0.0f;

				async Task Act() => await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than any value");
			}

			[Test]
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

			[Test]
			[AutoArguments]
			public async Task ForFloat_WhenUnexpectedIsNull_ShouldFail(float subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.1, (float)1.1, "0.9999999")]
			public async Task ForFloat_WhenValueIsGreaterThanUnexpected_ShouldFail(
				float subject, float? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((float)1.0, (float)2.1)]
			[Arguments((float)-3.03, (float)5.8)]
			[Arguments((float)0.0, (float)0.0)]
			public async Task ForFloat_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				float subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
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
			[Test]
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

			[Test]
			[AutoArguments]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldFail(
				int subject)
			{
				int? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2, 1, 1)]
			public async Task ForInt_WhenValueIsGreaterThanUnexpected_ShouldFail(int subject,
				int? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(-2, -1)]
			[Arguments(0, 0)]
			public async Task ForInt_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(int subject,
				int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

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
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, 1)]
			public async Task ForInt128_WhenValueIsGreaterThanUnexpected_ShouldFail(
				int subjectValue, int unexpectedValue, int expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2)]
			[Arguments(0, 0)]
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

			[Test]
			[AutoArguments]
			public async Task ForLong_WhenUnexpectedIsNull_ShouldFail(
				long subject)
			{
				long? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2L, 1, 1)]
			public async Task ForLong_WhenUnexpectedIsSmallerInt_ShouldFail(long subject, int unexpected,
				int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)2, (long)1, 1)]
			public async Task ForLong_WhenValueIsGreaterThanUnexpected_ShouldFail(long subject,
				long? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)-2, (long)-1)]
			[Arguments((long)0, (long)0)]
			public async Task ForLong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(long subject,
				long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)2, (byte)1, 1)]
			public async Task ForNullableByte_WhenValueIsGreaterThanUnexpected_ShouldFail(
				byte? subject, byte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((byte)1, (byte)2)]
			[Arguments((byte)0, (byte)0)]
			public async Task ForNullableByte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				byte? subject, byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				byte? unexpected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableDecimal_WhenUnexpectedIsNull_ShouldFail(decimal? subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, "1.0")]
			public async Task ForNullableDecimal_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double? subjectValue, double? unexpectedValue, string expectedDifference)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected = unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			[Arguments(-3.03, 5.8)]
			[Arguments(0.0, 0.0)]
			public async Task ForNullableDecimal_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double? subjectValue, double? unexpectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected = unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				double? subject = double.NaN;
				double? unexpected = 0.0;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow()
					.Because("NaN is not greater than any value");
			}

			[Test]
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

			[Test]
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

			[Test]
			[AutoArguments]
			public async Task ForNullableDouble_WhenUnexpectedIsNull_ShouldFail(double? subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(2.1, 1.1, "1.0")]
			public async Task ForNullableDouble_WhenValueIsGreaterThanUnexpected_ShouldFail(
				double? subject, double? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(1.1, 2.1)]
			[Arguments(-3.03, 5.8)]
			[Arguments(0.0, 0.0)]
			public async Task ForNullableDouble_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
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

			[Test]
			[AutoArguments]
			public async Task ForNullableFloat_WhenUnexpectedIsNull_ShouldFail(float? subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((float)2.1, (float)1.1, "0.9999999")]
			public async Task ForNullableFloat_WhenValueIsGreaterThanUnexpected_ShouldFail(
				float? subject, float? unexpected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((float)1.1, (float)2.1)]
			[Arguments((float)-3.03, (float)5.8)]
			[Arguments((float)0.0, (float)0.0)]
			public async Task ForNullableFloat_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Test]
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

			[Test]
			[Arguments(2, 1, 1)]
			public async Task ForNullableInt_WhenValueIsGreaterThanUnexpected_ShouldFail(
				int? subject, int? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(-2, -1)]
			[Arguments(0, 0)]
			public async Task ForNullableInt_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				int? unexpected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
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
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(2, 1, 1)]
			public async Task ForNullableInt128_WhenValueIsGreaterThanUnexpected_ShouldFail(
				int subjectValue, int unexpectedValue, int expectedDifference)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(1, 2)]
			[Arguments(0, 0)]
			public async Task ForNullableInt128_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				int subjectValue, int unexpectedValue)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = unexpectedValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Test]
			[Arguments((long)2, (long)1, 1)]
			public async Task ForNullableLong_WhenValueIsGreaterThanUnexpected_ShouldFail(
				long? subject, long? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((long)-2, (long)-1)]
			[Arguments((long)0, (long)0)]
			public async Task ForNullableLong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				long? unexpected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1, 1)]
			public async Task ForNullableSbyte_WhenValueIsGreaterThanUnexpected_ShouldFail(
				sbyte? subject, sbyte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((sbyte)-2, (sbyte)-1)]
			[Arguments((sbyte)0, (sbyte)0)]
			public async Task ForNullableSbyte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				sbyte? unexpected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((short)2, (short)1, 1)]
			public async Task ForNullableShort_WhenValueIsGreaterThanUnexpected_ShouldFail(
				short? subject, short? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((short)-2, (short)-1)]
			[Arguments((short)0, (short)0)]
			public async Task ForNullableShort_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				short? unexpected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1, 1)]
			public async Task ForNullableUint_WhenValueIsGreaterThanUnexpected_ShouldFail(
				uint? subject, uint? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((uint)1, (uint)2)]
			[Arguments((uint)0, (uint)0)]
			public async Task ForNullableUint_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				uint? subject, uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				uint? unexpected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, 1)]
			public async Task ForNullableUlong_WhenValueIsGreaterThanUnexpected_ShouldFail(
				ulong? subject, ulong? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ulong)1, (ulong)2)]
			[Arguments((ulong)0, (ulong)0)]
			public async Task ForNullableUlong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				ulong? subject, ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				ulong? unexpected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was <null>
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, 1)]
			public async Task ForNullableUshort_WhenValueIsGreaterThanUnexpected_ShouldFail(
				ushort? subject, ushort? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ushort)1, (ushort)2)]
			[Arguments((ushort)0, (ushort)0)]
			public async Task ForNullableUshort_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(
				ushort? subject, ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				ushort? unexpected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
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
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((sbyte)2, (sbyte)1, 1)]
			public async Task ForSbyte_WhenValueIsGreaterThanUnexpected_ShouldFail(sbyte subject,
				sbyte? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((sbyte)-2, (sbyte)-1)]
			[Arguments((sbyte)0, (sbyte)0)]
			public async Task ForSbyte_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(sbyte subject,
				sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForShort_WhenUnexpectedIsNull_ShouldFail(
				short subject)
			{
				short? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((short)2, (short)1, 1)]
			public async Task ForShort_WhenValueIsGreaterThanUnexpected_ShouldFail(short subject,
				short? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((short)-2, (short)-1)]
			[Arguments((short)0, (short)0)]
			public async Task ForShort_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(short subject,
				short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUint_WhenUnexpectedIsNull_ShouldFail(
				uint subject)
			{
				uint? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((uint)2, (uint)1, 1)]
			public async Task ForUint_WhenValueIsGreaterThanUnexpected_ShouldFail(uint subject,
				uint? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((uint)1, (uint)2)]
			[Arguments((uint)0, (uint)0)]
			public async Task ForUint_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(uint subject,
				uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUlong_WhenUnexpectedIsNull_ShouldFail(
				ulong subject)
			{
				ulong? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ulong)2, (ulong)1, 1)]
			public async Task ForUlong_WhenValueIsGreaterThanUnexpected_ShouldFail(ulong subject,
				ulong? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ulong)1, (ulong)2)]
			[Arguments((ulong)0, (ulong)0)]
			public async Task ForUlong_WhenValueIsLessThanOrEqualToUnexpected_ShouldSucceed(ulong subject,
				ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task ForUshort_WhenUnexpectedIsNull_ShouldFail(
				ushort subject)
			{
				ushort? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments((ushort)2, (ushort)1, 1)]
			public async Task ForUshort_WhenValueIsGreaterThanUnexpected_ShouldFail(ushort subject,
				ushort? unexpected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ushort)1, (ushort)2)]
			[Arguments((ushort)0, (ushort)0)]
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
			[Test]
			public async Task ForDouble_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldFail()
			{
				double subject = double.MinValue;
				double unexpected = double.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(double.PositiveInfinity);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than double.MaxValue ± +∞,
					             but it was double.MinValue
					             """)
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForDouble_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldSucceed()
			{
				double subject = double.MinValue;
				double unexpected = double.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(double.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("a distance that is not representable exceeds every finite tolerance");
			}

			[Test]
			[Arguments(9.75, ", which differs by -0.25")]
			[Arguments(10.0, "")]
			public async Task ForDouble_WhenInsideTolerance_ShouldFail(double subject, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than 10.0 ± 0.5,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			public async Task ForDouble_WhenOnTheShiftedBound_ShouldSucceed()
			{
				double subject = 9.5;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow()
					.Because("subtracting the tolerance moves the strict bound of IsGreaterThan to 9.5, which is not greater than itself");
			}

			[Test]
			[Arguments(9.25)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldSucceed(double subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenSubjectAndUnexpectedArePositiveInfinity_ShouldSucceed()
			{
				double subject = double.PositiveInfinity;
				double unexpected = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(1.0);

				await That(Act).DoesNotThrow()
					.Because("a tolerance does not move an infinite bound, and infinity is not greater than itself");
			}

			[Test]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldSucceed()
			{
				async Task Act()
					=> await That(double.NaN).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow()
					.Because("no tolerance brings NaN within reach of a value");
			}

			[Test]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsNotGreaterThan(12.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			public async Task ForDouble_WhenUnexpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(5.0).IsNotGreaterThan(double.NaN).Within(1.0);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected value must not be NaN.").AsPrefix()
					.Because("no tolerance can bring a value within reach of NaN");
			}

			[Test]
			public async Task ForFloat_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldFail()
			{
				float subject = float.MinValue;
				float unexpected = float.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(float.PositiveInfinity);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than float.MaxValue ± +∞,
					             but it was float.MinValue, which differs by -6.80564693277058E+38
					             """)
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForFloat_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldSucceed()
			{
				float subject = float.MinValue;
				float unexpected = float.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(float.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("a distance that is not representable exceeds every finite tolerance");
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldFail()
			{
				Half subject = Half.MinValue;
				Half unexpected = Half.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(Half.PositiveInfinity);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than Half.MaxValue ± +∞,
					             but it was Half.MinValue, which differs by -131008.0
					             """)
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForHalf_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldSucceed()
			{
				Half subject = Half.MinValue;
				Half unexpected = Half.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(Half.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("a distance that is not representable exceeds every finite tolerance");
			}
#endif

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
					=> await That(subject).IsNotGreaterThan(10).Within(2));
				Exception? inverse = await Catch.ExceptionAsync(async ()
					=> await That(subject).DoesNotComplyWith(it => it.IsGreaterThan(10).Within(2)));

				await That(negation?.Message).IsEqualTo(inverse?.Message)
					.Because("the tolerance widens the unnegated expectation and so narrows its negation");
			}

			[Test]
			[Arguments(9, ", which differs by -1")]
			[Arguments(10, "")]
			[Arguments(15, ", which differs by 5")]
			public async Task ForInt_WhenInsideTolerance_ShouldFail(int subject, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not greater than 10 ± 2,
					              but it was {Formatter.Format(subject)}{expectedDifference}
					              """);
			}

			[Test]
			public async Task ForInt_WhenOneStepPastTheShiftedBound_ShouldFail()
			{
				int subject = 9;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than 10 ± 2,
					             but it was 9, which differs by -1
					             """)
					.Because("9 is greater than the shifted bound 8, so IsGreaterThan(10).Within(2) holds");
			}

			[Test]
			public async Task ForInt_WhenOnTheShiftedBound_ShouldSucceed()
			{
				int subject = 8;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).DoesNotThrow()
					.Because("subtracting the tolerance moves the strict bound of IsGreaterThan to 8, which is not greater than itself");
			}

			[Test]
			[Arguments(7)]
			[Arguments(0)]
			public async Task ForInt_WhenOutsideTolerance_ShouldSucceed(int subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForInt_WhenToleranceIsZeroAndValuesAreEqual_ShouldSucceed()
			{
				int subject = 10;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(0);

				await That(Act).DoesNotThrow()
					.Because("a zero tolerance keeps the strict inequality, like the form without a tolerance");
			}

			[Test]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldFail()
			{
				int? unexpected = null;

				async Task Act()
					=> await That(5).IsNotGreaterThan(unexpected).Within(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that 5
					             is not greater than <null> ± 2,
					             but it was 5
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNFloat_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldFail()
			{
				NFloat subject = NFloat.MinValue;
				NFloat unexpected = NFloat.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(NFloat.PositiveInfinity);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than NFloat.MaxValue ± +∞,
					             but it was NFloat.MinValue
					             """)
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForNFloat_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldSucceed()
			{
				NFloat subject = NFloat.MinValue;
				NFloat unexpected = NFloat.MaxValue;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected).Within(NFloat.MaxValue);

				await That(Act).DoesNotThrow()
					.Because("a distance that is not representable exceeds every finite tolerance");
			}
#endif

			[Test]
			[Arguments(9.25)]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldSucceed(double? subject)
			{
				async Task Act()
					=> await That(subject).IsNotGreaterThan(10.0).Within(0.5);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableInt_WhenOnTheShiftedBound_ShouldSucceed()
			{
				int? subject = 8;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).DoesNotThrow()
					.Because("subtracting the tolerance moves the strict bound of IsGreaterThan to 8, which is not greater than itself");
			}

			[Test]
			public async Task ForNullableInt_WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(10).Within(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than 10 ± 2,
					             but it was <null>
					             """);
			}
		}
	}
}
