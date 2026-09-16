using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the day of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTimeOffset?> HasDay(this IThat<DateTimeOffset?> subject)
		=> new(subject, a => a?.Day, "day");

	/// <summary>
	///     Verifies that the day of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTimeOffset?, IThat<DateTimeOffset?>> HasDay(
		this IThat<DateTimeOffset?> subject,
		int expected)
		=> subject.HasDay().EqualTo(expected);
}
