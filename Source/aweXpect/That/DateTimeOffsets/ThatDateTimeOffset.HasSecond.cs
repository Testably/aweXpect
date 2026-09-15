using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the second of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasSecond(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Second, "second");
}
