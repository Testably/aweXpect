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
	public static PropertyResult.DateTimeKind<DateTime?> HasKind(this IThat<DateTime?> subject)
		=> new(subject, a => a?.Kind, "kind");

	/// <summary>
	///     Verifies that the kind of the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<DateTime?, IThat<DateTime?>> HasKind(
		this IThat<DateTime?> subject,
		DateTimeKind expected)
		=> subject.HasKind().EqualTo(expected);
}
