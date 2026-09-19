using System;

namespace aweXpect.Helpers;

internal static class TimeSpanHelpers
{
	/// <summary>
	///     The ticks of the <paramref name="value" /> shifted by the ticks of the <paramref name="tolerance" />.
	/// </summary>
	/// <remarks>
	///     Calculated as <see cref="decimal" />, because the shifted value can exceed the range of a
	///     <see cref="TimeSpan" /> when the <paramref name="value" /> is close to its limits.
	/// </remarks>
	public static decimal ShiftedTicks(this TimeSpan value, TimeSpan tolerance)
		=> (decimal)value.Ticks + tolerance.Ticks;

	/// <inheritdoc cref="ShiftedTicks(System.TimeSpan,System.TimeSpan)" />
	public static decimal? ShiftedTicks(this TimeSpan? value, TimeSpan tolerance)
		=> value?.ShiftedTicks(tolerance);
}
