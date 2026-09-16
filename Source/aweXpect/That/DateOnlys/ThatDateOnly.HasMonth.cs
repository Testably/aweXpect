#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateOnly
{
	/// <summary>
	///     Verifies that the month of the subject…
	/// </summary>
	public static PropertyResult.Int<DateOnly> HasMonth(this IThat<DateOnly> subject)
		=> new(subject, a => a.Month, "month");

	/// <summary>
	///     Verifies that the month of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateOnly, IThat<DateOnly>> HasMonth(
		this IThat<DateOnly> subject,
		int expected)
		=> subject.HasMonth().EqualTo(expected);
}
#endif
