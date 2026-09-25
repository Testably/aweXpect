#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsEqualTo
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2, -1)]
			[InlineData((byte)1, (byte)0, 1)]
			public async Task ForByte_WhenValueIsDifferentFromExpected_ShouldFail(byte subject,
				byte? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)1)]
			public async Task ForByte_WhenValueIsEqualToExpected_ShouldSucceed(byte subject,
				byte? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForDecimal_WhenExpectedIsNull_ShouldFail(decimal subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1, "-1.0")]
			[InlineData(1.1, 0.2, "0.9")]
			public async Task ForDecimal_WhenValueIsDifferentFromExpected_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForDecimal_WhenValueIsEqualToExpected_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal? expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2.0, 2.0F)]
			public async Task ForDouble_WhenExpectedIsEqualFloat_ShouldSucceed(
				double subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForDouble_WhenExpectedIsNull_ShouldFail(double subject)
			{
				double? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				double subject = double.NaN;
				double expected = double.NaN;

				async Task Act() => await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.NaN, 0.0)]
			[InlineData(0.0, double.NaN)]
			public async Task ForDouble_WhenSubjectOrExpectedIsNaN_ShouldFail(double subject,
				double expected)
			{
				async Task Act() => await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1.1, 2.1, "-1.0")]
			[InlineData(1.1, 0.3, "0.8")]
			public async Task ForDouble_WhenValueIsDifferentFromExpected_ShouldFail(
				double subject, double expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForDouble_WhenValueIsEqualToExpected_ShouldSucceed(
				double subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForFloat_WhenExpectedIsNull_ShouldFail(float subject)
			{
				float? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				float subject = float.NaN;
				float expected = float.NaN;

				async Task Act() => await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.NaN, 0.0)]
			[InlineData(0.0, float.NaN)]
			public async Task ForFloat_WhenSubjectOrExpectedIsNaN_ShouldFail(float subject,
				float expected)
			{
				async Task Act() => await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)2.2, "-1.1")]
			[InlineData((float)1.1, (float)0.3, "0.8")]
			public async Task ForFloat_WhenValueIsDifferentFromExpected_ShouldFail(
				float subject, float expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)1.1)]
			public async Task ForFloat_WhenValueIsEqualToExpected_ShouldSucceed(
				float subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForInt_WhenExpectedIsNull_ShouldFail(
				int subject)
			{
				int? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1, 2, -1)]
			[InlineData(3, 1, 2)]
			[InlineData(int.MinValue, 0, int.MinValue)]
			public async Task ForInt_WhenValueIsDifferentFromExpected_ShouldFail(
				int subject, int? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1, 1)]
			public async Task ForInt_WhenValueIsEqualToExpected_ShouldSucceed(int subject,
				int? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2, -1)]
			[InlineData(2, 1, 1)]
			public async Task ForInt128_WhenValueIsDifferentFromExpected_ShouldFail(
				int subjectValue, int expectedValue, int expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 1)]
			public async Task ForInt128_WhenValueIsEqualToExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Theory]
			[InlineData(1L, 1)]
			public async Task ForLong_WhenExpectedIsEqualInt_ShouldSucceed(long subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForLong_WhenExpectedIsNull_ShouldFail(
				long subject)
			{
				long? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((long)1, (long)2, -1)]
			[InlineData((long)1, (long)0, 1)]
			public async Task ForLong_WhenValueIsDifferentFromExpected_ShouldFail(
				long subject, long? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((long)1, (long)1)]
			public async Task ForLong_WhenValueIsEqualToExpected_ShouldSucceed(long subject,
				long? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNFloat_WhenExpectedIsNaN_ShouldFailWithoutDifference()
			{
				NFloat subject = 0;
				NFloat expected = NFloat.NaN;

				async Task Act() => await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to NaN,
					             but it was 0
					             """)
					.Because("a difference to NaN is meaningless");
			}

			[Fact]
			public async Task ForNFloat_WhenSubjectIsNaN_ShouldFailWithoutDifference()
			{
				NFloat subject = NFloat.NaN;
				NFloat expected = 0;

				async Task Act() => await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0,
					             but it was NaN
					             """)
					.Because("a difference to NaN is meaningless");
			}
#endif

			[Fact]
			public async Task ForNullableByte_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				byte? subject = null;
				byte? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)1, (byte)2, -1)]
			[InlineData((byte)1, (byte)0, 1)]
			public async Task ForNullableByte_WhenValueIsDifferentFromExpected_ShouldFail(
				byte? subject, byte? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)1)]
			public async Task ForNullableByte_WhenValueIsEqualToExpected_ShouldSucceed(
				byte? subject, byte? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableByte_WhenValueIsNull_ShouldFail(
				byte? expected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDecimal_WhenExpectedIsNull_ShouldFail(decimal? subject)
			{
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDecimal_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				decimal? subject = null;
				decimal? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1, "-1.0")]
			[InlineData(1.1, 0.3, "0.8")]
			public async Task ForNullableDecimal_WhenValueIsDifferentFromExpected_ShouldFail(
				double? subjectValue, double? expectedValue, string expectedDifference)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForNullableDecimal_WhenValueIsEqualToExpected_ShouldSucceed(
				double? subjectValue, double? expectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? expected = expectedValue == null ? null : new decimal(expectedValue.Value);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDouble_WhenExpectedIsNull_ShouldFail(double? subject)
			{
				double? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableDouble_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				double? subject = null;
				double? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1, "-1.0")]
			[InlineData(1.1, 0.3, "0.8")]
			public async Task ForNullableDouble_WhenValueIsDifferentFromExpected_ShouldFail(
				double? subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForNullableDouble_WhenValueIsEqualToExpected_ShouldSucceed(
				double? subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableFloat_WhenExpectedIsNull_ShouldFail(float? subject)
			{
				float? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForNullableFloat_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				float? subject = null;
				float? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)2.2, "-1.1")]
			[InlineData((float)1.1, (float)0.2, "0.9")]
			public async Task ForNullableFloat_WhenValueIsDifferentFromExpected_ShouldFail(
				float? subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((float)1.1, (float)1.1)]
			public async Task ForNullableFloat_WhenValueIsEqualToExpected_ShouldSucceed(
				float? subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				int? subject = null;
				int? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, 2, -1)]
			[InlineData(1, 0, 1)]
			public async Task ForNullableInt_WhenValueIsDifferentFromExpected_ShouldFail(
				int? subject, int? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(1, 1)]
			public async Task ForNullableInt_WhenValueIsEqualToExpected_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableInt_WhenValueIsNull_ShouldFail(
				int? expected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNullableInt128_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				Int128? subject = null;
				Int128? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2, -1)]
			[InlineData(2, 1, 1)]
			public async Task ForNullableInt128_WhenValueIsDifferentFromExpected_ShouldFail(
				int subjectValue, int expectedValue, int expectedDifference)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 1)]
			public async Task ForNullableInt128_WhenValueIsEqualToExpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? expected = expectedValue;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
#endif

			[Fact]
			public async Task ForNullableLong_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				long? subject = null;
				long? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)2, -1)]
			[InlineData((long)1, (long)0, 1)]
			public async Task ForNullableLong_WhenValueIsDifferentFromExpected_ShouldFail(
				long? subject, long? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((long)1, (long)1)]
			public async Task ForNullableLong_WhenValueIsEqualToExpected_ShouldSucceed(
				long? subject, long? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableLong_WhenValueIsNull_ShouldFail(
				long? expected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableSbyte_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				sbyte? subject = null;
				sbyte? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2, -1)]
			[InlineData((sbyte)1, (sbyte)0, 1)]
			public async Task ForNullableSbyte_WhenValueIsDifferentFromExpected_ShouldFail(
				sbyte? subject, sbyte? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)1)]
			public async Task ForNullableSbyte_WhenValueIsEqualToExpected_ShouldSucceed(
				sbyte? subject, sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldFail(
				sbyte? expected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableShort_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				short? subject = null;
				short? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)2, -1)]
			[InlineData((short)1, (short)0, 1)]
			public async Task ForNullableShort_WhenValueIsDifferentFromExpected_ShouldFail(
				short? subject, short? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((short)1, (short)1)]
			public async Task ForNullableShort_WhenValueIsEqualToExpected_ShouldSucceed(
				short? subject, short? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableShort_WhenValueIsNull_ShouldFail(
				short? expected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableUint_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				uint? subject = null;
				uint? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)2, -1)]
			[InlineData((uint)1, (uint)0, 1)]
			public async Task ForNullableUint_WhenValueIsDifferentFromExpected_ShouldFail(
				uint? subject, uint? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)1)]
			public async Task ForNullableUint_WhenValueIsEqualToExpected_ShouldSucceed(
				uint? subject, uint? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUint_WhenValueIsNull_ShouldFail(
				uint? expected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableUlong_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				ulong? subject = null;
				ulong? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2, -1)]
			[InlineData((ulong)1, (ulong)0, 1)]
			public async Task ForNullableUlong_WhenValueIsDifferentFromExpected_ShouldFail(
				ulong? subject, ulong? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)1)]
			public async Task ForNullableUlong_WhenValueIsEqualToExpected_ShouldSucceed(
				ulong? subject, ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldFail(
				ulong? expected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task ForNullableUshort_WhenValueAndExpectedAreNull_ShouldSucceed()
			{
				ushort? subject = null;
				ushort? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2, -1)]
			[InlineData((ushort)1, (ushort)0, 1)]
			public async Task ForNullableUshort_WhenValueIsDifferentFromExpected_ShouldFail(
				ushort? subject, ushort? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)1)]
			public async Task ForNullableUshort_WhenValueIsEqualToExpected_ShouldSucceed(
				ushort? subject, ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldFail(
				ushort? expected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
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
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2, -1)]
			[InlineData((sbyte)1, (sbyte)0, 1)]
			[InlineData(sbyte.MinValue, (sbyte)0, -128)]
			public async Task ForSbyte_WhenValueIsDifferentFromExpected_ShouldFail(
				sbyte subject, sbyte? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)1)]
			public async Task ForSbyte_WhenValueIsEqualToExpected_ShouldSucceed(
				sbyte subject, sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForShort_WhenExpectedIsNull_ShouldFail(
				short subject)
			{
				short? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((short)1, (short)2, -1)]
			[InlineData((short)1, (short)0, 1)]
			[InlineData(short.MinValue, (short)0, -32768)]
			public async Task ForShort_WhenValueIsDifferentFromExpected_ShouldFail(
				short subject, short? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((short)1, (short)1)]
			public async Task ForShort_WhenValueIsEqualToExpected_ShouldSucceed(
				short subject, short? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUint_WhenExpectedIsNull_ShouldFail(
				uint subject)
			{
				uint? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)2, -1)]
			[InlineData((uint)1, (uint)0, 1)]
			public async Task ForUint_WhenValueIsDifferentFromExpected_ShouldFail(
				uint subject, uint? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((uint)1, (uint)1)]
			public async Task ForUint_WhenValueIsEqualToExpected_ShouldSucceed(
				uint subject, uint? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUlong_WhenExpectedIsNull_ShouldFail(
				ulong subject)
			{
				ulong? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2, -1)]
			[InlineData((ulong)1, (ulong)0, 1)]
			public async Task ForUlong_WhenValueIsDifferentFromExpected_ShouldFail(
				ulong subject, ulong? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ulong)1, (ulong)1)]
			public async Task ForUlong_WhenValueIsEqualToExpected_ShouldSucceed(
				ulong subject, ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForUshort_WhenExpectedIsNull_ShouldFail(
				ushort subject)
			{
				ushort? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to <null>,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2, -1)]
			[InlineData((ushort)1, (ushort)0, 1)]
			public async Task ForUshort_WhenValueIsDifferentFromExpected_ShouldFail(
				ushort subject, ushort? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData((ushort)1, (ushort)1)]
			public async Task ForUshort_WhenValueIsEqualToExpected_ShouldSucceed(
				ushort subject, ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OverflowTests
		{
			[Theory]
			[InlineData(byte.MinValue, byte.MaxValue, "-255")]
			[InlineData(byte.MaxValue, byte.MinValue, "255")]
			public async Task ForByte_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				byte subject, byte expected, string expectedDifference)
			{
				byte? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ForDecimal_WhenDifferenceIsNotRepresentable_ShouldOmitTheDifference(
				bool isSubjectMinValue)
			{
				decimal subject = isSubjectMinValue ? decimal.MinValue : decimal.MaxValue;
				decimal expected = isSubjectMinValue ? decimal.MaxValue : decimal.MinValue;
				decimal? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(double.MinValue, double.MaxValue)]
			[InlineData(double.MaxValue, double.MinValue)]
			public async Task ForDouble_WhenDifferenceIsNotRepresentable_ShouldOmitTheDifference(
				double subject, double expected)
			{
				double? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(float.MinValue, float.MaxValue, -6.805646932770577E+38)]
			[InlineData(float.MaxValue, float.MinValue, 6.805646932770577E+38)]
			public async Task ForFloat_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				float subject, float expected, double expectedDifference)
			{
				float? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {Formatter.Format(expectedDifference)}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {Formatter.Format(expectedDifference)}
					              """);
			}

			[Theory]
			[InlineData(float.PositiveInfinity, float.MinValue)]
			[InlineData(float.NegativeInfinity, float.MaxValue)]
			public async Task ForFloat_WhenDifferenceIsNotRepresentable_ShouldOmitTheDifference(
				float subject, float expected)
			{
				float? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(int.MinValue, int.MaxValue, "-4294967295")]
			[InlineData(int.MaxValue, int.MinValue, "4294967295")]
			public async Task ForInt_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				int subject, int expected, string expectedDifference)
			{
				int? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(long.MinValue, long.MaxValue, "-18446744073709551615")]
			[InlineData(long.MaxValue, long.MinValue, "18446744073709551615")]
			public async Task ForLong_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				long subject, long expected, string expectedDifference)
			{
				long? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(sbyte.MinValue, sbyte.MaxValue, "-255")]
			[InlineData(sbyte.MaxValue, sbyte.MinValue, "255")]
			[InlineData((sbyte)0, sbyte.MinValue, "128")]
			public async Task ForSbyte_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				sbyte subject, sbyte expected, string expectedDifference)
			{
				sbyte? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(short.MinValue, short.MaxValue, "-65535")]
			[InlineData(short.MaxValue, short.MinValue, "65535")]
			public async Task ForShort_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				short subject, short expected, string expectedDifference)
			{
				short? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(uint.MinValue, uint.MaxValue, "-4294967295")]
			[InlineData(uint.MaxValue, uint.MinValue, "4294967295")]
			public async Task ForUint_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				uint subject, uint expected, string expectedDifference)
			{
				uint? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(ulong.MinValue, ulong.MaxValue, "-18446744073709551615")]
			[InlineData(ulong.MaxValue, ulong.MinValue, "18446744073709551615")]
			public async Task ForUlong_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				ulong subject, ulong expected, string expectedDifference)
			{
				ulong? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

			[Theory]
			[InlineData(ushort.MinValue, ushort.MaxValue, "-65535")]
			[InlineData(ushort.MaxValue, ushort.MinValue, "65535")]
			public async Task ForUshort_WhenDifferenceOverflows_ShouldIncludeTheDifference(
				ushort subject, ushort expected, string expectedDifference)
			{
				ushort? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {expectedDifference}
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForHalf_WhenDifferenceOverflows_ShouldIncludeTheDifference()
			{
				Half subject = Half.MinValue;
				Half expected = Half.MaxValue;
				Half? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {Formatter.Format(-131008.0)}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by {Formatter.Format(-131008.0)}
					              """);
			}

			[Fact]
			public async Task ForNint_WhenDifferenceOverflows_ShouldIncludeTheDifference()
			{
				nint subject = nint.MinValue;
				nint expected = nint.MaxValue;
				nint? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by -{Formatter.Format(nuint.MaxValue)}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by -{Formatter.Format(nuint.MaxValue)}
					              """);
			}

			[Fact]
			public async Task ForNuint_WhenDifferenceOverflows_ShouldIncludeTheDifference()
			{
				nuint subject = nuint.MinValue;
				nuint expected = nuint.MaxValue;
				nuint? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by -{Formatter.Format(nuint.MaxValue)}
					              """);
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)} which differs by -{Formatter.Format(nuint.MaxValue)}
					              """);
			}
#endif
		}
	}
}
