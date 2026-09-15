using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the day of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTime> HasDay(this IThat<DateTime> subject)
		=> new(subject, a => a.Day, "day");
}
