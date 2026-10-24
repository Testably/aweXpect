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
	///     Verifies that in the collection at least <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> AtLeast<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int minimum)
		=> new Items<TItem>(subject, EnumerableQuantifier.AtLeast(minimum));

	/// <summary>
	///     Verifies that in the collection at least <paramref name="minimum" /> items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> AtLeast(
		this IThat<IEnumerable?> subject,
		int minimum)
		=> new NonGenericItems<IEnumerable>(subject, EnumerableQuantifier.AtLeast(minimum));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection at least <paramref name="minimum" /> items…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> AtLeast<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int minimum)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject,
			EnumerableQuantifier.AtLeast(minimum));
#endif
}
