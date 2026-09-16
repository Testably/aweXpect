using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the hour of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTime?> HasHour(this IThat<DateTime?> subject)
		=> new(subject, a => a?.Hour, "hour");

	/// <summary>
	///     Verifies that the hour of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTime?, IThat<DateTime?>> HasHour(
		this IThat<DateTime?> subject,
		int expected)
		=> subject.HasHour().EqualTo(expected);
}
