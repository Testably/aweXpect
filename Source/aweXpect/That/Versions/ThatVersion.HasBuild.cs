using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatVersion
{
	/// <summary>
	///     Verifies that the build component of the <see cref="Version" /> subject…
	///     The build component is <c>-1</c> when it is unspecified.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyResult.Int<Version?> HasBuild(this IThat<Version?> subject)
		=> new(subject, a => a?.Build, "build");
}
