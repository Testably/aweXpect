using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the millisecond of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasMillisecond(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Millisecond, "millisecond");
}
