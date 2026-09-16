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

	/// <summary>
	///     Verifies that the revision component of the <see cref="Version" /> subject is equal to the
	///     <paramref name="expected" /> value.
	///     The revision component is <c>-1</c> when it is unspecified.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Version?, IThat<Version?>> HasRevision(
		this IThat<Version?> subject,
		int expected)
		=> subject.HasRevision().EqualTo(expected);
}
