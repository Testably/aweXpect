using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTime
{
	/// <summary>
	///     Verifies that the hour of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTime> HasHour(this IThat<DateTime> subject)
		=> new(subject, a => a.Hour, "hour");
}
