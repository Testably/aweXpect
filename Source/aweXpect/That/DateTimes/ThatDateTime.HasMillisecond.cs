using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the millisecond of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTime> HasMillisecond(this IThat<DateTime> subject)
		=> new(subject, a => a.Millisecond, "millisecond");

	/// <summary>
	///     Verifies that the millisecond of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTime, IThat<DateTime>> HasMillisecond(
		this IThat<DateTime> subject,
		int expected)
		=> subject.HasMillisecond().EqualTo(expected);
}
