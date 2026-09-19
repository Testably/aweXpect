#if NET8_0_OR_GREATER
using System;

namespace aweXpect.Helpers;

/// <summary>
///     Treats a <see cref="TimeOnly" /> as a point on the clock face, where midnight is not a boundary.
/// </summary>
internal static class TimeOnlyHelpers
{
	/// <summary>
	///     The shortest distance in ticks between <paramref name="value" /> and <paramref name="other" /> in either
	///     direction around the clock face, which therefore never exceeds 12 hours.
	/// </summary>
	public static long CircularDistanceTicks(this TimeOnly value, TimeOnly other)
	{
		long distance = Math.Abs(value.Ticks - other.Ticks);
		return Math.Min(distance, TimeSpan.TicksPerDay - distance);
	}

	/// <summary>
	///     Whether <paramref name="value" /> lies on the arc that runs clockwise from <paramref name="minimum" /> to
	///     <paramref name="maximum" />, extended at both ends by the <paramref name="tolerance" />.
	/// </summary>
	/// <remarks>
	///     The tolerance is clamped to a full day, so that an arbitrarily large tolerance covers the whole clock face
	///     instead of overflowing.
	/// </remarks>
	public static bool IsOnArc(this TimeOnly value, TimeOnly minimum, TimeOnly maximum, TimeSpan tolerance)
	{
		long toleranceTicks = Math.Clamp(tolerance.Ticks, -TimeSpan.TicksPerDay, TimeSpan.TicksPerDay);
		long arc = Clockwise(maximum.Ticks - minimum.Ticks) + (2 * toleranceTicks);
		if (arc < 0)
		{
			return false;
		}

		return arc >= TimeSpan.TicksPerDay ||
		       Clockwise(value.Ticks - minimum.Ticks + toleranceTicks) <= arc;
	}

	private static long Clockwise(long ticks)
		=> ((ticks % TimeSpan.TicksPerDay) + TimeSpan.TicksPerDay) % TimeSpan.TicksPerDay;
}
#endif
