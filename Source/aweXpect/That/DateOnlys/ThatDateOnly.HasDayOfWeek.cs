#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateOnly
{
	/// <summary>
	///     Verifies that the day of the week of the subject…
	/// </summary>
	public static PropertyResult.Enum<DateOnly, DayOfWeek> HasDayOfWeek(this IThat<DateOnly> subject)
		=> new(subject, a => a.DayOfWeek, "day of week");

	/// <summary>
	///     Verifies that the day of the week of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateOnly, IThat<DateOnly>> HasDayOfWeek(
		this IThat<DateOnly> subject,
		DayOfWeek? expected)
		=> subject.HasDayOfWeek().EqualTo(expected);
}
#endif
