#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection exactly <paramref name="expected" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedAsyncEnumerableItems<TItem> Exactly<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int expected)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.Exactly(expected));
}
#endif
