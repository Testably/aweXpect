#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedAsyncEnumerableItems<TItem> AtMost<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int maximum)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.AtMost(maximum));
}
#endif
