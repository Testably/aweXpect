#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateOnly
{
	/// <summary>
	///     Verifies that the day of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateOnly?> HasDay(this IThat<DateOnly?> subject)
		=> new(subject, a => a?.Day, "day");
}
#endif
