#if !NET8_0_OR_GREATER
using System;

namespace aweXpect.Helpers;

/// <summary>
///     The classification of a floating point type of the target frameworks without <c>INumber&lt;T&gt;</c>.
/// </summary>
internal sealed class FloatingPointTraits<TNumber>(Func<TNumber, bool> isNaN, Func<TNumber, bool> isInfinity)
{
	public Func<TNumber, bool> IsNaN { get; } = isNaN;
	public Func<TNumber, bool> IsInfinity { get; } = isInfinity;

	public bool IsFinite(TNumber value) => !IsInfinity(value) && !IsNaN(value);
}

/// <summary>
///     The <see cref="FloatingPointTraits{TNumber}" /> of each floating point type of the target frameworks without
///     <c>INumber&lt;T&gt;</c>, where the classification has to be called per type. The generator emits one overload
///     of every classification expectation per method.
/// </summary>
internal static class FloatingPointNumberFactory
{
	public static FloatingPointTraits<float> CreateFloat() => new(float.IsNaN, float.IsInfinity);

	public static FloatingPointTraits<double> CreateDouble() => new(double.IsNaN, double.IsInfinity);
}
#endif
