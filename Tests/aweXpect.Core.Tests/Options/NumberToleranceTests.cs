using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class NumberToleranceTests
{
	[Test]
	[Arguments(null, 1)]
	[Arguments(1, null)]
	public async Task CalculateDifference_WhenOneIsNull_ShouldReturnNull(int? actual, int? expected)
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));

		int? result = sut.CalculateDifference(actual, expected);

		await That(result).IsNull();
	}

	[Test]
	public async Task IsWithinTolerance_WhenBothAreNull_ShouldReturnTrue()
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));

		bool result = sut.IsWithinTolerance(null, null);

		await That(result).IsTrue();
	}

	[Test]
	[Arguments(null, 1)]
	[Arguments(-3, null)]
	public async Task IsWithinTolerance_WhenOneIsNull_ShouldReturnFalse(int? actual, int? expected)
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));

		bool result = sut.IsWithinTolerance(actual, expected);

		await That(result).IsFalse();
	}

	[Test]
	public async Task IsWithinTolerance_WhenTheDifferenceOverflows_ShouldReturnFalse()
	{
		NumberTolerance<int> sut = new((a, b) => checked(a - b));
		sut.Within(1);

		bool result = sut.IsWithinTolerance(int.MinValue, int.MaxValue);

		await That(result).IsFalse()
			.Because("a difference that does not fit into the type is larger than any tolerance");
	}

	[Test]
	[Arguments(1, 2, 1, true)]
	[Arguments(1, 3, 1, false)]
	public async Task IsWithinTolerance_WhenValuesAreNotNull_ShouldApplyTolerance(
		int actual, int expected, int tolerance, bool expectedResult)
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));
		sut.Within(tolerance);

		bool result = sut.IsWithinTolerance(actual, expected);

		await That(result).IsEqualTo(expectedResult);
	}

	[Test]
	[Arguments(-105, -100, true)]
	[Arguments(105, 100, true)]
	[Arguments(106, 100, false)]
	public async Task IsWithinTolerance_WithRelativeTolerance_ShouldApplyItToTheExpectedValue(
		int actual, int expected, bool expectedResult)
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));
		sut.Within(5);
		sut.UseRelativeTolerance((t, e) => t * Math.Abs(e) / 100, "%");

		bool result = sut.IsWithinTolerance(actual, expected);

		await That(result).IsEqualTo(expectedResult);
	}

	[Test]
	public async Task ToleranceFor_WithoutRelativeTolerance_ShouldReturnTheTolerance()
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));
		sut.Within(5);

		int? result = sut.ToleranceFor(200);

		await That(result).IsEqualTo(5);
	}

	[Test]
	public async Task ToleranceFor_WithoutTolerance_ShouldReturnNull()
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));
		sut.UseRelativeTolerance((t, e) => t * e / 100, "%");

		int? result = sut.ToleranceFor(200);

		await That(result).IsNull();
	}

	[Test]
	public async Task ToleranceFor_WithRelativeTolerance_ShouldCalculateTheAbsoluteTolerance()
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));
		sut.Within(5);
		sut.UseRelativeTolerance((t, e) => t * e / 100, "%");

		int? result = sut.ToleranceFor(200);

		await That(result).IsEqualTo(10);
		await That(sut.Tolerance).IsEqualTo(5)
			.Because("the specified tolerance is kept and only interpreted relative to the expected value");
	}

	[Test]
	public async Task ToString_WhenToleranceIsAChar_ShouldFormatItAsNumber()
	{
		NumberTolerance<char> sut = new((_, _) => null);
		sut.Within('\u0001');

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 1")
			.Because("a tolerance formatted as char would be an unreadable character");
	}

	[Test]
	public async Task ToString_WithRelativeTolerance_ShouldAppendTheUnit()
	{
		NumberTolerance<int> sut = new((a, b) => Math.Abs(a - b));
		sut.Within(5);
		sut.UseRelativeTolerance((t, e) => t * e / 100, "%");

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 5%");
	}

	[Test]
	public async Task WhenDoubleToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<double> sut = new((_, _) => null);

		void Act() => sut.Within(double.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
			.WithParamName("tolerance")
			.Because("NaN is neither negative nor a usable tolerance");
	}

	[Test]
	public async Task WhenDoubleToleranceIsPositive_ShouldNotThrow()
	{
		NumberTolerance<double> sut = new((_, _) => null);

		void Act() => sut.Within(0.1);

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(0.1);
	}

	[Test]
	public async Task WhenFloatToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<float> sut = new((_, _) => null);

		void Act() => sut.Within(float.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
			.WithParamName("tolerance")
			.Because("NaN is neither negative nor a usable tolerance");
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task WhenHalfToleranceIsNaN_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<Half> sut = new((_, _) => null);

		void Act() => sut.Within(Half.NaN);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must not be NaN.*").AsWildcard().And
			.WithParamName("tolerance")
			.Because("NaN is neither negative nor a usable tolerance");
	}
#endif

	[Test]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		NumberTolerance<int> sut = new((_, _) => null);

		void Act() => sut.Within(-1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*The tolerance must not be negative.*").AsWildcard();
	}

	[Test]
	public async Task WhenToleranceIsSetTwice_ShouldUseTheLastTolerance()
	{
		NumberTolerance<int> sut = new((_, _) => null);
		sut.Within(1);
		sut.UseRelativeTolerance((tolerance, expected) => tolerance * expected / 100, "%");

		sut.Within(2);

		await That(sut.Tolerance).IsEqualTo(2);
		await That(sut.ToleranceFor(100)).IsEqualTo(2)
			.Because("the later tolerance also replaces the earlier relative tolerance");
		await That(sut.ToString()).DoesNotEndWith("%");
	}

	[Test]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		NumberTolerance<int> sut = new((_, _) => null);

		void Act() => sut.Within(0);

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(0);
	}

	[Test]
	public async Task Within_ShouldReturnTheSameTolerance()
	{
		NumberTolerance<int> sut = new((_, _) => null);

		NumberTolerance<int> result = sut.Within(1);

		await That(result).IsSameAs(sut);
	}
}
