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

		if (actual.Equals(expected.Value))
		{
			return true;
		}

		if (!IsFinite(actual) || !IsFinite(expected.Value))
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

	public static bool IsConsideredEqualTo(this double? actual, double? expected, double tolerance)
	{
		if (actual is null || expected is null)
		{
			return actual is null && expected is null;
		}

		if (actual.Value.Equals(expected.Value))
		{
			return true;
		}

		if (!IsFinite(actual.Value) || !IsFinite(expected.Value))
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

		if (actual.Equals(expected.Value))
		{
			return true;
		}

		if (!IsFinite(actual) || !IsFinite(expected.Value))
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

		if (actual.Value.Equals(expected.Value))
		{
			return true;
		}

		if (!IsFinite(actual.Value) || !IsFinite(expected.Value))
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

	public static bool IsConsideredEqualTo(this DateTimeOffset actual, DateTimeOffset? expected, TimeSpan tolerance)
	{
		TimeSpan? difference = actual - expected;
		return difference <= tolerance && difference >= tolerance.Negate();
	}

	public static bool IsConsideredEqualTo(this DateTimeOffset? actual, DateTimeOffset? expected, TimeSpan tolerance)
	{
		if (actual is null && expected is null)
		{
			return true;
		}

		TimeSpan? difference = actual - expected;
		return difference <= tolerance && difference >= tolerance.Negate();
	}

	/// <remarks>
	///     Compares the shifted values instead of the difference, because the difference between two
	///     <see cref="TimeSpan" /> values can exceed the range of a <see cref="TimeSpan" />.
	/// </remarks>
	public static bool IsConsideredEqualTo(this TimeSpan actual, TimeSpan? expected, TimeSpan tolerance)
		=> expected is not null &&
		   actual.ShiftedTicks(tolerance) >= expected.Value.Ticks &&
		   expected.Value.ShiftedTicks(tolerance) >= actual.Ticks;

	/// <inheritdoc cref="IsConsideredEqualTo(TimeSpan, TimeSpan?, TimeSpan)" />
	public static bool IsConsideredEqualTo(this TimeSpan? actual, TimeSpan? expected, TimeSpan tolerance)
	{
		if (actual is null || expected is null)
		{
			return actual is null && expected is null;
		}

		return actual.Value.IsConsideredEqualTo(expected, tolerance);
	}

	/// <summary>
	///     Checks whether <paramref name="actual" /> and <paramref name="other" /> can be compared at all: a
	///     <see cref="DateTimeKind.Local" /> and a <see cref="DateTimeKind.Utc" /> value denote different instants for
	///     the same ticks, so any comparison between them would have to guess the local offset.
	/// </summary>
	public static bool IsKindCompatibleWith(this DateTime actual, DateTime other)
		=> AreKindCompatible(actual.Kind, other.Kind);

	/// <remarks>
	///     A non-finite value has no distance to any other value, so no tolerance can bridge it, while
	///     <see cref="double.Equals(double)" /> still lets <c>NaN</c> and each infinity match themselves.
	/// </remarks>
	private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

	/// <inheritdoc cref="IsFinite(double)" />
	private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

	public static bool AreKindCompatible(DateTimeKind? actualKind, DateTimeKind? expectedKind)
	{
		if (actualKind == DateTimeKind.Unspecified || expectedKind == DateTimeKind.Unspecified)
		{
			return true;
		}

		return actualKind == expectedKind;
	}
}
