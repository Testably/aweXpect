using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the offset of the subject…
	/// </summary>
	public static PropertyResult.TimeSpan<DateTimeOffset> HasOffset(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Offset, "offset");
}
