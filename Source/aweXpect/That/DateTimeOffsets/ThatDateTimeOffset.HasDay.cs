using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the day of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasDay(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Day, "day");
}
