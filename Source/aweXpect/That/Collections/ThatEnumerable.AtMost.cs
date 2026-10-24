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
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> AtMost<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int maximum)
		=> new Items<TItem>(subject, EnumerableQuantifier.AtMost(maximum));

	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> AtMost(
		this IThat<IEnumerable?> subject,
		int maximum)
		=> new NonGenericItems<IEnumerable>(subject, EnumerableQuantifier.AtMost(maximum));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> AtMost<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int maximum)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject, EnumerableQuantifier.AtMost(maximum));
#endif
}
