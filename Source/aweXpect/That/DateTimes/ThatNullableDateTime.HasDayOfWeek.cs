using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the day of the week of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Enum<DateTime?, DateTime, IThat<DateTime?>, DayOfWeek> HasDayOfWeek(this IThat<DateTime?> subject)
		=> new(subject, a => a?.DayOfWeek, "day of week");

	/// <summary>
	///     Verifies that the day of the week of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTime, IThat<DateTime?>> HasDayOfWeek(
		this IThat<DateTime?> subject,
		DayOfWeek? expected)
		=> subject.HasDayOfWeek().EqualTo(expected);
}
