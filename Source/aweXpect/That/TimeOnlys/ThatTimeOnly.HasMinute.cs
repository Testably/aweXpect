#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTimeOnly
{
	/// <summary>
	///     Verifies that the minute of the subject…
	/// </summary>
	public static PropertyResult.Int<TimeOnly> HasMinute(this IThat<TimeOnly> subject)
		=> new(subject, a => a.Minute, "minute");
}
#endif
