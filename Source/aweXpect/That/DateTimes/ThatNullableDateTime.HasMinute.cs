using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the minute of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTime?> HasMinute(this IThat<DateTime?> subject)
		=> new(subject, a => a?.Minute, "minute");

	/// <summary>
	///     Verifies that the minute of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTime?, IThat<DateTime?>> HasMinute(
		this IThat<DateTime?> subject,
		int expected)
		=> subject.HasMinute().EqualTo(expected);
}
