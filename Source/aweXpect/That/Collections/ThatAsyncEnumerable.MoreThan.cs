#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedAsyncEnumerableItems<TItem> MoreThan<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int minimum)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.MoreThan(minimum));
}
#endif
