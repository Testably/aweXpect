using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDateTimeOffset
{
	/// <summary>
	///     Verifies that the year of the subject…
	/// </summary>
	public static PropertyResult.Int<DateTimeOffset> HasYear(this IThat<DateTimeOffset> subject)
		=> new(subject, a => a.Year, "year");
}
