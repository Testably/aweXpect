using System.Linq;

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsNotOneOf
	{
		public sealed class WithinTests
		{
			[Theory]
			[InlineData((byte)5, (byte)0, (byte)6, (byte)16)]
			[InlineData((byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForByte_WhenInsideTolerance_ShouldFail(
				byte subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((byte)5, (byte)0, (byte)7, (byte)17)]
			[InlineData((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task ForByte_WhenOutsideTolerance_ShouldSucceed(
				byte subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)5, (byte)0, (byte)6, (byte)16)]
			[InlineData((byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForByte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				byte subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((byte)5, (byte)0, (byte)7, (byte)17)]
			[InlineData((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task ForByte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				byte subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldFail(
				double subjectValue, params double[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldSucceed(
				double subjectValue, params double[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, params decimal[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task ForDecimal_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				double subjectValue, params double?[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task ForDecimal_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				double subjectValue, params double?[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForDecimal_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, params decimal?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task ForDouble_WhenInsideTolerance_ShouldFail(
				double subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
			[InlineData(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
			[InlineData(double.NaN, double.PositiveInfinity, 1.0)]
			[InlineData(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
			[InlineData(double.PositiveInfinity, double.NaN, 1.0)]
			[InlineData(double.PositiveInfinity, double.NaN, double.PositiveInfinity)]
			[InlineData(12.5, double.PositiveInfinity, 1.0)]
			[InlineData(12.5, double.PositiveInfinity, double.PositiveInfinity)]
			[InlineData(double.PositiveInfinity, 12.5, 1.0)]
			[InlineData(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
			[InlineData(12.5, double.NaN, 1.0)]
			[InlineData(12.5, double.NaN, double.PositiveInfinity)]
			[InlineData(double.NaN, 12.5, 1.0)]
			[InlineData(double.NaN, 12.5, double.PositiveInfinity)]
			public async Task ForDouble_WhenNonFiniteValuesDiffer_ShouldSucceed(
				double subject, double unexpectedValue, double tolerance)
			{
				double[] unexpected = { unexpectedValue, };
				double? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).DoesNotThrow()
					.Because("it is the exact complement of the failing containment expectation");
				await That(ActNullable).DoesNotThrow();
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldSucceed(
				double subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(double.PositiveInfinity, 1.0)]
			[InlineData(double.PositiveInfinity, double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity, 1.0)]
			[InlineData(double.NegativeInfinity, double.PositiveInfinity)]
			[InlineData(double.NaN, 1.0)]
			[InlineData(double.NaN, double.PositiveInfinity)]
			public async Task ForDouble_WhenSubjectIsTheSameNonFiniteValue_ShouldFail(
				double value, double tolerance)
			{
				double[] unexpected = { value, };
				double? nullableSubject = value;

				async Task Act()
					=> await That(value).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that value
					              is not one of {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}
					              """)
					.Because("it is the exact complement of the succeeding containment expectation");
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is not one of {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsNotOneOf(12.0, 13.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("Tolerance must not be NaN*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Theory]
			[AutoData]
			public async Task
				ForDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task ForDouble_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				double subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task ForDouble_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				double subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForDouble_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.6F, 13.6F)]
			[InlineData(12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForFloat_WhenInsideTolerance_ShouldFail(
				float subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
			[InlineData(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
			[InlineData(float.NaN, float.PositiveInfinity, 1.0F)]
			[InlineData(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
			[InlineData(float.PositiveInfinity, float.NaN, 1.0F)]
			[InlineData(float.PositiveInfinity, float.NaN, float.PositiveInfinity)]
			[InlineData(12.5F, float.PositiveInfinity, 1.0F)]
			[InlineData(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
			[InlineData(float.PositiveInfinity, 12.5F, 1.0F)]
			[InlineData(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
			[InlineData(12.5F, float.NaN, 1.0F)]
			[InlineData(12.5F, float.NaN, float.PositiveInfinity)]
			[InlineData(float.NaN, 12.5F, 1.0F)]
			[InlineData(float.NaN, 12.5F, float.PositiveInfinity)]
			public async Task ForFloat_WhenNonFiniteValuesDiffer_ShouldSucceed(
				float subject, float unexpectedValue, float tolerance)
			{
				float[] unexpected = { unexpectedValue, };
				float? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).DoesNotThrow()
					.Because("it is the exact complement of the failing containment expectation");
				await That(ActNullable).DoesNotThrow();
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.7F, 13.7F)]
			[InlineData(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task ForFloat_WhenOutsideTolerance_ShouldSucceed(
				float subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(float.PositiveInfinity, 1.0F)]
			[InlineData(float.PositiveInfinity, float.PositiveInfinity)]
			[InlineData(float.NegativeInfinity, 1.0F)]
			[InlineData(float.NegativeInfinity, float.PositiveInfinity)]
			[InlineData(float.NaN, 1.0F)]
			[InlineData(float.NaN, float.PositiveInfinity)]
			public async Task ForFloat_WhenSubjectIsTheSameNonFiniteValue_ShouldFail(
				float value, float tolerance)
			{
				float[] unexpected = { value, };
				float? nullableSubject = value;

				async Task Act()
					=> await That(value).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that value
					              is not one of {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}
					              """)
					.Because("it is the exact complement of the succeeding containment expectation");
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is not one of {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task
				ForFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.6F, 13.6F)]
			[InlineData(12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForFloat_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				float subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.7F, 13.7F)]
			[InlineData(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task ForFloat_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				float subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForFloat_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
			[InlineData(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
			[InlineData(double.NaN, double.PositiveInfinity, 1.0)]
			[InlineData(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
			[InlineData(double.PositiveInfinity, double.NaN, 1.0)]
			[InlineData(double.PositiveInfinity, double.NaN, double.PositiveInfinity)]
			[InlineData(12.5, double.PositiveInfinity, 1.0)]
			[InlineData(12.5, double.PositiveInfinity, double.PositiveInfinity)]
			[InlineData(double.PositiveInfinity, 12.5, 1.0)]
			[InlineData(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
			[InlineData(12.5, double.NaN, 1.0)]
			[InlineData(12.5, double.NaN, double.PositiveInfinity)]
			[InlineData(double.NaN, 12.5, 1.0)]
			[InlineData(double.NaN, 12.5, double.PositiveInfinity)]
			public async Task ForHalf_WhenNonFiniteValuesDiffer_ShouldSucceed(
				double subjectValue, double unexpectedValue, double toleranceValue)
			{
				Half subject = (Half)subjectValue;
				Half tolerance = (Half)toleranceValue;
				Half[] unexpected = { (Half)unexpectedValue, };
				Half? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).DoesNotThrow()
					.Because("it is the exact complement of the failing containment expectation");
				await That(ActNullable).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Theory]
			[InlineData(double.PositiveInfinity, 1.0)]
			[InlineData(double.PositiveInfinity, double.PositiveInfinity)]
			[InlineData(double.NegativeInfinity, 1.0)]
			[InlineData(double.NegativeInfinity, double.PositiveInfinity)]
			[InlineData(double.NaN, 1.0)]
			[InlineData(double.NaN, double.PositiveInfinity)]
			public async Task ForHalf_WhenSubjectIsTheSameNonFiniteValue_ShouldFail(
				double value, double toleranceValue)
			{
				Half subject = (Half)value;
				Half tolerance = (Half)toleranceValue;
				Half[] unexpected = { subject, };
				Half? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("it is the exact complement of the succeeding containment expectation");
				await That(ActNullable).Throws<XunitException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is not one of {Formatter.Format(unexpected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
#endif

			[Theory]
			[InlineData(5, 0, 6, 16)]
			[InlineData(5, 0, 4, 14)]
			public async Task ForInt_WhenInsideTolerance_ShouldFail(
				int subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 0, 7, 17)]
			[InlineData(5, 0, 3, 13)]
			public async Task ForInt_WhenOutsideTolerance_ShouldSucceed(
				int subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5, 0, 6, 16)]
			[InlineData(5, 0, 4, 14)]
			public async Task ForInt_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				int subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 0, 7, 17)]
			[InlineData(5, 0, 3, 13)]
			public async Task ForInt_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				int subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForInt_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5L, 0L, 6L, 16L)]
			[InlineData(5L, 0L, 4L, 14L)]
			public async Task ForLong_WhenInsideTolerance_ShouldFail(
				long subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5L, 0L, 7L, 17L)]
			[InlineData(5L, 0L, 3L, 13L)]
			public async Task ForLong_WhenOutsideTolerance_ShouldSucceed(
				long subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5L, 0L, 6L, 16L)]
			[InlineData(5L, 0L, 4L, 14L)]
			public async Task ForLong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				long subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5L, 0L, 7L, 17L)]
			[InlineData(5L, 0L, 3L, 13L)]
			public async Task ForLong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				long subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForLong_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((byte)5, (byte)0, (byte)6, (byte)16)]
			[InlineData((byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForNullableByte_WhenInsideTolerance_ShouldFail(
				byte? subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((byte)5, (byte)0, (byte)7, (byte)17)]
			[InlineData((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task ForNullableByte_WhenOutsideTolerance_ShouldSucceed(
				byte? subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)5, (byte)0, (byte)6, (byte)16)]
			[InlineData((byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForNullableByte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				byte? subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((byte)5, (byte)0, (byte)7, (byte)17)]
			[InlineData((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task
				ForNullableByte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					byte? subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task ForNullableDecimal_WhenInsideTolerance_ShouldFail(
				double? subjectValue, params double[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task ForNullableDecimal_WhenOutsideTolerance_ShouldSucceed(
				double? subjectValue, params double[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal? subject, params decimal[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task
				ForNullableDecimal_WithNullableExpected_WhenInsideTolerance_ShouldFail(
					double? subjectValue, params double?[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task
				ForNullableDecimal_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					double? subjectValue, params double?[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableDecimal_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal? subject, params decimal?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task ForNullableDouble_WhenInsideTolerance_ShouldFail(
				double? subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldSucceed(
				double? subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double? subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.6, 13.6)]
			[InlineData(12.5, 12.0, 12.4, 13.4)]
			public async Task ForNullableDouble_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.0, 12.7, 13.7)]
			[InlineData(12.5, 12.0, 12.3, 13.3)]
			public async Task
				ForNullableDouble_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableDouble_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.6F, 13.6F)]
			[InlineData(12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForNullableFloat_WhenInsideTolerance_ShouldFail(
				float? subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.7F, 13.7F)]
			[InlineData(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task ForNullableFloat_WhenOutsideTolerance_ShouldSucceed(
				float? subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float? subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.6F, 13.6F)]
			[InlineData(12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForNullableFloat_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5
					              """);
			}

			[Theory]
			[InlineData(12.5F, 12.0F, 12.7F, 13.7F)]
			[InlineData(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task
				ForNullableFloat_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableFloat_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5, 0, 6, 16)]
			[InlineData(5, 0, 4, 14)]
			public async Task ForNullableInt_WhenInsideTolerance_ShouldFail(
				int? subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 0, 7, 17)]
			[InlineData(5, 0, 3, 13)]
			public async Task ForNullableInt_WhenOutsideTolerance_ShouldSucceed(
				int? subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int? subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5, 0, 6, 16)]
			[InlineData(5, 0, 4, 14)]
			public async Task ForNullableInt_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData(5, 0, 7, 17)]
			[InlineData(5, 0, 3, 13)]
			public async Task
				ForNullableInt_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableInt_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((long)5, (long)0, (long)6, (long)16)]
			[InlineData((long)5, (long)0, (long)4, (long)14)]
			public async Task ForNullableLong_WhenInsideTolerance_ShouldFail(
				long? subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((long)5, (long)0, (long)7, (long)17)]
			[InlineData((long)5, (long)0, (long)3, (long)13)]
			public async Task ForNullableLong_WhenOutsideTolerance_ShouldSucceed(
				long? subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long? subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((long)5, (long)0, (long)6, (long)16)]
			[InlineData((long)5, (long)0, (long)4, (long)14)]
			public async Task ForNullableLong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((long)5, (long)0, (long)7, (long)17)]
			[InlineData((long)5, (long)0, (long)3, (long)13)]
			public async Task
				ForNullableLong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableLong_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForNullableSbyte_WhenInsideTolerance_ShouldFail(
				sbyte? subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task ForNullableSbyte_WhenOutsideTolerance_ShouldSucceed(
				sbyte? subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte? subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForNullableSbyte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task
				ForNullableSbyte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableSbyte_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)6, (short)16)]
			[InlineData((short)5, (short)0, (short)4, (short)14)]
			public async Task ForNullableShort_WhenInsideTolerance_ShouldFail(
				short? subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)7, (short)17)]
			[InlineData((short)5, (short)0, (short)3, (short)13)]
			public async Task ForNullableShort_WhenOutsideTolerance_ShouldSucceed(
				short? subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short? subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)6, (short)16)]
			[InlineData((short)5, (short)0, (short)4, (short)14)]
			public async Task ForNullableShort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)7, (short)17)]
			[InlineData((short)5, (short)0, (short)3, (short)13)]
			public async Task
				ForNullableShort_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForNullableShort_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)6, (uint)16)]
			[InlineData((uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForNullableUint_WhenInsideTolerance_ShouldFail(
				uint? subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)7, (uint)17)]
			[InlineData((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task ForNullableUint_WhenOutsideTolerance_ShouldSucceed(
				uint? subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)6, (uint)16)]
			[InlineData((uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForNullableUint_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				uint? subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)7, (uint)17)]
			[InlineData((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task
				ForNullableUint_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					uint? subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[InlineData((ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForNullableUlong_WhenInsideTolerance_ShouldFail(
				ulong? subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[InlineData((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task ForNullableUlong_WhenOutsideTolerance_ShouldSucceed(
				ulong? subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[InlineData((ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForNullableUlong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				ulong? subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[InlineData((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task
				ForNullableUlong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					ulong? subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[InlineData((ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForNullableUshort_WhenInsideTolerance_ShouldFail(
				ushort? subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[InlineData((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
			public async Task ForNullableUshort_WhenOutsideTolerance_ShouldSucceed(
				ushort? subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[InlineData((ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForNullableUshort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				ushort? subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[InlineData((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
			public async Task
				ForNullableUshort_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					ushort? subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForSbyte_WhenInsideTolerance_ShouldFail(
				sbyte subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task ForSbyte_WhenOutsideTolerance_ShouldSucceed(
				sbyte subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForSbyte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				sbyte subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[InlineData((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task ForSbyte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				sbyte subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForSbyte_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)6, (short)16)]
			[InlineData((short)5, (short)0, (short)4, (short)14)]
			public async Task ForShort_WhenInsideTolerance_ShouldFail(
				short subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)7, (short)17)]
			[InlineData((short)5, (short)0, (short)3, (short)13)]
			public async Task ForShort_WhenOutsideTolerance_ShouldSucceed(
				short subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)6, (short)16)]
			[InlineData((short)5, (short)0, (short)4, (short)14)]
			public async Task ForShort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				short subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((short)5, (short)0, (short)7, (short)17)]
			[InlineData((short)5, (short)0, (short)3, (short)13)]
			public async Task ForShort_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				short subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task
				ForShort_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)6, (uint)16)]
			[InlineData((uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForUint_WhenInsideTolerance_ShouldFail(
				uint subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)7, (uint)17)]
			[InlineData((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task ForUint_WhenOutsideTolerance_ShouldSucceed(
				uint subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)6, (uint)16)]
			[InlineData((uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForUint_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				uint subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((uint)5, (uint)0, (uint)7, (uint)17)]
			[InlineData((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task ForUint_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				uint subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[InlineData((ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForUlong_WhenInsideTolerance_ShouldFail(
				ulong subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[InlineData((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task ForUlong_WhenOutsideTolerance_ShouldSucceed(
				ulong subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[InlineData((ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForUlong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				ulong subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[InlineData((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task ForUlong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				ulong subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[InlineData((ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForUshort_WhenInsideTolerance_ShouldFail(
				ushort subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[InlineData((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
			public async Task ForUshort_WhenOutsideTolerance_ShouldSucceed(
				ushort subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[InlineData((ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForUshort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				ushort subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5
					              """);
			}

			[Theory]
			[InlineData((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[InlineData((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
			public async Task ForUshort_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				ushort subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
