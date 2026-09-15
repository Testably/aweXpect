#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateOnly
{
	/// <summary>
	///     Verifies that the month of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateOnly?> HasMonth(this IThat<DateOnly?> subject)
		=> new(subject, a => a?.Month, "month");
}
#endif
