using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the second of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasSecond(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Second, "second");

	/// <summary>
	///     Verifies that the second of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<DateTimeOffset, IThat<DateTimeOffset>> HasSecond(
		this IThat<DateTimeOffset> subject,
		int expected)
		=> subject.HasSecond().EqualTo(expected);
}
