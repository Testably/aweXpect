using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the second of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTime> HasSecond(this IThat<DateTime> subject)
		=> new(subject, a => a.Second, "second");

	/// <summary>
	///     Verifies that the second of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTime, IThat<DateTime>> HasSecond(
		this IThat<DateTime> subject,
		int expected)
		=> subject.HasSecond().EqualTo(expected);
}
