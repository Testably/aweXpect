using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> All<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new Items<TItem>(subject, EnumerableQuantifier.All());

	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> All(
		this IThat<IEnumerable?> subject)
		=> new NonGenericItems<IEnumerable>(subject, EnumerableQuantifier.All());

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> All<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject, EnumerableQuantifier.All());
#endif
}
