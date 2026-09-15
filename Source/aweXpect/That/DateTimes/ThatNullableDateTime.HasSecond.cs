using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the second of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTime?> HasSecond(this IThat<DateTime?> subject)
		=> new(subject, a => a?.Second, "second");
}
