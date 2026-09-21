using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ThrowHelper
{
	public static ArgumentException EmptyCollection()
		=> new("You have to provide at least one expected value!");

	/// <summary>
	///     Rejects an empty set of expected <paramref name="values" /> and returns them materialized, so that a
	///     sequence which can only be enumerated once survives both the guard and the subsequent comparison.
	/// </summary>
	public static IReadOnlyList<T> EnsureNotEmpty<T>(IEnumerable<T> values)
	{
		IReadOnlyList<T> materializedValues = values as IReadOnlyList<T> ?? values.ToList();
		if (materializedValues.Count == 0)
		{
			throw Tracing.WriteException(EmptyCollection());
		}

		return materializedValues;
	}

	/// <summary>
	///     Rejects an inverted range, so that a tolerance cannot silently turn it into a satisfiable one.
	/// </summary>
	public static void ThrowIfMaximumIsBelowMinimum<T>(T? minimum, T? maximum)
		where T : struct, IComparable<T>
	{
		if (minimum is not null && maximum is not null && maximum.Value.CompareTo(minimum.Value) < 0)
		{
			// ReSharper disable once LocalizableElement
			throw new ArgumentOutOfRangeException(nameof(maximum),
				"The maximum must be greater than or equal to the minimum.");
		}
	}

	/// <summary>
	///     Rejects a recursion depth below one at the call site, so that the exception names the caller's parameter
	///     instead of the option that is set from it.
	/// </summary>
	public static void ThrowIfRecursionDepthIsNotPositive(int maximumRecursionDepth)
	{
		if (maximumRecursionDepth < 1)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(maximumRecursionDepth), maximumRecursionDepth,
					"The maximum recursion depth must be greater than zero."));
		}
	}

	/// <summary>
	///     Rejects a tolerance with a sub-day remainder, because a date without a time of day cannot honour it and
	///     would silently drop it.
	/// </summary>
	public static void ThrowIfToleranceIsNotWholeDays(TimeSpan? tolerance)
	{
		if (tolerance is not null && tolerance.Value.Ticks % TimeSpan.TicksPerDay != 0)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance must be a whole number of days"));
		}
	}
}
