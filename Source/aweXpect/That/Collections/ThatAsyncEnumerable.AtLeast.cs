#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection at least <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedAsyncEnumerableItems<TItem> AtLeast<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int minimum)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.AtLeast(minimum));
}
#endif
