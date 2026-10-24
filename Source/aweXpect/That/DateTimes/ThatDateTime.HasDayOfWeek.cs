using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the day of the week of the subject…
	/// </summary>
	public static PropertyResult.Enum<DateTime, DayOfWeek> HasDayOfWeek(this IThat<DateTime> subject)
		=> new(subject, a => a.DayOfWeek, "day of week");

	/// <summary>
	///     Verifies that the day of the week of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTime, IThat<DateTime>> HasDayOfWeek(
		this IThat<DateTime> subject,
		DayOfWeek? expected)
		=> subject.HasDayOfWeek().EqualTo(expected);
}
