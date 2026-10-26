using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the URL-decoded path of the <see cref="Uri" /> subject…
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.AbsolutePath" />
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.String<Uri, IThat<Uri?>, Uri?> HasPath(this IThat<Uri?> subject)
		=> new(subject.FailIfRelative(), u => u is null ? null : Uri.UnescapeDataString(u.AbsolutePath), "path");

	/// <summary>
	///     Verifies that the URL-decoded path of the <see cref="Uri" /> subject is equal to the
	///     <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.AbsolutePath" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Uri, IThat<Uri?>> HasPath(this IThat<Uri?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return subject.HasPath().EqualTo(expected);
	}
}
