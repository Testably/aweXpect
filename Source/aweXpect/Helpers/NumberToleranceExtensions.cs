using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

namespace aweXpect.Helpers;

internal static class NumberToleranceExtensions
{
	public static bool IsGreaterThan<TNumber>(
		this NumberTolerance<TNumber> tolerance, TNumber? actual, TNumber? expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (!IsComparable(actual) || !IsComparable(expected))
		{
			return false;
		}

		int cmp = actual!.Value.CompareTo(expected!.Value);
		if (tolerance.Tolerance is null)
		{
			return cmp > 0;
		}

		if (cmp > 0)
		{
			return true;
		}

		return IsStrictlyWithinTolerance(tolerance, actual.Value, expected.Value, tolerance.Tolerance.Value);
	}

	public static bool IsGreaterThanOrEqualTo<TNumber>(
		this NumberTolerance<TNumber> tolerance, TNumber? actual, TNumber? expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (!IsComparable(actual) || !IsComparable(expected))
		{
			return false;
		}

		int cmp = actual!.Value.CompareTo(expected!.Value);
		if (cmp >= 0)
		{
			return true;
		}

		if (tolerance.Tolerance is null)
		{
			return false;
		}

		TNumber? diff = TryCalculateDifference(tolerance, actual.Value, expected.Value);
		return diff is not null && diff.Value.CompareTo(tolerance.Tolerance.Value) <= 0;
	}

	public static bool IsInRange<TNumber>(
		this NumberTolerance<TNumber> tolerance, TNumber? actual, TNumber? minimum, TNumber? maximum)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> tolerance.IsGreaterThanOrEqualTo(actual, minimum) && tolerance.IsLessThanOrEqualTo(actual, maximum);

	public static bool IsLessThan<TNumber>(
		this NumberTolerance<TNumber> tolerance, TNumber? actual, TNumber? expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (!IsComparable(actual) || !IsComparable(expected))
		{
			return false;
		}

		int cmp = actual!.Value.CompareTo(expected!.Value);
		if (tolerance.Tolerance is null)
		{
			return cmp < 0;
		}

		if (cmp < 0)
		{
			return true;
		}

		return IsStrictlyWithinTolerance(tolerance, actual.Value, expected.Value, tolerance.Tolerance.Value);
	}

	public static bool IsLessThanOrEqualTo<TNumber>(
		this NumberTolerance<TNumber> tolerance, TNumber? actual, TNumber? expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (!IsComparable(actual) || !IsComparable(expected))
		{
			return false;
		}

		int cmp = actual!.Value.CompareTo(expected!.Value);
		if (cmp <= 0)
		{
			return true;
		}

		if (tolerance.Tolerance is null)
		{
			return false;
		}

		TNumber? diff = TryCalculateDifference(tolerance, actual.Value, expected.Value);
		return diff is not null && diff.Value.CompareTo(tolerance.Tolerance.Value) <= 0;
	}

	public static void ThrowIfNaN<TNumber>(this TNumber? value,
		string? description = null,
		[CallerArgumentExpression(nameof(value))] string? paramName = null)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (IsNaN(value))
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(paramName, $"The {description ?? paramName} must not be NaN."));
		}
	}

	private static bool IsNaN<TNumber>(TNumber? value)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
		=> value is not null && TNumber.IsNaN(value.Value);
#else
		where TNumber : struct, IComparable<TNumber>
		=> value switch
		{
			double d => double.IsNaN(d),
			float f => float.IsNaN(f),
			_ => false,
		};
#endif

	/// <summary>
	///     Infinite values compare correctly, but <c>NaN</c> sorts below everything in <see cref="IComparable{T}" />
	///     and would otherwise satisfy any lower bound.
	/// </summary>
	private static bool IsComparable<TNumber>(TNumber? value)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> value is not null && !IsNaN(value);

	private static bool IsInfinity<TNumber>(TNumber value)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
		=> TNumber.IsInfinity(value);
#else
		where TNumber : struct, IComparable<TNumber>
		=> value switch
		{
			double d => double.IsInfinity(d),
			float f => float.IsInfinity(f),
			_ => false,
		};
#endif

	/// <remarks>
	///     The distance has to be strictly smaller than the tolerance, so that a strict comparison shifts its bound
	///     without becoming inclusive, like the time comparisons. An infinite bound stays infinite when shifted, so
	///     nothing that is not already beyond it is within reach.
	/// </remarks>
	private static bool IsStrictlyWithinTolerance<TNumber>(
		NumberTolerance<TNumber> tolerance, TNumber actual, TNumber expected, TNumber toleranceValue)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		if (IsInfinity(expected))
		{
			return false;
		}

		TNumber? diff = TryCalculateDifference(tolerance, actual, expected);
		return diff is not null && diff.Value.CompareTo(toleranceValue) < 0;
	}

	private static TNumber? TryCalculateDifference<TNumber>(
		NumberTolerance<TNumber> tolerance, TNumber actual, TNumber expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		try
		{
			return tolerance.CalculateDifference(actual, expected);
		}
		catch (OverflowException)
		{
			return null;
		}
	}
}
