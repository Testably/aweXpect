using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the millisecond of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTimeOffset?> HasMillisecond(this IThat<DateTimeOffset?> subject)
		=> new(subject, a => a?.Millisecond, "millisecond");

	/// <summary>
	///     Verifies that the millisecond of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTimeOffset?, IThat<DateTimeOffset?>> HasMillisecond(
		this IThat<DateTimeOffset?> subject,
		int expected)
		=> subject.HasMillisecond().EqualTo(expected);
}
