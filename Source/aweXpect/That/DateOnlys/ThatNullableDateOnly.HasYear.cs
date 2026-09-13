#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateOnly
{
	/// <summary>
	///     Verifies that the year of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateOnly?> HasYear(this IThat<DateOnly?> source)
		=> new(source, a => a?.Year, "year");
}
#endif
