using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the second of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTimeOffset?> HasSecond(this IThat<DateTimeOffset?> subject)
		=> new(subject, a => a?.Second, "second");

	/// <summary>
	///     Verifies that the second of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTimeOffset?, IThat<DateTimeOffset?>> HasSecond(
		this IThat<DateTimeOffset?> subject,
		int expected)
		=> subject.HasSecond().EqualTo(expected);
}
