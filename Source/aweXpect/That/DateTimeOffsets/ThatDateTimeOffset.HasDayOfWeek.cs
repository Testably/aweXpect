using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the day of the week of the subject…
	/// </summary>
	public static PropertyResult.Enum<DateTimeOffset, DayOfWeek> HasDayOfWeek(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.DayOfWeek, "day of week");

	/// <summary>
	///     Verifies that the day of the week of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTimeOffset, IThat<DateTimeOffset>> HasDayOfWeek(
		this IThat<DateTimeOffset> subject,
		DayOfWeek? expected)
		=> subject.HasDayOfWeek().EqualTo(expected);
}
