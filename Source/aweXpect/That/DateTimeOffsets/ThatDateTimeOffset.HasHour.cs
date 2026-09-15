using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the hour of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasHour(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Hour, "hour");
}
