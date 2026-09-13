using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the major component of the <see cref="Version" /> subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<Version?> HasMajor(this IThat<Version?> source)
		=> new(source, a => a?.Major, "major");
}
