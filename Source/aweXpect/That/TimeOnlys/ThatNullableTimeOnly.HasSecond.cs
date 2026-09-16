#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableTimeOnly
{
	/// <summary>
	///     Verifies that the second of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<TimeOnly?> HasSecond(this IThat<TimeOnly?> subject)
		=> new(subject, a => a?.Second, "second");

	/// <summary>
	///     Verifies that the second of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TimeOnly?, IThat<TimeOnly?>> HasSecond(
		this IThat<TimeOnly?> subject,
		int expected)
		=> subject.HasSecond().EqualTo(expected);
}
#endif
