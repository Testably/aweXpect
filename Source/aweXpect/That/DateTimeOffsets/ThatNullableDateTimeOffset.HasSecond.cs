using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the second of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTimeOffset?> HasSecond(this IThat<DateTimeOffset?> source)
		=> new(source, a => a?.Second, "second");
}
