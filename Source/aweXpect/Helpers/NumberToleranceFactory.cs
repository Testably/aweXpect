#if !NET8_0_OR_GREATER
using System;
using aweXpect.Options;

namespace aweXpect.Helpers;

/// <summary>
///     The <see cref="NumberTolerance{TNumber}" /> of each numeric type of the target frameworks without
///     <c>INumber&lt;T&gt;</c>, where the difference has to be computed per type. The generator emits one overload of
///     every number expectation per method.
/// </summary>
internal static class NumberToleranceFactory
{
	public static NumberTolerance<byte> CreateByte()
		=> new((a, e) => { checked { return (byte)(a > e ? a - e : e - a); } });

	public static NumberTolerance<sbyte> CreateSByte()
		=> new((a, e) => { checked { return (sbyte)(a > e ? a - e : e - a); } });

	public static NumberTolerance<short> CreateShort()
		=> new((a, e) => { checked { return (short)(a > e ? a - e : e - a); } });

	public static NumberTolerance<ushort> CreateUShort()
		=> new((a, e) => { checked { return (ushort)(a > e ? a - e : e - a); } });

	public static NumberTolerance<int> CreateInt()
		=> new((a, e) => { checked { return a > e ? a - e : e - a; } });

	public static NumberTolerance<uint> CreateUInt()
		=> new((a, e) => { checked { return a > e ? a - e : e - a; } });

	public static NumberTolerance<long> CreateLong()
		=> new((a, e) => { checked { return a > e ? a - e : e - a; } });

	public static NumberTolerance<ulong> CreateULong()
		=> new((a, e) => { checked { return a > e ? a - e : e - a; } });

	/// <remarks>
	///     Only the floating point types can be non-finite, and a non-finite value has no representable distance to a
	///     value it is not equal to.
	/// </remarks>
	public static NumberTolerance<float> CreateFloat()
		=> new((a, e) => IsFinite(a) && IsFinite(e) ? Math.Abs(a - e) : null);

	public static NumberTolerance<double> CreateDouble()
		=> new((a, e) => IsFinite(a) && IsFinite(e) ? Math.Abs(a - e) : null);

	public static NumberTolerance<decimal> CreateDecimal()
		=> new((a, e) => { checked { return a > e ? a - e : e - a; } });

	private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

	private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
}
#endif
