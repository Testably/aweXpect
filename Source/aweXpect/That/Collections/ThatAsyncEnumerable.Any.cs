#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedAsyncEnumerableItems<TItem> Any<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.AtLeast(1));
}
#endif
