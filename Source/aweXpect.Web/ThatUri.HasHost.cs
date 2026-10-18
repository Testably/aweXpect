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
}
