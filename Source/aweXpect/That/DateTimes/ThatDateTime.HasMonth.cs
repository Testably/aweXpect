using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the month of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTime> HasMonth(this IThat<DateTime> subject)
		=> new(subject, a => a.Month, "month");

	/// <summary>
	///     Verifies that the month of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTime, IThat<DateTime>> HasMonth(
		this IThat<DateTime> subject,
		int expected)
		=> subject.HasMonth().EqualTo(expected);
}
