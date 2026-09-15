using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the minor component of the <see cref="Version" /> subject…
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<Version?> HasMinor(this IThat<Version?> subject)
		=> new(subject, a => a?.Minor, "minor");
}
