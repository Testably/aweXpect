using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the month of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasMonth(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Month, "month");
}
