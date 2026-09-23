#if !NET8_0_OR_GREATER
using System;

namespace aweXpect.Helpers;

/// <summary>
///     The sign predicates of a signed numeric type of the target frameworks without <c>INumber&lt;T&gt;</c>.
/// </summary>
internal sealed class NumberSign<TNumber>(Func<TNumber, bool> isPositive, Func<TNumber, bool> isNegative)
{
	public Func<TNumber, bool> IsPositive { get; } = isPositive;
	public Func<TNumber, bool> IsNegative { get; } = isNegative;
}

/// <summary>
///     The <see cref="NumberSign{TNumber}" /> of each signed numeric type of the target frameworks without
///     <c>INumber&lt;T&gt;</c>, where the comparison has to be written per type. The generator emits one overload of
///     every sign expectation per method.
/// </summary>
internal static class SignedNumberFactory
{
	public static NumberSign<sbyte> CreateSByte() => new(a => a > 0, a => a < 0);

	public static NumberSign<short> CreateShort() => new(a => a > 0, a => a < 0);

	public static NumberSign<int> CreateInt() => new(a => a > 0, a => a < 0);

	public static NumberSign<long> CreateLong() => new(a => a > 0, a => a < 0);

	public static NumberSign<float> CreateFloat() => new(a => a > 0, a => a < 0);

	public static NumberSign<double> CreateDouble() => new(a => a > 0, a => a < 0);

	public static NumberSign<decimal> CreateDecimal() => new(a => a > 0, a => a < 0);
}
#endif
