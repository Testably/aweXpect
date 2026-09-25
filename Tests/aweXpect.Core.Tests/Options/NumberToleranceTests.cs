using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class NumberToleranceTests
{
	[Fact]
	public async Task IsWithinTolerance_WhenBothAreNull_ShouldReturnTrue()
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));

		bool result = sut.IsWithinTolerance(null, null);

		await That(result).IsTrue();
	}

	[Theory]
	[InlineData(null, 1)]
	[InlineData(-3, null)]
	public async Task IsWithinTolerance_WhenOneIsNull_ShouldReturnFalse(int? actual, int? expected)
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));

		bool result = sut.IsWithinTolerance(actual, expected);

		await That(result).IsFalse();
	}

	[Theory]
	[InlineData(1, 2, 1, true)]
	[InlineData(1, 3, 1, false)]
	public async Task IsWithinTolerance_WhenValuesAreNotNull_ShouldApplyTolerance(
		int actual, int expected, int tolerance, bool expectedResult)
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));
		sut.SetTolerance(tolerance);

		bool result = sut.IsWithinTolerance(actual, expected);

		await That(result).IsEqualTo(expectedResult);
	}

	[Fact]
	public async Task WhenDoubleToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<double> sut = new((_, _) => null);

		void Act() => sut.SetTolerance(double.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
			.WithParamName("tolerance")
			.Because("NaN is neither negative nor a usable tolerance");
	}

	[Fact]
	public async Task WhenDoubleToleranceIsPositive_ShouldNotThrow()
	{
		NumberTolerance<double> sut = new((_, _) => null);

		void Act() => sut.SetTolerance(0.1);

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(0.1);
	}

	[Fact]
	public async Task WhenFloatToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<float> sut = new((_, _) => null);

		void Act() => sut.SetTolerance(float.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
			.WithParamName("tolerance")
			.Because("NaN is neither negative nor a usable tolerance");
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task WhenHalfToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<Half> sut = new((_, _) => null);

		void Act() => sut.SetTolerance(Half.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
			.WithParamName("tolerance")
			.Because("NaN is neither negative nor a usable tolerance");
	}
#endif

	[Fact]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<int> sut = new((_, _) => null);

		void Act() => sut.SetTolerance(-1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*The tolerance must not be negative.*").AsWildcard();
	}

	[Fact]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		NumberTolerance<int> sut = new((_, _) => null);

		void Act() => sut.SetTolerance(0);

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(0);
	}
}
