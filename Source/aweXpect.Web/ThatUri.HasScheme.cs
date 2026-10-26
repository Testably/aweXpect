using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the scheme of the <see cref="Uri" /> subject…
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.Scheme" />
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.String<Uri, IThat<Uri?>, Uri?> HasScheme(this IThat<Uri?> subject)
		=> new(subject.FailIfRelative(), u => u?.Scheme, "scheme");

	/// <summary>
	///     Verifies that the scheme of the <see cref="Uri" /> subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The scheme is compared as <see cref="Uri.Scheme" /> returns it, which is always lower case.
	///     <seealso cref="Uri.Scheme" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Uri, IThat<Uri?>> HasScheme(this IThat<Uri?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return subject.HasScheme().EqualTo(expected);
	}
}
