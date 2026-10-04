using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace aweXpect;

/// <summary>
///     Extension methods for results that compare numbers or times within a tolerance.
/// </summary>
/// <remarks>
///     The tolerance can be specified only once.
/// </remarks>
public static class ToleranceExtensions
{
	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the number comparison.
	/// </summary>
	/// <remarks>
	///     The tolerance relaxes the bounds of equality, ordering and range expectations, and a subject exactly the
	///     tolerance away matches wherever the bound itself matches, for example on <c>IsEqualTo</c> or
	///     <c>IsGreaterThanOrEqualTo</c>, but not on <c>IsGreaterThan</c>.
	/// </remarks>
	public static TSelf Within<TSelf, TNumber>(this INumberToleranceResult<TSelf, TNumber> result, TNumber tolerance)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		result.Options.SetTolerance(tolerance);
		return (TSelf)result;
	}

	/// <inheritdoc cref="Within{TSelf, TNumber}(INumberToleranceResult{TSelf, TNumber}, TNumber)" />
	/// <remarks>
	///     An <see langword="int" /> literal, e.g. in <c>Within(1)</c>, cannot be inferred as a
	///     <see langword="byte" /> tolerance, so this overload fixes the type.
	/// </remarks>
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, byte> result, byte tolerance)
		=> Within<TSelf, byte>(result, tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, sbyte> result, sbyte tolerance)
		=> Within<TSelf, sbyte>(result, tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, short> result, short tolerance)
		=> Within<TSelf, short>(result, tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, ushort> result, ushort tolerance)
		=> Within<TSelf, ushort>(result, tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, uint> result, uint tolerance)
		=> Within<TSelf, uint>(result, tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, ulong> result, ulong tolerance)
		=> Within<TSelf, ulong>(result, tolerance);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, nuint> result, nuint tolerance)
		=> Within<TSelf, nuint>(result, tolerance);
#endif

	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the time comparison.
	/// </summary>
	/// <remarks>
	///     The tolerance relaxes the bounds of equality, ordering and range expectations, and a subject exactly the
	///     tolerance away matches wherever the bound itself matches, for example on <c>IsEqualTo</c> or
	///     <c>IsOnOrAfter</c>, but not on <c>IsAfter</c>. A negated expectation, such as <c>IsNotAfter</c>,
	///     <c>IsNotBetween</c> or <c>IsNotEqualTo</c>, is the exact inverse of its positive form, so the tolerance
	///     narrows it and it fails within the tolerance of the bound. On a <c>DateOnly</c>,
	///     a <paramref name="tolerance" /> that is not a whole number of days throws an
	///     <see cref="ArgumentOutOfRangeException" />; an expectation of an extension gets this check by passing a
	///     <see cref="DayTolerance" /> as its options.
	/// </remarks>
	public static TResult Within<TResult>(this TResult result, TimeSpan tolerance)
		where TResult : IOptionsProvider<TimeTolerance>
	{
		result.Options.SetTolerance(tolerance);
		return result;
	}
}
