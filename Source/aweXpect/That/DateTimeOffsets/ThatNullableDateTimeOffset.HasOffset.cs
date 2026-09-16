using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the offset of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.TimeSpan<DateTimeOffset?> HasOffset(this IThat<DateTimeOffset?> subject)
		=> new(subject, a => a?.Offset, "offset");

	/// <summary>
	///     Verifies that the offset of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTimeOffset?, IThat<DateTimeOffset?>> HasOffset(
		this IThat<DateTimeOffset?> subject,
		TimeSpan expected)
		=> subject.HasOffset().EqualTo(expected);
}
