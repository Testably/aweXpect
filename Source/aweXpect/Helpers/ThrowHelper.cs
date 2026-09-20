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
	///     Rejects an empty set of expected values before the subject is looked at, so that the guard cannot depend on
	///     runtime data.
	/// </summary>
	public static void ThrowIfEmpty<T>(IEnumerable<T> values)
	{
		if (!values.Any())
		{
			throw Tracing.WriteException(EmptyCollection());
		}
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
