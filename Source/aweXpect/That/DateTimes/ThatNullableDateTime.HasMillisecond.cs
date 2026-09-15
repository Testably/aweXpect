using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the millisecond of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTime?> HasMillisecond(this IThat<DateTime?> subject)
		=> new(subject, a => a?.Millisecond, "millisecond");
}
