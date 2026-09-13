#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableTimeOnly
{
	/// <summary>
	///     Verifies that the minute of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<TimeOnly?> HasMinute(this IThat<TimeOnly?> source)
		=> new(source, a => a?.Minute, "minute");
}
#endif
