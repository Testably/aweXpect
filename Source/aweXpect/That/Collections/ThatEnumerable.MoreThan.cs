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
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> MoreThan<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int minimum)
		=> new Items<TItem>(subject, EnumerableQuantifier.MoreThan(minimum));

	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> MoreThan(
		this IThat<IEnumerable?> subject,
		int minimum)
		=> new NonGenericItems<IEnumerable>(subject, EnumerableQuantifier.MoreThan(minimum));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> MoreThan<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int minimum)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject,
			EnumerableQuantifier.MoreThan(minimum));
#endif
}
