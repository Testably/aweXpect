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
	public static PropertyResult.Int<Version?> HasMajor(this IThat<Version?> subject)
		=> new(subject, a => a?.Major, "major");

	/// <summary>
	///     Verifies that the major component of the <see cref="Version" /> subject is equal to the
	///     <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Version?, IThat<Version?>> HasMajor(
		this IThat<Version?> subject,
		int expected)
		=> subject.HasMajor().EqualTo(expected);
}
