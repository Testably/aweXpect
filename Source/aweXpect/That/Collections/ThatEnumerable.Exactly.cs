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
	///     Verifies that in the collection exactly <paramref name="expected" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> Exactly<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int expected)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.Exactly(expected));

	/// <summary>
	///     Verifies that in the collection exactly <paramref name="expected" /> items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> Exactly(
		this IThat<IEnumerable?> subject,
		int expected)
		=> new NonGenericItems<IEnumerable>(subject, EnumerableQuantifier.Exactly(expected));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection exactly <paramref name="expected" /> items…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> Exactly<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int expected)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject,
			EnumerableQuantifier.Exactly(expected));
#endif
}
