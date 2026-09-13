using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the hour of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTime?> HasHour(this IThat<DateTime?> source)
		=> new(source, a => a?.Hour, "hour");
}
