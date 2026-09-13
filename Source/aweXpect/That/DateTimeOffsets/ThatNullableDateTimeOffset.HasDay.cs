using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTimeOffset
{
	/// <summary>
	///     Verifies that the day of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<DateTimeOffset?> HasDay(this IThat<DateTimeOffset?> source)
		=> new(source, a => a?.Day, "day");
}
