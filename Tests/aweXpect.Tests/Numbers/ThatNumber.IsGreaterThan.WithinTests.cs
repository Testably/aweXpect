#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsGreaterThan
	{
		public sealed class WithinTests
		{
			[Test]
			[Arguments((byte)5, (byte)5)]
			[Arguments((byte)6, (byte)5)]
			public async Task ForByte_WhenInsideTolerance_ShouldSucceed(
				byte subject, byte expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments((byte)5, (byte)6, "-1")]
			[Arguments((byte)5, (byte)7, "-2")]
			[Arguments((byte)0, (byte)5, "-5")]
			public async Task ForByte_WhenOutsideTolerance_ShouldFail(
				byte subject, byte expected, string expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(12.5, 12.5)]
			[Arguments(12.6, 12.5)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(12.0, 12.5, "-0.5")]
			[Arguments(12.5, 12.6, "-0.1")]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldFail(
				double subjectValue, double expectedValue, string expectedDifference)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(new decimal(0.1));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, decimal expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			public async Task ForDouble_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldSucceed()
			{
				double subject = double.MinValue;
				double expected = double.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(double.PositiveInfinity);

				await That(Act).DoesNotThrow()
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForDouble_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldFail()
			{
				double subject = double.MinValue;
				double expected = double.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(double.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than double.MaxValue ± double.MaxValue,
					             but it was double.MinValue
					             """)
					.Because("a distance that is not representable exceeds every finite tolerance");
			}

			[Test]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 5.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1.0);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("no tolerance can bring a value within reach of NaN");
			}

			[Test]
			[Arguments(12.5, 12.5)]
			[Arguments(12.5, 12.6)]
			[Arguments(12.6, 12.5)]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed(
				double subject, double expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(12.0, 12.5)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail(
				double subject, double expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(0.1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}, which differs by -0.5
					              """);
			}

			[Test]
			public async Task ForDouble_WhenSubjectAndExpectedArePositiveInfinity_ShouldFail()
			{
				double subject = double.PositiveInfinity;
				double expected = double.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1.0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than +∞ ± 1.0,
					             but it was +∞
					             """)
					.Because("subtracting a tolerance from infinity leaves infinity, which is not greater than itself");
			}

			[Test]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double subject = double.NaN;
				double expected = 5.0;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1.0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5.0 ± 1.0,
					             but it was NaN
					             """);
			}

			[Test]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsGreaterThan(12.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Test]
			public async Task ForFloat_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldSucceed()
			{
				float subject = float.MinValue;
				float expected = float.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(float.PositiveInfinity);

				await That(Act).DoesNotThrow()
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForFloat_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldFail()
			{
				float subject = float.MinValue;
				float expected = float.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(float.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than float.MaxValue ± float.MaxValue,
					             but it was float.MinValue, which differs by -6.80564693277058E+38
					             """)
					.Because("a distance that is not representable exceeds every finite tolerance");
			}

			[Test]
			[Arguments(12.5f, 12.5f)]
			[Arguments(12.5f, 12.6f)]
			[Arguments(12.6f, 12.5f)]
			public async Task ForFloat_WhenInsideTolerance_ShouldSucceed(
				float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(0.11f);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(12.0f, 12.5f)]
			public async Task ForFloat_WhenOutsideTolerance_ShouldFail(
				float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(0.1f);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}, which differs by -0.5
					              """);
			}

			[Test]
			public async Task ForFloat_WhenSubjectAndExpectedArePositiveInfinity_ShouldFail()
			{
				float subject = float.PositiveInfinity;
				float expected = float.PositiveInfinity;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1.0f);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than +∞ ± 1.0,
					             but it was +∞
					             """)
					.Because("subtracting a tolerance from infinity leaves infinity, which is not greater than itself");
			}

			[Test]
			[Arguments(5, 5)]
			[Arguments(6, 5)]
			public async Task ForInt_WhenInsideTolerance_ShouldSucceed(int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForInt_WhenNegatedAndOnToleranceBoundary_ShouldSucceed()
			{
				int subject = 4;
				int expected = 5;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsGreaterThan(expected).Within(1));

				await That(Act).DoesNotThrow()
					.Because("the negation is the exact complement of the strict inequality");
			}

			[Test]
			[Arguments(3, 5, -2)]
			[Arguments(0, 5, -5)]
			[Arguments(4, 5, -1)]
			public async Task ForInt_WhenOutsideTolerance_ShouldFail(int subject, int expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[AutoArguments]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*The tolerance must not be negative.*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Test]
			[Arguments(5L, 5L)]
			[Arguments(6L, 5L)]
			public async Task ForLong_WhenInsideTolerance_ShouldSucceed(long subject, long expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1L);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(3L, 5L, -2)]
			[Arguments(5L, 6L, -1)]
			public async Task ForLong_WhenOutsideTolerance_ShouldFail(long subject, long expected, int expectedDifference)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1L);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is greater than {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}, which differs by {expectedDifference}
					              """);
			}

			[Test]
			[Arguments(5, 5)]
			[Arguments(6, 5)]
			public async Task ForNullableInt_WhenInsideTolerance_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ForNullableInt_WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;
				int? expected = 5;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5 ± 1,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task ForSByte_WhenDifferenceWouldOverflow_ShouldFailWithoutThrowing()
			{
				sbyte subject = sbyte.MinValue;
				sbyte expected = sbyte.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 127 ± 1,
					             but it was -128, which differs by -255
					             """)
					.Because("the difference must not overflow the range of sbyte");
			}

			[Test]
			public async Task WhenToleranceIsNotSet_ShouldUseStrictInequality()
			{
				int subject = 5;
				int expected = 5;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5,
					             but it was 5
					             """);
			}

			[Test]
			public async Task WhenToleranceIsZero_ShouldUseStrictInequality()
			{
				int subject = 5;
				int expected = 5;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5 ± 0,
					             but it was 5
					             """)
					.Because("a tolerance widens the bound, but does not make the comparison inclusive");
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForHalf_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldSucceed()
			{
				Half subject = Half.MinValue;
				Half expected = Half.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(Half.PositiveInfinity);

				await That(Act).DoesNotThrow()
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForHalf_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldFail()
			{
				Half subject = Half.MinValue;
				Half expected = Half.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(Half.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than Half.MaxValue ± Half.MaxValue,
					             but it was Half.MinValue, which differs by -131008.0
					             """)
					.Because("a distance that is not representable exceeds every finite tolerance");
			}
#endif

#if NET8_0_OR_GREATER
			[Test]
			public async Task ForNFloat_WhenDistanceOverflowsAndToleranceIsInfinite_ShouldSucceed()
			{
				NFloat subject = NFloat.MinValue;
				NFloat expected = NFloat.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(NFloat.PositiveInfinity);

				await That(Act).DoesNotThrow()
					.Because("every finite distance is below an infinite tolerance, even when it is not representable");
			}

			[Test]
			public async Task ForNFloat_WhenDistanceOverflowsAndToleranceIsMaxValue_ShouldFail()
			{
				NFloat subject = NFloat.MinValue;
				NFloat expected = NFloat.MaxValue;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected).Within(NFloat.MaxValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than NFloat.MaxValue ± NFloat.MaxValue,
					             but it was NFloat.MinValue
					             """)
					.Because("a distance that is not representable exceeds every finite tolerance");
			}
#endif
		}
	}
}
