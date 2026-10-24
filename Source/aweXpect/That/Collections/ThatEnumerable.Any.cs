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
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> Any<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new Items<TItem>(subject, EnumerableQuantifier.AtLeast(1));

	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> Any(
		this IThat<IEnumerable?> subject)
		=> new NonGenericItems<IEnumerable>(subject, EnumerableQuantifier.AtLeast(1));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> Any<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject, EnumerableQuantifier.AtLeast(1));
#endif
}
