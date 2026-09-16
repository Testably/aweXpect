using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the year of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTimeOffset?> HasYear(this IThat<DateTimeOffset?> subject)
		=> new(subject, a => a?.Year, "year");

	/// <summary>
	///     Verifies that the year of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTimeOffset?, IThat<DateTimeOffset?>> HasYear(
		this IThat<DateTimeOffset?> subject,
		int expected)
		=> subject.HasYear().EqualTo(expected);
}
