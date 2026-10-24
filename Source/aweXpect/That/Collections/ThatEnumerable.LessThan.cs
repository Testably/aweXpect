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
	///     Verifies that in the collection fewer than <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> LessThan<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int maximum)
		=> new Items<TItem>(subject, EnumerableQuantifier.LessThan(maximum));

	/// <summary>
	///     Verifies that in the collection fewer than <paramref name="maximum" /> items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> LessThan(
		this IThat<IEnumerable?> subject,
		int maximum)
		=> new NonGenericItems<IEnumerable>(subject, EnumerableQuantifier.LessThan(maximum));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection fewer than <paramref name="maximum" /> items…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> LessThan<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int maximum)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject,
			EnumerableQuantifier.LessThan(maximum));
#endif
}
