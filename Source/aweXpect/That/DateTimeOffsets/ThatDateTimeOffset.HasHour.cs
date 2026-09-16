using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the hour of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasHour(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Hour, "hour");

	/// <summary>
	///     Verifies that the hour of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTimeOffset, IThat<DateTimeOffset>> HasHour(
		this IThat<DateTimeOffset> subject,
		int expected)
		=> subject.HasHour().EqualTo(expected);
}
