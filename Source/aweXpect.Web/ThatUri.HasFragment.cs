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

	private static string DecodeFragment(string fragment)
		=> Uri.UnescapeDataString(fragment.Length > 0 ? fragment.Substring(1) : fragment);
}
