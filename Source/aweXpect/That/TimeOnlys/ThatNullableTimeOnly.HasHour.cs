#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableTimeOnly
{
	/// <summary>
	///     Verifies that the hour of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<TimeOnly?> HasHour(this IThat<TimeOnly?> subject)
		=> new(subject, a => a?.Hour, "hour");
}
#endif
