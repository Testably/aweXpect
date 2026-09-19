using System;

namespace aweXpect.Helpers;

internal static class EqualityHelpers
{
	public static bool IsConsideredEqualTo(this double actual, double? expected, double tolerance)
	{
		if (expected is null)
		{
			return false;
		}

		if (double.IsNaN(actual) || double.IsNaN(expected.Value))
		{
			return double.IsNaN(actual) && double.IsNaN(expected.Value);
		}

		checked
		{
			return actual > expected.Value
				? actual - expected.Value <= tolerance
				: expected.Value - actual <= tolerance;
		}
	}

	public static bool IsConsideredEqualTo(this double? actual, double? expected, double tolerance)
	{
		if (actual is null || expected is null)
		{
			return actual is null && expected is null;
		}

		if (double.IsNaN(actual.Value) || double.IsNaN(expected.Value))
		{
			return double.IsNaN(actual.Value) && double.IsNaN(expected.Value);
		}

		checked
		{
			return actual > expected.Value
				? actual - expected.Value <= tolerance
				: expected.Value - actual <= tolerance;
		}
	}

	public static bool IsConsideredEqualTo(this decimal actual, decimal? expected, decimal tolerance)
	{
		if (expected is null)
		{
			return false;
		}

		checked
		{
			return actual > expected.Value
				? actual - expected.Value <= tolerance
				: expected.Value - actual <= tolerance;
		}
	}

	public static bool IsConsideredEqualTo(this decimal? actual, decimal? expected, decimal tolerance)
	{
		if (actual is null && expected is null)
		{
			return true;
		}

		if (actual is null || expected is null)
		{
			return false;
		}

		checked
		{
			return actual > expected.Value
				? actual - expected.Value <= tolerance
				: expected.Value - actual <= tolerance;
		}
	}

	public static bool IsConsideredEqualTo(this float actual, float? expected, float tolerance)
	{
		if (expected is null)
		{
			return false;
		}

		if (float.IsNaN(actual) || float.IsNaN(expected.Value))
		{
			return float.IsNaN(actual) && float.IsNaN(expected.Value);
		}

		checked
		{
			return actual > expected.Value
				? actual - expected.Value <= tolerance
				: expected.Value - actual <= tolerance;
		}
	}

	public static bool IsConsideredEqualTo(this float? actual, float? expected, float tolerance)
	{
		if (actual is null && expected is null)
		{
			return true;
		}

		if (actual is null || expected is null)
		{
			return false;
		}

		if (float.IsNaN(actual.Value) || float.IsNaN(expected.Value))
		{
			return float.IsNaN(actual.Value) && float.IsNaN(expected.Value);
		}

		checked
		{
			return actual > expected.Value
				? actual - expected.Value <= tolerance
				: expected.Value - actual <= tolerance;
		}
	}

	public static bool IsConsideredEqualTo(this DateTime actual, DateTime? expected, TimeSpan tolerance)
		=> IsConsideredEqualTo(actual, expected, tolerance, out _);

	public static bool IsConsideredEqualTo(this DateTime actual, DateTime? expected, TimeSpan tolerance,
		out bool hasKindDifference)
	{
		TimeSpan? difference = actual - expected;
		hasKindDifference = !AreKindCompatible(actual.Kind, expected?.Kind);
		return !hasKindDifference && difference <= tolerance && difference >= tolerance.Negate();
	}

	public static bool IsConsideredEqualTo(this DateTime? actual, DateTime? expected, TimeSpan tolerance)
		=> IsConsideredEqualTo(actual, expected, tolerance, out _);

	public static bool IsConsideredEqualTo(this DateTime? actual, DateTime? expected, TimeSpan tolerance,
		out bool hasKindDifference)
	{
		if (actual is null && expected is null)
		{
			hasKindDifference = false;
			return true;
		}

		TimeSpan? difference = actual - expected;
		hasKindDifference = !AreKindCompatible(actual?.Kind, expected?.Kind);
		return !hasKindDifference && difference <= tolerance && difference >= tolerance.Negate();
	}

	/// <summary>
	///     Checks whether <paramref name="actual" /> and <paramref name="other" /> can be compared at all: a
	///     <see cref="DateTimeKind.Local" /> and a <see cref="DateTimeKind.Utc" /> value denote different instants for
	///     the same ticks, so any comparison between them would have to guess the local offset.
	/// </summary>
	public static bool IsKindCompatibleWith(this DateTime actual, DateTime other)
		=> AreKindCompatible(actual.Kind, other.Kind);

	private static bool AreKindCompatible(DateTimeKind? actualKind, DateTimeKind? expectedKind)
	{
		if (actualKind == DateTimeKind.Unspecified || expectedKind == DateTimeKind.Unspecified)
		{
			return true;
		}

		return actualKind == expectedKind;
	}
}
