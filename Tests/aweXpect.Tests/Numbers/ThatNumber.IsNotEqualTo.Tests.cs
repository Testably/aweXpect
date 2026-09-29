namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Theory]
			[AutoData]
			public async Task ForByte_WhenUnexpectedIsNull_ShouldSucceed(
				byte subject)
			{
				byte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)1, (byte)2)]
			[InlineData((byte)1, (byte)0)]
			public async Task ForByte_WhenValueIsDifferentFromUnexpected_ShouldSucceed(byte subject,
				byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)1, (byte)1)]
			public async Task ForByte_WhenValueIsEqualToUnexpected_ShouldFail(byte subject,
				byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForDecimal_WhenUnexpectedIsNull_ShouldSucceed(decimal subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(1.1, 0.1)]
			public async Task ForDecimal_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				double subjectValue, double unexpectedValue)
			{
				decimal subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForDecimal_WhenValueIsEqualToUnexpected_ShouldFail(
				double subjectValue, double unexpectedValue)
			{
				decimal subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenSubjectAndExpectedAreNaN_ShouldFail()
			{
				double subject = double.NaN;
				double expected = double.NaN;

				async Task Act() => await That(subject).IsNotEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to NaN,
					             but it was NaN
					             """);
			}

			[Theory]
			[InlineData(double.NaN, 0.0)]
			[InlineData(0.0, double.NaN)]
			public async Task ForDouble_WhenSubjectOrExpectedIsNaN_ShouldSucceed(
				double subject, double expected)
			{
				async Task Act() => await That(subject).IsNotEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(2.0, 2.0F)]
			public async Task ForDouble_WhenUnexpectedIsEqualFloat_ShouldFail(
				double subject, float unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForDouble_WhenUnexpectedIsNull_ShouldSucceed(double subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(1.1, 0.1)]
			public async Task ForDouble_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				double subject, double unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForDouble_WhenValueIsEqualToUnexpected_ShouldFail(
				double subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenSubjectAndExpectedAreNaN_ShouldFail()
			{
				float subject = float.NaN;
				float expected = float.NaN;

				async Task Act() => await That(subject).IsNotEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to NaN,
					             but it was NaN
					             """);
			}

			[Theory]
			[InlineData(float.NaN, 0.0)]
			[InlineData(0.0, float.NaN)]
			public async Task ForFloat_WhenSubjectOrExpectedIsNaN_ShouldSucceed(
				float subject, float expected)
			{
				async Task Act() => await That(subject).IsNotEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForFloat_WhenUnexpectedIsNull_ShouldSucceed(float subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1)]
			[InlineData((float)1.1, (float)0.1)]
			public async Task ForFloat_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				float subject, float unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)1.1)]
			public async Task ForFloat_WhenValueIsEqualToUnexpected_ShouldFail(
				float subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForInt_WhenUnexpectedIsNull_ShouldSucceed(
				int subject)
			{
				int? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, 2)]
			[InlineData(2, 1)]
			public async Task ForInt_WhenValueIsDifferentFromUnexpected_ShouldSucceed(int subject,
				int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, 1)]
			public async Task ForInt_WhenValueIsEqualToUnexpected_ShouldFail(int subject,
				int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Theory]
			[AutoData]
			public async Task ForInt128_WhenUnexpectedIsNull_ShouldSucceed(int subjectValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2)]
			[InlineData(2, 1)]
			public async Task ForInt128_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = expectedValue;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 1)]
			public async Task ForInt128_WhenValueIsEqualToUnexpected_ShouldFail(
				int subjectValue, int expectedValue)
			{
				Int128 subject = subjectValue;
				Int128? unexpected = expectedValue;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Theory]
			[InlineData(1L, 1)]
			public async Task ForLong_WhenUnexpectedIsEqualToInt_ShouldFail(
				long subject, int unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForLong_WhenUnexpectedIsNull_ShouldSucceed(
				long subject)
			{
				long? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)2)]
			[InlineData((long)1, (long)0)]
			public async Task ForLong_WhenValueIsDifferentFromUnexpected_ShouldSucceed(long subject,
				long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)1)]
			public async Task ForLong_WhenValueIsEqualToUnexpected_ShouldFail(long subject,
				long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData((byte)1, (byte)2)]
			[InlineData((byte)1, (byte)0)]
			public async Task ForNullableByte_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				byte? subject, byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)1, (byte)1)]
			public async Task ForNullableByte_WhenValueIsEqualToUnexpected_ShouldFail(
				byte? subject, byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableByte_WhenValueIsNull_ShouldSucceed(
				byte? unexpected)
			{
				byte? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDecimal_WhenUnexpectedIsNull_ShouldSucceed(decimal? subject)
			{
				decimal? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(1.1, 0.1)]
			public async Task ForNullableDecimal_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				double? subjectValue, double? unexpectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected =
					unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForNullableDecimal_WhenValueIsEqualToUnexpected_ShouldFail(
				double? subjectValue, double? unexpectedValue)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal? unexpected =
					unexpectedValue == null ? null : new decimal(unexpectedValue.Value);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableDouble_WhenUnexpectedIsNull_ShouldSucceed(double? subject)
			{
				double? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 2.1)]
			[InlineData(1.1, 0.1)]
			public async Task ForNullableDouble_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1.1, 1.1)]
			public async Task ForNullableDouble_WhenValueIsEqualToUnexpected_ShouldFail(
				double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableFloat_WhenUnexpectedIsNull_ShouldSucceed(float? subject)
			{
				float? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)2.1)]
			[InlineData((float)1.1, (float)0.1)]
			public async Task ForNullableFloat_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((float)1.1, (float)1.1)]
			public async Task ForNullableFloat_WhenValueIsEqualToUnexpected_ShouldFail(
				float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(1, 2)]
			[InlineData(1, 0)]
			public async Task ForNullableInt_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, 1)]
			public async Task ForNullableInt_WhenValueIsEqualToUnexpected_ShouldFail(
				int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableInt_WhenValueIsNull_ShouldSucceed(
				int? unexpected)
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Theory]
			[AutoData]
			public async Task ForNullableInt128_WhenUnexpectedIsNull_ShouldSucceed(int subjectValue)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 2)]
			[InlineData(2, 1)]
			public async Task ForNullableInt128_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				int subjectValue, int expectedValue)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = expectedValue;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(1, 1)]
			public async Task ForNullableInt128_WhenValueIsEqualToUnexpected_ShouldFail(
				int subjectValue, int expectedValue)
			{
				Int128? subject = subjectValue;
				Int128? unexpected = expectedValue;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Theory]
			[InlineData((long)1, (long)2)]
			[InlineData((long)1, (long)0)]
			public async Task ForNullableLong_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((long)1, (long)1)]
			public async Task ForNullableLong_WhenValueIsEqualToUnexpected_ShouldFail(
				long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableLong_WhenValueIsNull_ShouldSucceed(
				long? unexpected)
			{
				long? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2)]
			[InlineData((sbyte)1, (sbyte)0)]
			public async Task ForNullableSbyte_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)1)]
			public async Task ForNullableSbyte_WhenValueIsEqualToUnexpected_ShouldFail(
				sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableSbyte_WhenValueIsNull_ShouldSucceed(
				sbyte? unexpected)
			{
				sbyte? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)2)]
			[InlineData((short)1, (short)0)]
			public async Task ForNullableShort_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)1)]
			public async Task ForNullableShort_WhenValueIsEqualToUnexpected_ShouldFail(
				short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableShort_WhenValueIsNull_ShouldSucceed(
				short? unexpected)
			{
				short? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)2)]
			[InlineData((uint)1, (uint)0)]
			public async Task ForNullableUint_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				uint? subject, uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)1)]
			public async Task ForNullableUint_WhenValueIsEqualToUnexpected_ShouldFail(
				uint? subject, uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUint_WhenValueIsNull_ShouldSucceed(
				uint? unexpected)
			{
				uint? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2)]
			[InlineData((ulong)1, (ulong)0)]
			public async Task ForNullableUlong_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				ulong? subject, ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)1)]
			public async Task ForNullableUlong_WhenValueIsEqualToUnexpected_ShouldFail(
				ulong? subject, ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUlong_WhenValueIsNull_ShouldSucceed(
				ulong? unexpected)
			{
				ulong? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2)]
			[InlineData((ushort)1, (ushort)0)]
			public async Task ForNullableUshort_WhenValueIsDifferentFromUnexpected_ShouldSucceed(
				ushort? subject, ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)1)]
			public async Task ForNullableUshort_WhenValueIsEqualToUnexpected_ShouldFail(
				ushort? subject, ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForNullableUshort_WhenValueIsNull_ShouldSucceed(
				ushort? unexpected)
			{
				ushort? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task ForSbyte_WhenUnexpectedIsNull_ShouldSucceed(
				sbyte subject)
			{
				sbyte? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)2)]
			[InlineData((sbyte)1, (sbyte)0)]
			public async Task ForSbyte_WhenValueIsDifferentFromUnexpected_ShouldSucceed(sbyte subject,
				sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)1, (sbyte)1)]
			public async Task ForSbyte_WhenValueIsEqualToUnexpected_ShouldFail(sbyte subject,
				sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForShort_WhenUnexpectedIsNull_ShouldSucceed(
				short subject)
			{
				short? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)2)]
			[InlineData((short)1, (short)0)]
			public async Task ForShort_WhenValueIsDifferentFromUnexpected_ShouldSucceed(short subject,
				short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((short)1, (short)1)]
			public async Task ForShort_WhenValueIsEqualToUnexpected_ShouldFail(short subject,
				short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForUint_WhenUnexpectedIsNull_ShouldSucceed(
				uint subject)
			{
				uint? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)2)]
			[InlineData((uint)1, (uint)0)]
			public async Task ForUint_WhenValueIsDifferentFromUnexpected_ShouldSucceed(uint subject,
				uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)1, (uint)1)]
			public async Task ForUint_WhenValueIsEqualToUnexpected_ShouldFail(uint subject,
				uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForUlong_WhenUnexpectedIsNull_ShouldSucceed(
				ulong subject)
			{
				ulong? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)2)]
			[InlineData((ulong)1, (ulong)0)]
			public async Task ForUlong_WhenValueIsDifferentFromUnexpected_ShouldSucceed(ulong subject,
				ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)1, (ulong)1)]
			public async Task ForUlong_WhenValueIsEqualToUnexpected_ShouldFail(ulong subject,
				ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task ForUshort_WhenUnexpectedIsNull_ShouldSucceed(
				ushort subject)
			{
				ushort? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)2)]
			[InlineData((ushort)1, (ushort)0)]
			public async Task ForUshort_WhenValueIsDifferentFromUnexpected_ShouldSucceed(ushort subject,
				ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)1, (ushort)1)]
			public async Task ForUshort_WhenValueIsEqualToUnexpected_ShouldFail(ushort subject,
				ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class OverflowTests
		{
			[Fact]
			public async Task DecimalDifferenceOverflow_ShouldSucceed()
			{
				async Task Action()
					=> await That(decimal.MinValue).IsNotEqualTo(decimal.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task DoubleDifferenceOverflow_ShouldSucceed()
			{
				async Task Action()
					=> await That(double.MinValue).IsNotEqualTo(double.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task FloatDifferenceOverflow_ShouldSucceed()
			{
				async Task Action()
					=> await That(float.MinValue).IsNotEqualTo(float.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task IntDifferenceOverflow_ShouldSucceed()
			{
				async Task Action()
					=> await That(int.MinValue).IsNotEqualTo(int.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task LongDifferenceOverflow_ShouldSucceed()
			{
				async Task Action()
					=> await That(long.MinValue).IsNotEqualTo(long.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task NullableDecimalDifferenceOverflow_ShouldSucceed()
			{
				decimal? minValue = decimal.MinValue;

				async Task Action()
					=> await That(minValue).IsNotEqualTo(decimal.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task NullableDoubleDifferenceOverflow_ShouldSucceed()
			{
				double? minValue = double.MinValue;

				async Task Action()
					=> await That(minValue).IsNotEqualTo(double.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task NullableFloatDifferenceOverflow_ShouldSucceed()
			{
				float? minValue = float.MinValue;

				async Task Action()
					=> await That(minValue).IsNotEqualTo(float.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task NullableIntDifferenceOverflow_ShouldSucceed()
			{
				int? minValue = int.MinValue;

				async Task Action()
					=> await That(minValue).IsNotEqualTo(int.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task NullableLongDifferenceOverflow_ShouldSucceed()
			{
				long? minValue = long.MinValue;

				async Task Action()
					=> await That(minValue).IsNotEqualTo(long.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task NullableSbyteDifferenceOverflow_ShouldSucceed()
			{
				sbyte? minValue = sbyte.MinValue;

				async Task Action()
					=> await That(minValue).IsNotEqualTo(sbyte.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task NullableShortDifferenceOverflow_ShouldSucceed()
			{
				short? minValue = short.MinValue;

				async Task Action()
					=> await That(minValue).IsNotEqualTo(short.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task SbyteDifferenceOverflow_ShouldSucceed()
			{
				async Task Action()
					=> await That(sbyte.MinValue).IsNotEqualTo(sbyte.MaxValue);

				await That(Action).DoesNotThrow();
			}

			[Fact]
			public async Task ShortDifferenceOverflow_ShouldSucceed()
			{
				async Task Action()
					=> await That(short.MinValue).IsNotEqualTo(short.MaxValue);

				await That(Action).DoesNotThrow();
			}
		}
	}
}
