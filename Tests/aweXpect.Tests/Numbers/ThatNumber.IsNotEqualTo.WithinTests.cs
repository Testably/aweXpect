namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class WithinTests
		{
			[Theory]
			[InlineData((byte)5, (byte)6)]
			[InlineData((byte)5, (byte)4)]
			public async Task ForByte_WhenInsideTolerance_ShouldFail(
				byte subject, byte unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((byte)5, (byte)7)]
			[InlineData((byte)5, (byte)3)]
			public async Task ForByte_WhenOutsideTolerance_ShouldSucceed(
				byte subject, byte unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(12.5, 12.6)]
			[InlineData(12.5, 12.4)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldFail(
				double subjectValue, double unexpectedValue)
			{
				decimal subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.7)]
			[InlineData(12.5, 12.3)]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldSucceed(
				double subjectValue, double unexpectedValue)
			{
				decimal subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, decimal unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.6)]
			[InlineData(12.5, 12.4)]
			public async Task ForDouble_WhenInsideTolerance_ShouldFail(
				double subject, double unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.7)]
			[InlineData(12.5, 12.3)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldSucceed(
				double subject, double unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsNotEqualTo(12.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("Tolerance must not be NaN*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Theory]
			[AutoData]
			public async Task
				ForDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double subject, double unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5F, 12.6F)]
			[InlineData(12.5F, 12.4F)]
			public async Task ForFloat_WhenInsideTolerance_ShouldFail(
				float subject, float unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11F);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5F, 12.7F)]
			[InlineData(12.5F, 12.3F)]
			public async Task ForFloat_WhenOutsideTolerance_ShouldSucceed(
				float subject, float unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float subject, float unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5, 6)]
			[InlineData(5, 4)]
			public async Task ForInt_WhenInsideTolerance_ShouldFail(
				int subject, int unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 7)]
			[InlineData(5, 3)]
			public async Task ForInt_WhenOutsideTolerance_ShouldSucceed(
				int subject, int unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, int unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5L, 6L)]
			[InlineData(5L, 4L)]
			public async Task ForLong_WhenInsideTolerance_ShouldFail(
				long subject, long unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5L, 7L)]
			[InlineData(5L, 3L)]
			public async Task ForLong_WhenOutsideTolerance_ShouldSucceed(
				long subject, long unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long subject, long unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((byte)5, (byte)6)]
			[InlineData((byte)5, (byte)4)]
			public async Task ForNullableByte_WhenInsideTolerance_ShouldFail(
				byte? subject, byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((byte)5, (byte)7)]
			[InlineData((byte)5, (byte)3)]
			public async Task ForNullableByte_WhenOutsideTolerance_ShouldSucceed(
				byte? subject, byte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(12.5, 12.6)]
			[InlineData(12.5, 12.4)]
			public async Task ForNullableDecimal_WhenInsideTolerance_ShouldFail(
				double subjectValue, double unexpectedValue)
			{
				decimal? subject = new(subjectValue);
				decimal unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.7)]
			[InlineData(12.5, 12.3)]
			public async Task ForNullableDecimal_WhenOutsideTolerance_ShouldSucceed(
				double subjectValue, double unexpectedValue)
			{
				decimal? subject = new(subjectValue);
				decimal? unexpected = new(unexpectedValue);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal? subject, decimal? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.6)]
			[InlineData(12.5, 12.4)]
			public async Task ForNullableDouble_WhenInsideTolerance_ShouldFail(
				double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.7)]
			[InlineData(12.5, 12.3)]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldSucceed(
				double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double? subject, double? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5F, 12.6F)]
			[InlineData(12.5F, 12.4F)]
			public async Task ForNullableFloat_WhenInsideTolerance_ShouldFail(
				float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11F);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5F, 12.7F)]
			[InlineData(12.5F, 12.3F)]
			public async Task ForNullableFloat_WhenOutsideTolerance_ShouldSucceed(
				float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float? subject, float? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5, 6)]
			[InlineData(5, 4)]
			public async Task ForNullableInt_WhenInsideTolerance_ShouldFail(
				int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 7)]
			[InlineData(5, 3)]
			public async Task ForNullableInt_WhenOutsideTolerance_ShouldSucceed(
				int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int? subject, int? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((long)5, (long)6)]
			[InlineData((long)5, (long)4)]
			public async Task ForNullableLong_WhenInsideTolerance_ShouldFail(
				long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((long)5, (long)7)]
			[InlineData((long)5, (long)3)]
			public async Task ForNullableLong_WhenOutsideTolerance_ShouldSucceed(
				long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long? subject, long? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)6)]
			[InlineData((sbyte)5, (sbyte)4)]
			public async Task ForNullableSbyte_WhenInsideTolerance_ShouldFail(
				sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)7)]
			[InlineData((sbyte)5, (sbyte)3)]
			public async Task ForNullableSbyte_WhenOutsideTolerance_ShouldSucceed(
				sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte? subject, sbyte? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((short)5, (short)6)]
			[InlineData((short)5, (short)4)]
			public async Task ForNullableShort_WhenInsideTolerance_ShouldFail(
				short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((short)5, (short)7)]
			[InlineData((short)5, (short)3)]
			public async Task ForNullableShort_WhenOutsideTolerance_ShouldSucceed(
				short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short? subject, short? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((uint)5, (uint)6)]
			[InlineData((uint)5, (uint)4)]
			public async Task ForNullableUint_WhenInsideTolerance_ShouldFail(
				uint? subject, uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((uint)5, (uint)7)]
			[InlineData((uint)5, (uint)3)]
			public async Task ForNullableUint_WhenOutsideTolerance_ShouldSucceed(
				uint? subject, uint? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)5, (ulong)6)]
			[InlineData((ulong)5, (ulong)4)]
			public async Task ForNullableUlong_WhenInsideTolerance_ShouldFail(
				ulong? subject, ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ulong)5, (ulong)7)]
			[InlineData((ulong)5, (ulong)3)]
			public async Task ForNullableUlong_WhenOutsideTolerance_ShouldSucceed(
				ulong? subject, ulong? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)5, (ushort)6)]
			[InlineData((ushort)5, (ushort)4)]
			public async Task ForNullableUshort_WhenInsideTolerance_ShouldFail(
				ushort? subject, ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ushort)5, (ushort)7)]
			[InlineData((ushort)5, (ushort)3)]
			public async Task ForNullableUshort_WhenOutsideTolerance_ShouldSucceed(
				ushort? subject, ushort? unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(5, 6)]
			[InlineData(5, 4)]
			public async Task ForSbyte_WhenInsideTolerance_ShouldFail(
				sbyte subject, sbyte unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 7)]
			[InlineData(5, 3)]
			public async Task ForSbyte_WhenOutsideTolerance_ShouldSucceed(
				sbyte subject, sbyte unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte subject, sbyte unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5, 6)]
			[InlineData(5, 4)]
			public async Task ForShort_WhenInsideTolerance_ShouldFail(
				short subject, short unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 7)]
			[InlineData(5, 3)]
			public async Task ForShort_WhenOutsideTolerance_ShouldSucceed(
				short subject, short unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short subject, short unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5, 6)]
			[InlineData(5, 4)]
			public async Task ForUint_WhenInsideTolerance_ShouldFail(
				uint subject, uint unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 7)]
			[InlineData(5, 3)]
			public async Task ForUint_WhenOutsideTolerance_ShouldSucceed(
				uint subject, uint unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(5, 6)]
			[InlineData(5, 4)]
			public async Task ForUlong_WhenInsideTolerance_ShouldFail(
				ulong subject, ulong unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 7)]
			[InlineData(5, 3)]
			public async Task ForUlong_WhenOutsideTolerance_ShouldSucceed(
				ulong subject, ulong unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(5, 6)]
			[InlineData(5, 4)]
			public async Task ForUshort_WhenInsideTolerance_ShouldFail(
				ushort subject, ushort unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 7)]
			[InlineData(5, 3)]
			public async Task ForUshort_WhenOutsideTolerance_ShouldSucceed(
				ushort subject, ushort unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
