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
	public static PropertyResult.String<Uri?, Uri, IThat<Uri?>> HasPath(this IThat<Uri?> source)
		=> new(source.FailIfRelative(), u => u is null ? null : Uri.UnescapeDataString(u.AbsolutePath), "path");
}
