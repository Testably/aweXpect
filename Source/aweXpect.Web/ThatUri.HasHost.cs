using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the host of the <see cref="Uri" /> subject…
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.Host" />
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.String<Uri?, Uri, IThat<Uri?>> HasHost(this IThat<Uri?> subject)
		=> new(subject.FailIfRelative(), u => u?.Host, "host");

	/// <summary>
	///     Verifies that the host of the <see cref="Uri" /> subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The host is compared as <see cref="Uri.Host" /> returns it, which is lower case for registered names.
	///     <seealso cref="Uri.Host" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Uri, IThat<Uri?>> HasHost(this IThat<Uri?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return subject.HasHost().EqualTo(expected);
	}
}
