using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the day of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTime> HasDay(this IThat<DateTime> subject)
		=> new(subject, a => a.Day, "day");

	/// <summary>
	///     Verifies that the day of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTime, IThat<DateTime>> HasDay(
		this IThat<DateTime> subject,
		int expected)
		=> subject.HasDay().EqualTo(expected);
}
