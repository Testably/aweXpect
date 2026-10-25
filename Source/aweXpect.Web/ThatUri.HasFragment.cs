using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	/// <summary>
	///     Verifies that the URL-decoded fragment of the <see cref="Uri" /> subject, without the leading <c>#</c>…
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.Fragment" />
	/// </remarks>
	[GuaranteesNotNull]
	public static PropertyResult.String<Uri?, Uri, IThat<Uri?>> HasFragment(this IThat<Uri?> subject)
		=> new(subject.FailIfRelative(), u => u is null ? null : DecodeFragment(u.Fragment), "fragment");

	/// <summary>
	///     Verifies that the URL-decoded fragment of the <see cref="Uri" /> subject, without the leading <c>#</c>, is
	///     equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     <seealso cref="Uri.Fragment" />
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<Uri, IThat<Uri?>> HasFragment(this IThat<Uri?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return subject.HasFragment().EqualTo(expected);
	}

	private static string DecodeFragment(string fragment)
		=> Uri.UnescapeDataString(fragment.Length > 0 ? fragment.Substring(1) : fragment);
}
