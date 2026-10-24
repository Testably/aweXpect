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
		where TSelf : INumberToleranceResult<TSelf, TNumber>
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		result.Options.Within(tolerance);
		return (TSelf)result;
	}

	/// <inheritdoc cref="Within{TSelf, TNumber}(INumberToleranceResult{TSelf, TNumber}, TNumber)" />
	/// <remarks>
	///     An <see langword="int" /> literal, e.g. in <c>Within(1)</c>, cannot be inferred as a
	///     <see langword="byte" /> tolerance, so this overload fixes the type.
	/// </remarks>
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, byte> result, byte tolerance)
		where TSelf : INumberToleranceResult<TSelf, byte>
		=> result.Within<TSelf, byte>(tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, sbyte> result, sbyte tolerance)
		where TSelf : INumberToleranceResult<TSelf, sbyte>
		=> result.Within<TSelf, sbyte>(tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, short> result, short tolerance)
		where TSelf : INumberToleranceResult<TSelf, short>
		=> result.Within<TSelf, short>(tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, ushort> result, ushort tolerance)
		where TSelf : INumberToleranceResult<TSelf, ushort>
		=> result.Within<TSelf, ushort>(tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, uint> result, uint tolerance)
		where TSelf : INumberToleranceResult<TSelf, uint>
		=> result.Within<TSelf, uint>(tolerance);

	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, ulong> result, ulong tolerance)
		where TSelf : INumberToleranceResult<TSelf, ulong>
		=> result.Within<TSelf, ulong>(tolerance);

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Within{TSelf}(INumberToleranceResult{TSelf, byte}, byte)" />
	public static TSelf Within<TSelf>(this INumberToleranceResult<TSelf, nuint> result, nuint tolerance)
		where TSelf : INumberToleranceResult<TSelf, nuint>
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
		result.Options.Within(tolerance);
		return result;
	}

	/// <summary>
	///     …interpreting the tolerance as a percentage of the expected value.
	/// </summary>
	/// <remarks>
	///     On an expected value of zero, only an equal value is within the tolerance.
	/// </remarks>
	public static TResult Percent<TResult>(this IWithinToleranceResult<TResult, double> result)
	{
		result.Options.UseRelativeTolerance(static (tolerance, expected) => tolerance * Math.Abs(expected) / 100,
			"%");
		return result.Result;
	}

	/// <summary>
	///     …interpreting the tolerance as a percentage of the expected value.
	/// </summary>
	/// <remarks>
	///     On an expected value of zero, only an equal value is within the tolerance.
	/// </remarks>
	public static TResult Percent<TResult>(this IWithinToleranceResult<TResult, float> result)
	{
		result.Options.UseRelativeTolerance(static (tolerance, expected) => tolerance * Math.Abs(expected) / 100,
			"%");
		return result.Result;
	}

	/// <summary>
	///     …interpreting the tolerance as a percentage of the expected value.
	/// </summary>
	/// <remarks>
	///     On an expected value of zero, only an equal value is within the tolerance.
	/// </remarks>
	public static TResult Percent<TResult>(this IWithinToleranceResult<TResult, decimal> result)
	{
		result.Options.UseRelativeTolerance(RelativeDecimalTolerance, "%");
		return result.Result;
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Percent{TResult}(IWithinToleranceResult{TResult, double})" />
	public static TResult Percent<TResult>(this IWithinToleranceResult<TResult, Half> result)
	{
		result.Options.UseRelativeTolerance(
			static (tolerance, expected) => (Half)((float)tolerance * Math.Abs((float)expected) / 100), "%");
		return result.Result;
	}
#endif

	/// <remarks>
	///     A tolerance beyond the range of <see langword="decimal" /> covers every difference that is representable.
	/// </remarks>
	private static decimal RelativeDecimalTolerance(decimal tolerance, decimal expected)
	{
		try
		{
			return tolerance * Math.Abs(expected) / 100;
		}
		catch (OverflowException)
		{
			return decimal.MaxValue;
		}
	}
}
