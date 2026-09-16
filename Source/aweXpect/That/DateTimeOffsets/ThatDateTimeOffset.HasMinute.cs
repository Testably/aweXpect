using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the minute of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasMinute(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Minute, "minute");

	/// <summary>
	///     Verifies that the minute of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTimeOffset, IThat<DateTimeOffset>> HasMinute(
		this IThat<DateTimeOffset> subject,
		int expected)
		=> subject.HasMinute().EqualTo(expected);
}
