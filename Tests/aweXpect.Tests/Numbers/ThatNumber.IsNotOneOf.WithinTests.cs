using System.Linq;

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsNotOneOf
	{
		public sealed class WithinTests
		{
			[Test]
			[Arguments("-1", (byte)5, (byte)0, (byte)6, (byte)16)]
			[Arguments("1", (byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForByte_WhenInsideTolerance_ShouldFail(
				string differsBy, byte subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((byte)5, (byte)0, (byte)7, (byte)17)]
			[Arguments((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task ForByte_WhenOutsideTolerance_ShouldSucceed(
				byte subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (byte)5, (byte)0, (byte)6, (byte)16)]
			[Arguments("1", (byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForByte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, byte subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((byte)5, (byte)0, (byte)7, (byte)17)]
			[Arguments((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task ForByte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				byte subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-0.1", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.1", 12.5, 12.0, 12.4, 13.4)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldFail(
				string differsBy, double subjectValue, params double[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 0.1,
					              but it was 12.5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
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

			[Test]
			[AutoArguments]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, params decimal[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.1", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.1", 12.5, 12.0, 12.4, 13.4)]
			public async Task ForDecimal_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, double subjectValue, params double?[] unexpectedValues)
			{
				decimal subject = new(subjectValue);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
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

			[Test]
			[AutoArguments]
			public async Task
				ForDecimal_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, params decimal?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.0999999999999996", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.0999999999999996", 12.5, 12.0, 12.4, 13.4)]
			public async Task ForDouble_WhenInsideTolerance_ShouldFail(
				string differsBy, double subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
			[Arguments(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
			[Arguments(double.NaN, double.PositiveInfinity, 1.0)]
			[Arguments(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
			[Arguments(double.PositiveInfinity, double.NaN, 1.0)]
			[Arguments(double.PositiveInfinity, double.NaN, double.PositiveInfinity)]
			[Arguments(12.5, double.PositiveInfinity, 1.0)]
			[Arguments(12.5, double.PositiveInfinity, double.PositiveInfinity)]
			[Arguments(double.PositiveInfinity, 12.5, 1.0)]
			[Arguments(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
			[Arguments(12.5, double.NaN, 1.0)]
			[Arguments(12.5, double.NaN, double.PositiveInfinity)]
			[Arguments(double.NaN, 12.5, 1.0)]
			[Arguments(double.NaN, 12.5, double.PositiveInfinity)]
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

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldSucceed(
				double subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.PositiveInfinity, 1.0)]
			[Arguments(double.PositiveInfinity, double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity, 1.0)]
			[Arguments(double.NegativeInfinity, double.PositiveInfinity)]
			[Arguments(double.NaN, 1.0)]
			[Arguments(double.NaN, double.PositiveInfinity)]
			public async Task ForDouble_WhenSubjectIsTheSameNonFiniteValue_ShouldFail(
				double value, double tolerance)
			{
				double[] unexpected = { value, };
				double? nullableSubject = value;

				async Task Act()
					=> await That(value).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that value
					              is not one of unexpected ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """)
					.Because("it is the exact complement of the succeeding containment expectation");
				await That(ActNullable).Throws<FailException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is not one of unexpected ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsNotOneOf(12.0, 13.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			[AutoArguments]
			public async Task
				ForDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.0999999999999996", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.0999999999999996", 12.5, 12.0, 12.4, 13.4)]
			public async Task ForDouble_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, double subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
			public async Task ForDouble_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				double subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForDouble_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.1000004", 12.5F, 12.0F, 12.6F, 13.6F)]
			[Arguments("0.1000004", 12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForFloat_WhenInsideTolerance_ShouldFail(
				string differsBy, float subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(float.PositiveInfinity, float.NegativeInfinity, 1.0F)]
			[Arguments(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity)]
			[Arguments(float.NaN, float.PositiveInfinity, 1.0F)]
			[Arguments(float.NaN, float.PositiveInfinity, float.PositiveInfinity)]
			[Arguments(float.PositiveInfinity, float.NaN, 1.0F)]
			[Arguments(float.PositiveInfinity, float.NaN, float.PositiveInfinity)]
			[Arguments(12.5F, float.PositiveInfinity, 1.0F)]
			[Arguments(12.5F, float.PositiveInfinity, float.PositiveInfinity)]
			[Arguments(float.PositiveInfinity, 12.5F, 1.0F)]
			[Arguments(float.PositiveInfinity, 12.5F, float.PositiveInfinity)]
			[Arguments(12.5F, float.NaN, 1.0F)]
			[Arguments(12.5F, float.NaN, float.PositiveInfinity)]
			[Arguments(float.NaN, 12.5F, 1.0F)]
			[Arguments(float.NaN, 12.5F, float.PositiveInfinity)]
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

			[Test]
			[Arguments(12.5F, 12.0F, 12.7F, 13.7F)]
			[Arguments(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task ForFloat_WhenOutsideTolerance_ShouldSucceed(
				float subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(float.PositiveInfinity, 1.0F)]
			[Arguments(float.PositiveInfinity, float.PositiveInfinity)]
			[Arguments(float.NegativeInfinity, 1.0F)]
			[Arguments(float.NegativeInfinity, float.PositiveInfinity)]
			[Arguments(float.NaN, 1.0F)]
			[Arguments(float.NaN, float.PositiveInfinity)]
			public async Task ForFloat_WhenSubjectIsTheSameNonFiniteValue_ShouldFail(
				float value, float tolerance)
			{
				float[] unexpected = { value, };
				float? nullableSubject = value;

				async Task Act()
					=> await That(value).IsNotOneOf(unexpected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsNotOneOf(unexpected).Within(tolerance);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that value
					              is not one of unexpected ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """)
					.Because("it is the exact complement of the succeeding containment expectation");
				await That(ActNullable).Throws<FailException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is not one of unexpected ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(value)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.1000004", 12.5F, 12.0F, 12.6F, 13.6F)]
			[Arguments("0.1000004", 12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForFloat_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, float subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(12.5F, 12.0F, 12.7F, 13.7F)]
			[Arguments(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task ForFloat_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				float subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForFloat_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

#if NET8_0_OR_GREATER
			[Test]
			[Arguments(double.PositiveInfinity, double.NegativeInfinity, 1.0)]
			[Arguments(double.PositiveInfinity, double.NegativeInfinity, double.PositiveInfinity)]
			[Arguments(double.NaN, double.PositiveInfinity, 1.0)]
			[Arguments(double.NaN, double.PositiveInfinity, double.PositiveInfinity)]
			[Arguments(double.PositiveInfinity, double.NaN, 1.0)]
			[Arguments(double.PositiveInfinity, double.NaN, double.PositiveInfinity)]
			[Arguments(12.5, double.PositiveInfinity, 1.0)]
			[Arguments(12.5, double.PositiveInfinity, double.PositiveInfinity)]
			[Arguments(double.PositiveInfinity, 12.5, 1.0)]
			[Arguments(double.PositiveInfinity, 12.5, double.PositiveInfinity)]
			[Arguments(12.5, double.NaN, 1.0)]
			[Arguments(12.5, double.NaN, double.PositiveInfinity)]
			[Arguments(double.NaN, 12.5, 1.0)]
			[Arguments(double.NaN, 12.5, double.PositiveInfinity)]
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
			[Test]
			[Arguments(double.PositiveInfinity, 1.0)]
			[Arguments(double.PositiveInfinity, double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity, 1.0)]
			[Arguments(double.NegativeInfinity, double.PositiveInfinity)]
			[Arguments(double.NaN, 1.0)]
			[Arguments(double.NaN, double.PositiveInfinity)]
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

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """)
					.Because("it is the exact complement of the succeeding containment expectation");
				await That(ActNullable).Throws<FailException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is not one of unexpected ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}
#endif

			[Test]
			[Arguments("-1", 5, 0, 6, 16)]
			[Arguments("1", 5, 0, 4, 14)]
			public async Task ForInt_WhenInsideTolerance_ShouldFail(
				string differsBy, int subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(5, 0, 7, 17)]
			[Arguments(5, 0, 3, 13)]
			public async Task ForInt_WhenOutsideTolerance_ShouldSucceed(
				int subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", 5, 0, 6, 16)]
			[Arguments("1", 5, 0, 4, 14)]
			public async Task ForInt_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, int subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(5, 0, 7, 17)]
			[Arguments(5, 0, 3, 13)]
			public async Task ForInt_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				int subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForInt_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", 5L, 0L, 6L, 16L)]
			[Arguments("1", 5L, 0L, 4L, 14L)]
			public async Task ForLong_WhenInsideTolerance_ShouldFail(
				string differsBy, long subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(5L, 0L, 7L, 17L)]
			[Arguments(5L, 0L, 3L, 13L)]
			public async Task ForLong_WhenOutsideTolerance_ShouldSucceed(
				long subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", 5L, 0L, 6L, 16L)]
			[Arguments("1", 5L, 0L, 4L, 14L)]
			public async Task ForLong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, long subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(5L, 0L, 7L, 17L)]
			[Arguments(5L, 0L, 3L, 13L)]
			public async Task ForLong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				long subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForLong_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (byte)5, (byte)0, (byte)6, (byte)16)]
			[Arguments("1", (byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForNullableByte_WhenInsideTolerance_ShouldFail(
				string differsBy, byte? subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((byte)5, (byte)0, (byte)7, (byte)17)]
			[Arguments((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task ForNullableByte_WhenOutsideTolerance_ShouldSucceed(
				byte? subject, params byte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (byte)5, (byte)0, (byte)6, (byte)16)]
			[Arguments("1", (byte)5, (byte)0, (byte)4, (byte)14)]
			public async Task ForNullableByte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, byte? subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((byte)5, (byte)0, (byte)7, (byte)17)]
			[Arguments((byte)5, (byte)0, (byte)3, (byte)13)]
			public async Task
				ForNullableByte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					byte? subject, params byte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-0.1", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.1", 12.5, 12.0, 12.4, 13.4)]
			public async Task ForNullableDecimal_WhenInsideTolerance_ShouldFail(
				string differsBy, double? subjectValue, params double[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal[] unexpected = unexpectedValues
					.Select(unexpectedValue => new decimal(unexpectedValue))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 0.1,
					              but it was 12.5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
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

			[Test]
			[AutoArguments]
			public async Task
				ForNullableDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal? subject, params decimal[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.1", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.1", 12.5, 12.0, 12.4, 13.4)]
			public async Task
				ForNullableDecimal_WithNullableExpected_WhenInsideTolerance_ShouldFail(
					string differsBy, double? subjectValue, params double?[] unexpectedValues)
			{
				decimal? subject = subjectValue == null ? null : new decimal(subjectValue.Value);
				decimal?[] unexpected = unexpectedValues
					.Select(unexpectedValue => unexpectedValue == null
						? (decimal?)null
						: new decimal(unexpectedValue.Value))
					.ToArray();

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.1,
					              but it was 12.5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
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

			[Test]
			[AutoArguments]
			public async Task
				ForNullableDecimal_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal? subject, params decimal?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.0999999999999996", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.0999999999999996", 12.5, 12.0, 12.4, 13.4)]
			public async Task ForNullableDouble_WhenInsideTolerance_ShouldFail(
				string differsBy, double? subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldSucceed(
				double? subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double? subject, params double[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.0999999999999996", 12.5, 12.0, 12.6, 13.6)]
			[Arguments("0.0999999999999996", 12.5, 12.0, 12.4, 13.4)]
			public async Task ForNullableDouble_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(12.5, 12.0, 12.7, 13.7)]
			[Arguments(12.5, 12.0, 12.3, 13.3)]
			public async Task
				ForNullableDouble_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableDouble_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double? subject, params double?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.1000004", 12.5F, 12.0F, 12.6F, 13.6F)]
			[Arguments("0.1000004", 12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForNullableFloat_WhenInsideTolerance_ShouldFail(
				string differsBy, float? subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(12.5F, 12.0F, 12.7F, 13.7F)]
			[Arguments(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task ForNullableFloat_WhenOutsideTolerance_ShouldSucceed(
				float? subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float? subject, params float[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-0.1000004", 12.5F, 12.0F, 12.6F, 13.6F)]
			[Arguments("0.1000004", 12.5F, 12.0F, 12.4F, 13.4F)]
			public async Task ForNullableFloat_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 0.11,
					              but it was 12.5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(12.5F, 12.0F, 12.7F, 13.7F)]
			[Arguments(12.5F, 12.0F, 12.3F, 13.3F)]
			public async Task
				ForNullableFloat_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableFloat_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float? subject, params float?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", 5, 0, 6, 16)]
			[Arguments("1", 5, 0, 4, 14)]
			public async Task ForNullableInt_WhenInsideTolerance_ShouldFail(
				string differsBy, int? subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments(5, 0, 7, 17)]
			[Arguments(5, 0, 3, 13)]
			public async Task ForNullableInt_WhenOutsideTolerance_ShouldSucceed(
				int? subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int? subject, params int[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", 5, 0, 6, 16)]
			[Arguments("1", 5, 0, 4, 14)]
			public async Task ForNullableInt_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments(5, 0, 7, 17)]
			[Arguments(5, 0, 3, 13)]
			public async Task
				ForNullableInt_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableInt_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int? subject, params int?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (long)5, (long)0, (long)6, (long)16)]
			[Arguments("1", (long)5, (long)0, (long)4, (long)14)]
			public async Task ForNullableLong_WhenInsideTolerance_ShouldFail(
				string differsBy, long? subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((long)5, (long)0, (long)7, (long)17)]
			[Arguments((long)5, (long)0, (long)3, (long)13)]
			public async Task ForNullableLong_WhenOutsideTolerance_ShouldSucceed(
				long? subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long? subject, params long[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (long)5, (long)0, (long)6, (long)16)]
			[Arguments("1", (long)5, (long)0, (long)4, (long)14)]
			public async Task ForNullableLong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((long)5, (long)0, (long)7, (long)17)]
			[Arguments((long)5, (long)0, (long)3, (long)13)]
			public async Task
				ForNullableLong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableLong_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long? subject, params long?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[Arguments("1", (sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForNullableSbyte_WhenInsideTolerance_ShouldFail(
				string differsBy, sbyte? subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task ForNullableSbyte_WhenOutsideTolerance_ShouldSucceed(
				sbyte? subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte? subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[Arguments("1", (sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForNullableSbyte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task
				ForNullableSbyte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableSbyte_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte? subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (short)5, (short)0, (short)6, (short)16)]
			[Arguments("1", (short)5, (short)0, (short)4, (short)14)]
			public async Task ForNullableShort_WhenInsideTolerance_ShouldFail(
				string differsBy, short? subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((short)5, (short)0, (short)7, (short)17)]
			[Arguments((short)5, (short)0, (short)3, (short)13)]
			public async Task ForNullableShort_WhenOutsideTolerance_ShouldSucceed(
				short? subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short? subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (short)5, (short)0, (short)6, (short)16)]
			[Arguments("1", (short)5, (short)0, (short)4, (short)14)]
			public async Task ForNullableShort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((short)5, (short)0, (short)7, (short)17)]
			[Arguments((short)5, (short)0, (short)3, (short)13)]
			public async Task
				ForNullableShort_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableShort_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short? subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (uint)5, (uint)0, (uint)6, (uint)16)]
			[Arguments("1", (uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForNullableUint_WhenInsideTolerance_ShouldFail(
				string differsBy, uint? subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((uint)5, (uint)0, (uint)7, (uint)17)]
			[Arguments((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task ForNullableUint_WhenOutsideTolerance_ShouldSucceed(
				uint? subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (uint)5, (uint)0, (uint)6, (uint)16)]
			[Arguments("1", (uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForNullableUint_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, uint? subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((uint)5, (uint)0, (uint)7, (uint)17)]
			[Arguments((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task
				ForNullableUint_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					uint? subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[Arguments("1", (ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForNullableUlong_WhenInsideTolerance_ShouldFail(
				string differsBy, ulong? subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[Arguments((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task ForNullableUlong_WhenOutsideTolerance_ShouldSucceed(
				ulong? subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[Arguments("1", (ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForNullableUlong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, ulong? subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[Arguments((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task
				ForNullableUlong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					ulong? subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[Arguments("1", (ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForNullableUshort_WhenInsideTolerance_ShouldFail(
				string differsBy, ushort? subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[Arguments((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
			public async Task ForNullableUshort_WhenOutsideTolerance_ShouldSucceed(
				ushort? subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[Arguments("1", (ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForNullableUshort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, ushort? subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[Arguments((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
			public async Task
				ForNullableUshort_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
					ushort? subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[Arguments("1", (sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForSbyte_WhenInsideTolerance_ShouldFail(
				string differsBy, sbyte subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task ForSbyte_WhenOutsideTolerance_ShouldSucceed(
				sbyte subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte subject, params sbyte[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (sbyte)5, (sbyte)0, (sbyte)6, (sbyte)16)]
			[Arguments("1", (sbyte)5, (sbyte)0, (sbyte)4, (sbyte)14)]
			public async Task ForSbyte_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, sbyte subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)7, (sbyte)17)]
			[Arguments((sbyte)5, (sbyte)0, (sbyte)3, (sbyte)13)]
			public async Task ForSbyte_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				sbyte subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForSbyte_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte subject, params sbyte?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (short)5, (short)0, (short)6, (short)16)]
			[Arguments("1", (short)5, (short)0, (short)4, (short)14)]
			public async Task ForShort_WhenInsideTolerance_ShouldFail(
				string differsBy, short subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((short)5, (short)0, (short)7, (short)17)]
			[Arguments((short)5, (short)0, (short)3, (short)13)]
			public async Task ForShort_WhenOutsideTolerance_ShouldSucceed(
				short subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short subject, params short[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (short)5, (short)0, (short)6, (short)16)]
			[Arguments("1", (short)5, (short)0, (short)4, (short)14)]
			public async Task ForShort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, short subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((short)5, (short)0, (short)7, (short)17)]
			[Arguments((short)5, (short)0, (short)3, (short)13)]
			public async Task ForShort_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				short subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[AutoArguments]
			public async Task
				ForShort_WithNullableExpected_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short subject, params short?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments("-1", (uint)5, (uint)0, (uint)6, (uint)16)]
			[Arguments("1", (uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForUint_WhenInsideTolerance_ShouldFail(
				string differsBy, uint subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((uint)5, (uint)0, (uint)7, (uint)17)]
			[Arguments((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task ForUint_WhenOutsideTolerance_ShouldSucceed(
				uint subject, params uint[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (uint)5, (uint)0, (uint)6, (uint)16)]
			[Arguments("1", (uint)5, (uint)0, (uint)4, (uint)14)]
			public async Task ForUint_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, uint subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((uint)5, (uint)0, (uint)7, (uint)17)]
			[Arguments((uint)5, (uint)0, (uint)3, (uint)13)]
			public async Task ForUint_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				uint subject, params uint?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[Arguments("1", (ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForUlong_WhenInsideTolerance_ShouldFail(
				string differsBy, ulong subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[Arguments((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task ForUlong_WhenOutsideTolerance_ShouldSucceed(
				ulong subject, params ulong[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ulong)5, (ulong)0, (ulong)6, (ulong)16)]
			[Arguments("1", (ulong)5, (ulong)0, (ulong)4, (ulong)14)]
			public async Task ForUlong_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, ulong subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((ulong)5, (ulong)0, (ulong)7, (ulong)17)]
			[Arguments((ulong)5, (ulong)0, (ulong)3, (ulong)13)]
			public async Task ForUlong_WithNullableExpected_WhenOutsideTolerance_ShouldSucceed(
				ulong subject, params ulong?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[Arguments("1", (ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForUshort_WhenInsideTolerance_ShouldFail(
				string differsBy, ushort subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of unexpected ± 1,
					              but it was 5, which differs by {differsBy} from the closest value

					              Unexpected values:
					              {Formatter.Format(unexpected)}
					              """);
			}

			[Test]
			[Arguments((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[Arguments((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
			public async Task ForUshort_WhenOutsideTolerance_ShouldSucceed(
				ushort subject, params ushort[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("-1", (ushort)5, (ushort)0, (ushort)6, (ushort)16)]
			[Arguments("1", (ushort)5, (ushort)0, (ushort)4, (ushort)14)]
			public async Task ForUshort_WithNullableExpected_WhenInsideTolerance_ShouldFail(
				string differsBy, ushort subject, params ushort?[] unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)} ± 1,
					              but it was 5, which differs by {differsBy} from the closest value
					              """);
			}

			[Test]
			[Arguments((ushort)5, (ushort)0, (ushort)7, (ushort)17)]
			[Arguments((ushort)5, (ushort)0, (ushort)3, (ushort)13)]
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
