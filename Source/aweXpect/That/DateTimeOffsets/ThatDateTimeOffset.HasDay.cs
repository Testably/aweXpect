using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the day of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasDay(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Day, "day");

	/// <summary>
	///     Verifies that the day of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTimeOffset, IThat<DateTimeOffset>> HasDay(
		this IThat<DateTimeOffset> subject,
		int expected)
		=> subject.HasDay().EqualTo(expected);
}
