using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the revision component of the <see cref="Version" /> subject…
	///     The revision component is <c>-1</c> when it is unspecified.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<Version?> HasRevision(this IThat<Version?> subject)
		=> new(subject, a => a?.Revision, "revision");
}
