using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableDateTime
{
	/// <summary>
	///     Verifies that the kind of the subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.DateTimeKind<DateTime?> HasKind(this IThat<DateTime?> source)
		=> new(source, a => a?.Kind, "kind");
}
