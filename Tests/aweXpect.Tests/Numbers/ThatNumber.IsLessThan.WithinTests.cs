#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed partial class IsLessThan
	{
		public sealed class WithinTests
		{
			[Theory]
			[InlineData((byte)5, (byte)5)]
			[InlineData((byte)5, (byte)6)]
			public async Task ForByte_WhenInsideTolerance_ShouldSucceed(
				byte subject, byte expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData((byte)6, (byte)5)]
			[InlineData((byte)7, (byte)5)]
			[InlineData((byte)10, (byte)5)]
			public async Task ForByte_WhenOutsideTolerance_ShouldFail(
				byte subject, byte expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData(12.5, 12.5)]
			[InlineData(12.5, 12.6)]
			public async Task ForDecimal_WhenInsideTolerance_ShouldSucceed(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(new decimal(0.1));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(12.6, 12.5)]
			[InlineData(13.0, 12.5)]
			public async Task ForDecimal_WhenOutsideTolerance_ShouldFail(
				double subjectValue, double expectedValue)
			{
				decimal subject = new(subjectValue);
				decimal expected = new(expectedValue);

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(new decimal(0.1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task
				ForDecimal_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					decimal subject, decimal expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(new decimal(-0.1));

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Fact]
			public async Task ForDouble_WhenExpectedIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				double subject = 5.0;
				double expected = double.NaN;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1.0);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("expected").And
					.WithMessage("The expected value must not be NaN.").AsPrefix()
					.Because("no tolerance can bring a value within reach of NaN");
			}

			[Theory]
			[InlineData(12.5, 12.5)]
			[InlineData(12.5, 12.6)]
			[InlineData(12.6, 12.5)]
			public async Task ForDouble_WhenInsideTolerance_ShouldSucceed(
				double subject, double expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(0.1);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(13.0, 12.5)]
			public async Task ForDouble_WhenOutsideTolerance_ShouldFail(
				double subject, double expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(0.1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForDouble_WhenSubjectAndExpectedAreNegativeInfinity_ShouldFail()
			{
				double subject = double.NegativeInfinity;
				double expected = double.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1.0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than -∞ ± 1.0,
					             but it was -∞
					             """)
					.Because("adding a tolerance to negative infinity leaves negative infinity, which is not less than itself");
			}

			[Fact]
			public async Task ForDouble_WhenSubjectIsNaN_ShouldFail()
			{
				double subject = double.NaN;
				double expected = 5.0;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1.0);

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task ForDouble_WhenToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
			{
				async Task Act()
					=> await That(12.5).IsLessThan(13.0).Within(double.NaN);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("Tolerance must not be NaN*").AsWildcard().And
					.WithParamName("tolerance")
					.Because("NaN is not a negative tolerance, so it needs its own message");
			}

			[Theory]
			[InlineData(12.5f, 12.5f)]
			[InlineData(12.5f, 12.6f)]
			[InlineData(12.6f, 12.5f)]
			public async Task ForFloat_WhenInsideTolerance_ShouldSucceed(
				float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(0.11f);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(13.0f, 12.5f)]
			public async Task ForFloat_WhenOutsideTolerance_ShouldFail(
				float subject, float expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(0.1f);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)} ± 0.1,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task ForFloat_WhenSubjectAndExpectedAreNegativeInfinity_ShouldFail()
			{
				float subject = float.NegativeInfinity;
				float expected = float.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1.0f);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than -∞ ± 1.0,
					             but it was -∞
					             """)
					.Because("adding a tolerance to negative infinity leaves negative infinity, which is not less than itself");
			}

			[Theory]
			[InlineData(5, 5)]
			[InlineData(5, 6)]
			public async Task ForInt_WhenInsideTolerance_ShouldSucceed(int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForInt_WhenNegatedAndOnToleranceBoundary_ShouldSucceed()
			{
				int subject = 6;
				int expected = 5;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLessThan(expected).Within(1));

				await That(Act).DoesNotThrow()
					.Because("the negation is the exact complement of the strict inequality");
			}

			[Theory]
			[InlineData(6, 5)]
			[InlineData(7, 5)]
			[InlineData(10, 5)]
			public async Task ForInt_WhenOutsideTolerance_ShouldFail(int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task
				ForInt_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException(
					int subject, int expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(-1);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithMessage("*Tolerance must be non-negative*").AsWildcard().And
					.WithParamName("tolerance");
			}

			[Theory]
			[InlineData(5L, 5L)]
			[InlineData(5L, 6L)]
			public async Task ForLong_WhenInsideTolerance_ShouldSucceed(long subject, long expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1L);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(6L, 5L)]
			[InlineData(7L, 5L)]
			public async Task ForLong_WhenOutsideTolerance_ShouldFail(long subject, long expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1L);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is less than {Formatter.Format(expected)} ± 1,
					              but it was {Formatter.Format(subject)}
					              """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForNFloat_WhenSubjectAndExpectedAreNegativeInfinity_ShouldFail()
			{
				NFloat subject = NFloat.NegativeInfinity;
				NFloat expected = NFloat.NegativeInfinity;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than -∞ ± 1,
					             but it was -∞
					             """)
					.Because("adding a tolerance to negative infinity leaves negative infinity, which is not less than itself");
			}
#endif

			[Theory]
			[InlineData(5, 5)]
			[InlineData(5, 6)]
			public async Task ForNullableInt_WhenInsideTolerance_ShouldSucceed(
				int? subject, int? expected)
			{
				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForNullableInt_WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;
				int? expected = 5;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(1);

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task ForSByte_WhenDifferenceWouldOverflow_ShouldFailWithoutThrowing()
			{
				sbyte subject = sbyte.MaxValue;
				sbyte expected = sbyte.MinValue;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within((sbyte)1);

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenToleranceIsNotSet_ShouldUseStrictInequality()
			{
				int subject = 5;
				int expected = 5;

				async Task Act()
					=> await That(subject).IsLessThan(expected);

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenToleranceIsZero_ShouldUseStrictInequality()
			{
				int subject = 5;
				int expected = 5;

				async Task Act()
					=> await That(subject).IsLessThan(expected).Within(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is less than 5 ± 0,
					             but it was 5
					             """)
					.Because("a tolerance widens the bound, but does not make the comparison inclusive");
			}
		}
	}
}
