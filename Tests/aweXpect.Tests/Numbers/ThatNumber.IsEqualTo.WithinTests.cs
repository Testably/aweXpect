namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsEqualTo
	{
		public sealed class WithinTests
		{
			[Test]
			[Arguments((byte)5, (byte)6)]
			[Arguments((byte)5, (byte)4)]
			public async Task ForByte_WhenInsideTolerance_ShouldSucceed(
				byte subject, byte expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)5, (byte)7, -2)]
			[Arguments((byte)5, (byte)3, 2)]
			public async Task ForByte_WhenOutsideTolerance_ShouldFail(
				byte subject, byte expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.6)]
			[Arguments(12.5, 12.4)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(12.5, 12.7, "-0.2")]
			[Arguments(12.5, 12.3, "0.2")]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0.1,
					              but it was 12.5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, decimal expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(12.5, 12.6)]
			[Arguments(12.5, 12.4)]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed(
				double subject, double expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.11);

				await That(Act).DoesNotThrow();
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
			public async Task ForDouble_WhenNonFiniteValuesDiffer_ShouldFail(
				double subject, double expected, double tolerance)
			{
				double? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected).Within(tolerance);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("no tolerance can bridge the distance to a non-finite value");
				await That(ActNullable).Throws<FailException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.9, "-0.4")]
			[Arguments(12.5, 12.1, "0.4")]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail(
				double subject, double expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0.1,
					              but it was 12.5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			public async Task ForDouble_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				double subject = double.NaN;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(double.PositiveInfinity, 1.0)]
			[Arguments(double.PositiveInfinity, double.PositiveInfinity)]
			[Arguments(double.NegativeInfinity, 1.0)]
			[Arguments(double.NegativeInfinity, double.PositiveInfinity)]
			[Arguments(double.NaN, 1.0)]
			[Arguments(double.NaN, double.PositiveInfinity)]
			public async Task ForDouble_WhenSubjectAndExpectedAreTheSameNonFiniteValue_ShouldSucceed(
				double value, double tolerance)
			{
				double? nullableSubject = value;

				async Task Act()
					=> await That(value).IsEqualTo(value).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(value).Within(tolerance);

				await That(Act).DoesNotThrow()
					.Because("the tolerance must not narrow what plain equality already accepts");
				await That(ActNullable).DoesNotThrow();
			}

			[Test]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsEqualTo(12.5).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			[AutoArguments]
			public async Task
				ForDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double subject, double expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task ForDouble_WhenToleranceIsSpecifiedTwice_ShouldThrowInvalidOperationException(
				bool negated)
			{
				double subject = 12.5;

				async Task Act()
				{
					if (negated)
					{
						await That(subject).IsNotEqualTo(12.5).Within(0.1).Within(0.2);
					}
					else
					{
						await That(subject).IsEqualTo(12.5).Within(0.1).Within(0.2);
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("the second tolerance would silently replace the first one");
			}

			[Test]
			[Arguments(12.5F, 12.6F)]
			[Arguments(12.5F, 12.4F)]
			public async Task ForFloat_WhenInsideTolerance_ShouldSucceed(
				float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.11F);

				await That(Act).DoesNotThrow();
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
			public async Task ForFloat_WhenNonFiniteValuesDiffer_ShouldFail(
				float subject, float expected, float tolerance)
			{
				float? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected).Within(tolerance);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("no tolerance can bridge the distance to a non-finite value");
				await That(ActNullable).Throws<FailException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(12.5F, 13.0F, "-0.5")]
			[Arguments(12.5F, 12.0F, "0.5")]
			public async Task ForFloat_WhenOutsideTolerance_ShouldFail(
				float subject, float expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1F);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0.1,
					              but it was 12.5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			public async Task ForFloat_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				float subject = float.NaN;
				float expected = float.NaN;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1F);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(float.PositiveInfinity, 1.0F)]
			[Arguments(float.PositiveInfinity, float.PositiveInfinity)]
			[Arguments(float.NegativeInfinity, 1.0F)]
			[Arguments(float.NegativeInfinity, float.PositiveInfinity)]
			[Arguments(float.NaN, 1.0F)]
			[Arguments(float.NaN, float.PositiveInfinity)]
			public async Task ForFloat_WhenSubjectAndExpectedAreTheSameNonFiniteValue_ShouldSucceed(
				float value, float tolerance)
			{
				float? nullableSubject = value;

				async Task Act()
					=> await That(value).IsEqualTo(value).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(value).Within(tolerance);

				await That(Act).DoesNotThrow()
					.Because("the tolerance must not narrow what plain equality already accepts");
				await That(ActNullable).DoesNotThrow();
			}

			[Test]
			public async Task ForFloat_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5F).IsEqualTo(12.5F).Within(float.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			[AutoArguments]
			public async Task
				ForFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-0.1F);

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
			public async Task ForHalf_WhenNonFiniteValuesDiffer_ShouldFail(
				double subjectValue, double expectedValue, double toleranceValue)
			{
				Half subject = (Half)subjectValue;
				Half expected = (Half)expectedValue;
				Half tolerance = (Half)toleranceValue;
				Half? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(expected).Within(tolerance);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """)
					.Because("no tolerance can bridge the distance to a non-finite value");
				await That(ActNullable).Throws<FailException>()
					.WithMessage($"""
					              Expected that nullableSubject
					              is equal to {Formatter.Format(expected)} ± {Formatter.Format(tolerance)},
					              but it was {Formatter.Format(subject)}
					              """);
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
			public async Task ForHalf_WhenSubjectAndExpectedAreTheSameNonFiniteValue_ShouldSucceed(
				double value, double toleranceValue)
			{
				Half subject = (Half)value;
				Half tolerance = (Half)toleranceValue;
				Half? nullableSubject = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(subject).Within(tolerance);

				async Task ActNullable()
					=> await That(nullableSubject).IsEqualTo(subject).Within(tolerance);

				await That(Act).DoesNotThrow()
					.Because("the tolerance must not narrow what plain equality already accepts");
				await That(ActNullable).DoesNotThrow();
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That((Half)12.5).IsEqualTo((Half)12.5).Within(Half.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}
#endif

			[Test]
			[Arguments(5, 6)]
			[Arguments(5, 4)]
			public async Task ForInt_WhenInsideTolerance_ShouldSucceed(
				int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5, 7, -2)]
			[Arguments(5, 3, 2)]
			public async Task ForInt_WhenOutsideTolerance_ShouldFail(
				int subject, int expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(5L, 6L)]
			[Arguments(5L, 4L)]
			public async Task ForLong_WhenInsideTolerance_ShouldSucceed(
				long subject, long expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5L, 7L, -2)]
			[Arguments(5L, 3L, 2)]
			public async Task ForLong_WhenOutsideTolerance_ShouldFail(
				long subject, long expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long subject, long expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments((byte)5, (byte)6)]
			[Arguments((byte)5, (byte)4)]
			public async Task ForNullableByte_WhenInsideTolerance_ShouldSucceed(
				byte? subject, byte? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)5, (byte)7, -2)]
			[Arguments((byte)5, (byte)3, 2)]
			public async Task ForNullableByte_WhenOutsideTolerance_ShouldFail(
				byte? subject, byte? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.6)]
			[Arguments(12.5, 12.4)]
			public async Task ForNullableDecimal_WhenInsideTolerance_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal? subject = new(subjectValue);
				decimal? expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(12.5, 12.7, "-0.2")]
			[Arguments(12.5, 12.3, "0.2")]
			public async Task ForNullableDecimal_WhenOutsideTolerance_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal? subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0.1,
					              but it was 12.5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal? subject, decimal? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(12.5, 12.6)]
			[Arguments(12.5, 12.4)]
			public async Task ForNullableDouble_WhenInsideTolerance_ShouldSucceed(
				double? subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.11);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(12.5, 12.9, "-0.4")]
			[Arguments(12.5, 12.1, "0.4")]
			public async Task ForNullableDouble_WhenOutsideTolerance_ShouldFail(
				double? subject, double? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0.1,
					              but it was 12.5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			public async Task ForNullableDouble_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				double? subject = double.NaN;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double? subject = 12.5;

				async Task Act()
					=> await That(subject).IsEqualTo(12.5).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableDouble_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					double? subject, double? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-0.1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task ForNullableDouble_WhenToleranceIsSpecifiedTwice_ShouldThrowInvalidOperationException(
				bool negated)
			{
				double? subject = 12.5;

				async Task Act()
				{
					if (negated)
					{
						await That(subject).IsNotEqualTo(12.5).Within(0.1).Within(0.2);
					}
					else
					{
						await That(subject).IsEqualTo(12.5).Within(0.1).Within(0.2);
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("the second tolerance would silently replace the first one");
			}

			[Test]
			[Arguments(12.5F, 12.6F)]
			[Arguments(12.5F, 12.4F)]
			public async Task ForNullableFloat_WhenInsideTolerance_ShouldSucceed(
				float? subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.11F);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(12.5F, 13.0F, "-0.5")]
			[Arguments(12.5F, 12.0F, "0.5")]
			public async Task ForNullableFloat_WhenOutsideTolerance_ShouldFail(
				float? subject, float? expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1F);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 0.1,
					              but it was 12.5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			public async Task ForNullableFloat_WhenSubjectAndExpectedAreNaN_ShouldSucceed()
			{
				float? subject = float.NaN;
				float expected = float.NaN;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1F);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableFloat_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				float? subject = 12.5F;

				async Task Act()
					=> await That(subject).IsEqualTo(12.5F).Within(float.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableFloat_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					float? subject, float? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-0.1F);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNullableHalf_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				Half? subject = (Half)12.5;

				async Task Act()
					=> await That(subject).IsEqualTo((Half)12.5).Within(Half.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}
#endif

			[Test]
			[Arguments(5, 6)]
			[Arguments(5, 4)]
			public async Task ForNullableInt_WhenInsideTolerance_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5, 7, -2)]
			[Arguments(5, 3, 2)]
			public async Task ForNullableInt_WhenOutsideTolerance_ShouldFail(
				int? subject, int? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments((long)5, (long)6)]
			[Arguments((long)5, (long)4)]
			public async Task ForNullableLong_WhenInsideTolerance_ShouldSucceed(
				long? subject, long? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((long)5, (long)7, -2)]
			[Arguments((long)5, (long)3, 2)]
			public async Task ForNullableLong_WhenOutsideTolerance_ShouldFail(
				long? subject, long? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableLong_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					long? subject, long? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments((sbyte)5, (sbyte)6)]
			[Arguments((sbyte)5, (sbyte)4)]
			public async Task ForNullableSbyte_WhenInsideTolerance_ShouldSucceed(
				sbyte? subject, sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((sbyte)5, (sbyte)7, -2)]
			[Arguments((sbyte)5, (sbyte)3, 2)]
			public async Task ForNullableSbyte_WhenOutsideTolerance_ShouldFail(
				sbyte? subject, sbyte? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte? subject, sbyte? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments((short)5, (short)6)]
			[Arguments((short)5, (short)4)]
			public async Task ForNullableShort_WhenInsideTolerance_ShouldSucceed(
				short? subject, short? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((short)5, (short)7, -2)]
			[Arguments((short)5, (short)3, 2)]
			public async Task ForNullableShort_WhenOutsideTolerance_ShouldFail(
				short? subject, short? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForNullableShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short? subject, short? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments((uint)5, (uint)6)]
			[Arguments((uint)5, (uint)4)]
			public async Task ForNullableUint_WhenInsideTolerance_ShouldSucceed(
				uint? subject, uint? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((uint)5, (uint)7, -2)]
			[Arguments((uint)5, (uint)3, 2)]
			public async Task ForNullableUint_WhenOutsideTolerance_ShouldFail(
				uint? subject, uint? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ulong)5, (ulong)6)]
			[Arguments((ulong)5, (ulong)4)]
			public async Task ForNullableUlong_WhenInsideTolerance_ShouldSucceed(
				ulong? subject, ulong? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((ulong)5, (ulong)7, -2)]
			[Arguments((ulong)5, (ulong)3, 2)]
			public async Task ForNullableUlong_WhenOutsideTolerance_ShouldFail(
				ulong? subject, ulong? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments((ushort)5, (ushort)6)]
			[Arguments((ushort)5, (ushort)4)]
			public async Task ForNullableUshort_WhenInsideTolerance_ShouldSucceed(
				ushort? subject, ushort? expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((ushort)5, (ushort)7, -2)]
			[Arguments((ushort)5, (ushort)3, 2)]
			public async Task ForNullableUshort_WhenOutsideTolerance_ShouldFail(
				ushort? subject, ushort? expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(5, 6)]
			[Arguments(5, 4)]
			public async Task ForSbyte_WhenInsideTolerance_ShouldSucceed(
				sbyte subject, sbyte expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5, 7, -2)]
			[Arguments(5, 3, 2)]
			public async Task ForSbyte_WhenOutsideTolerance_ShouldFail(
				sbyte subject, sbyte expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForSbyte_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					sbyte subject, sbyte expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(5, 6)]
			[Arguments(5, 4)]
			public async Task ForShort_WhenInsideTolerance_ShouldSucceed(
				short subject, short expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5, 7, -2)]
			[Arguments(5, 3, 2)]
			public async Task ForShort_WhenOutsideTolerance_ShouldFail(
				short subject, short expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForShort_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					short subject, short expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(5, 6)]
			[Arguments(5, 4)]
			public async Task ForUint_WhenInsideTolerance_ShouldSucceed(
				uint subject, uint expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5, 7, -2)]
			[Arguments(5, 3, 2)]
			public async Task ForUint_WhenOutsideTolerance_ShouldFail(
				uint subject, uint expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(5, 6)]
			[Arguments(5, 4)]
			public async Task ForUlong_WhenInsideTolerance_ShouldSucceed(
				ulong subject, ulong expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5, 7, -2)]
			[Arguments(5, 3, 2)]
			public async Task ForUlong_WhenOutsideTolerance_ShouldFail(
				ulong subject, ulong expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(5, 6)]
			[Arguments(5, 4)]
			public async Task ForUshort_WhenInsideTolerance_ShouldSucceed(
				ushort subject, ushort expected)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(5, 7, -2)]
			[Arguments(5, 3, 2)]
			public async Task ForUshort_WhenOutsideTolerance_ShouldFail(
				ushort subject, ushort expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to {Formatter.Format(expected)} ± 1,
					              but it was 5, which differs by {expectedDifference}
					              """);
			}
		}
	}
}
