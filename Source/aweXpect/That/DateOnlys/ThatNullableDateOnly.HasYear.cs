#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateOnly
{
	/// <summary>
	///     Verifies that the year of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateOnly?> HasYear(this IThat<DateOnly?> subject)
		=> new(subject, a => a?.Year, "year");

	/// <summary>
	///     Verifies that the year of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateOnly?, IThat<DateOnly?>> HasYear(
		this IThat<DateOnly?> subject,
		int expected)
		=> subject.HasYear().EqualTo(expected);
}
#endif
