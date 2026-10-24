#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedAsyncEnumerableItems<TItem> All<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.All());
}
#endif
