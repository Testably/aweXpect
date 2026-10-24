#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<IQuantifiedAsyncEnumerableItems<TItem>, Times>Between<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int minimum)
		=> new(maximum => new Items<TItem>(subject,
			EnumerableQuantifier.Between(minimum, maximum)));
}
#endif
