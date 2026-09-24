using System;
using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Helpers;

internal static class TimeDifferenceHelpers
{
	/// <summary>
	///     Appends the signed <paramref name="differenceTicks" /> as time span, followed by the optional
	///     <paramref name="reference" /> it was measured from.
	/// </summary>
	/// <remarks>
	///     The sign is written explicitly, because the time span formatter only writes the magnitude. The difference is
	///     omitted, when the formatter cannot show it: below one millisecond it would read as zero, and at or beyond
	///     the range of a <see cref="TimeSpan" /> it has no value to format.
	/// </remarks>
	public static StringBuilder AppendTimeDifference(this StringBuilder stringBuilder, decimal? differenceTicks,
		string? reference = null)
	{
		if (differenceTicks is null)
		{
			return stringBuilder;
		}

		decimal magnitude = Math.Abs(differenceTicks.Value);
		if (magnitude < TimeSpan.TicksPerMillisecond || magnitude >= TimeSpan.MaxValue.Ticks)
		{
			return stringBuilder;
		}

		stringBuilder.Append(differenceTicks.Value < 0 ? " which differs by -" : " which differs by ");
		Formatter.Format(stringBuilder, TimeSpan.FromTicks((long)magnitude));
		return stringBuilder.AppendReference(reference);
	}

	/// <summary>
	///     Appends the signed <paramref name="differenceDays" />, followed by the optional <paramref name="reference" />
	///     it was measured from.
	/// </summary>
	public static StringBuilder AppendDayDifference(this StringBuilder stringBuilder, int? differenceDays,
		string? reference = null)
	{
		if (differenceDays is null or 0)
		{
			return stringBuilder;
		}

		stringBuilder.Append(" which differs by ").Append(differenceDays.Value)
			.Append(differenceDays.Value is 1 or -1 ? " day" : " days");
		return stringBuilder.AppendReference(reference);
	}

	/// <summary>
	///     Appends the difference in ticks to the bound of the range that <paramref name="actualTicks" /> lies
	///     outside of, if any.
	/// </summary>
	public static StringBuilder AppendTimeDifferenceToRange(this StringBuilder stringBuilder, decimal? actualTicks,
		decimal? minimumTicks, decimal? maximumTicks)
	{
		if (actualTicks < minimumTicks)
		{
			return stringBuilder.AppendTimeDifference(actualTicks - minimumTicks, "the minimum");
		}

		if (actualTicks > maximumTicks)
		{
			return stringBuilder.AppendTimeDifference(actualTicks - maximumTicks, "the maximum");
		}

		return stringBuilder;
	}

	/// <summary>
	///     Appends the difference in days to the bound of the range that <paramref name="actualDays" /> lies outside
	///     of, if any.
	/// </summary>
	public static StringBuilder AppendDayDifferenceToRange(this StringBuilder stringBuilder, int? actualDays,
		int? minimumDays, int? maximumDays)
	{
		if (actualDays < minimumDays)
		{
			return stringBuilder.AppendDayDifference(actualDays - minimumDays, "the minimum");
		}

		if (actualDays > maximumDays)
		{
			return stringBuilder.AppendDayDifference(actualDays - maximumDays, "the maximum");
		}

		return stringBuilder;
	}

	/// <summary>
	///     Appends the difference to the closest of the <paramref name="expected" /> values, that has a compatible
	///     <see cref="DateTime.Kind" />.
	/// </summary>
	public static StringBuilder AppendTimeDifferenceToClosest(this StringBuilder stringBuilder, DateTime? actual,
		IEnumerable<DateTime?> expected)
		=> stringBuilder.AppendTimeDifferenceToClosest(expected.Select(value
			=> actual is not null && value is not null && actual.Value.IsKindCompatibleWith(value.Value)
				? (actual.Value - value.Value).Ticks
				: (decimal?)null));

	/// <summary>
	///     Appends the difference to the closest of the <paramref name="expected" /> values.
	/// </summary>
	public static StringBuilder AppendTimeDifferenceToClosest(this StringBuilder stringBuilder,
		DateTimeOffset? actual, IEnumerable<DateTimeOffset?> expected)
		=> stringBuilder.AppendTimeDifferenceToClosest(expected.Select(value => (decimal?)(actual - value)?.Ticks));

	/// <summary>
	///     Appends the difference to the closest of the <paramref name="expected" /> values.
	/// </summary>
	public static StringBuilder AppendTimeDifferenceToClosest(this StringBuilder stringBuilder, TimeSpan? actual,
		IEnumerable<TimeSpan?> expected)
		=> stringBuilder.AppendTimeDifferenceToClosest(expected.Select(value
			=> (decimal?)actual?.Ticks - value?.Ticks));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Appends the difference around the clock face to the closest of the <paramref name="expected" /> values.
	/// </summary>
	public static StringBuilder AppendTimeDifferenceToClosest(this StringBuilder stringBuilder, TimeOnly? actual,
		IEnumerable<TimeOnly?> expected)
		=> stringBuilder.AppendTimeDifferenceToClosest(expected.Select(value
			=> actual is not null && value is not null
				? actual.Value.CircularDifferenceTicks(value.Value)
				: (decimal?)null));
#endif

	private static StringBuilder AppendTimeDifferenceToClosest(this StringBuilder stringBuilder,
		IEnumerable<decimal?> differenceTicks)
		=> stringBuilder.AppendTimeDifference(GetSmallest(differenceTicks), "the closest value");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Appends the shorter difference around the clock face between <paramref name="actual" /> and
	///     <paramref name="expected" />.
	/// </summary>
	public static StringBuilder AppendCircularTimeDifference(this StringBuilder stringBuilder, TimeOnly? actual,
		TimeOnly? expected)
		=> stringBuilder.AppendTimeDifference(actual is not null && expected is not null
			? actual.Value.CircularDifferenceTicks(expected.Value)
			: null);

	/// <summary>
	///     Appends the difference in days to the closest of the <paramref name="expected" /> values.
	/// </summary>
	public static StringBuilder AppendDayDifferenceToClosest(this StringBuilder stringBuilder, DateOnly? actual,
		IEnumerable<DateOnly?> expected)
		=> stringBuilder.AppendDayDifference((int?)GetSmallest(expected.Select(value
			=> (decimal?)actual?.DayNumber - value?.DayNumber)), "the closest value");

	/// <summary>
	///     Appends the difference to the closer end of the arc that runs clockwise from <paramref name="minimum" /> to
	///     <paramref name="maximum" />, if <paramref name="actual" /> lies outside of it.
	/// </summary>
	public static StringBuilder AppendTimeDifferenceToArc(this StringBuilder stringBuilder, TimeOnly? actual,
		TimeOnly? minimum, TimeOnly? maximum)
	{
		if (actual is null || minimum is null || maximum is null ||
		    actual.Value.IsOnArc(minimum.Value, maximum.Value, TimeSpan.Zero))
		{
			return stringBuilder;
		}

		long beforeMinimum = minimum.Value.ClockwiseTicksFrom(actual.Value);
		long afterMaximum = actual.Value.ClockwiseTicksFrom(maximum.Value);
		return beforeMinimum < afterMaximum
			? stringBuilder.AppendTimeDifference(-beforeMinimum, "the minimum")
			: stringBuilder.AppendTimeDifference(afterMaximum, "the maximum");
	}
#endif

	private static decimal? GetSmallest(IEnumerable<decimal?> differences)
	{
		decimal? smallest = null;
		foreach (decimal? difference in differences)
		{
			if (difference is not null && (smallest is null || Math.Abs(difference.Value) < Math.Abs(smallest.Value)))
			{
				smallest = difference;
			}
		}

		return smallest;
	}

	private static StringBuilder AppendReference(this StringBuilder stringBuilder, string? reference)
		=> reference is null ? stringBuilder : stringBuilder.Append(" from ").Append(reference);
}
