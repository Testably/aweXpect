using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the year of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTime> HasYear(this IThat<DateTime> subject)
		=> new(subject, a => a.Year, "year");

	/// <summary>
	///     Verifies that the year of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTime, IThat<DateTime>> HasYear(
		this IThat<DateTime> subject,
		int expected)
		=> subject.HasYear().EqualTo(expected);
}
