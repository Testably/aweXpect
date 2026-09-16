#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTimeOnly
{
	/// <summary>
	///     Verifies that the millisecond of the subject…
	/// </summary>
	public static PropertyResult.Int<TimeOnly> HasMillisecond(this IThat<TimeOnly> subject)
		=> new(subject, a => a.Millisecond, "millisecond");

	/// <summary>
	///     Verifies that the millisecond of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TimeOnly, IThat<TimeOnly>> HasMillisecond(
		this IThat<TimeOnly> subject,
		int expected)
		=> subject.HasMillisecond().EqualTo(expected);
}
#endif
