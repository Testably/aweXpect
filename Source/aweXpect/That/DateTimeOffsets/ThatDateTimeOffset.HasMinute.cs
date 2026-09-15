using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the minute of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasMinute(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Minute, "minute");
}
