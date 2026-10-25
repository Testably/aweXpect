using System;

namespace aweXpect.Core.Helpers;

/// <summary>
///     The differences with which a <see cref="Options.NumberTolerance{TNumber}" /> of the property results compares,
///     like the number expectations of aweXpect do.
/// </summary>
internal static class NumberDifferences
{
	/// <remarks>
	///     A non-finite value has no representable distance to a value it is not equal to.
	/// </remarks>
	public static double? Double(double actual, double expected)
		=> IsFinite(actual) && IsFinite(expected) ? Math.Abs(actual - expected) : null;

	/// <remarks>
	///     A difference beyond the range of <see langword="decimal" /> throws an <see cref="OverflowException" />, which
	///     the tolerance treats as not within it.
	/// </remarks>
	public static decimal? Decimal(decimal actual, decimal expected)
		=> actual > expected ? actual - expected : expected - actual;

	private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
}
