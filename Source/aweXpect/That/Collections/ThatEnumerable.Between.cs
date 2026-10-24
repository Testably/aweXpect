using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<IQuantifiedEnumerableItems<TItem>, Times>Between<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int minimum)
		=> new(maximum => new Items<TItem>(subject,
			EnumerableQuantifier.Between(minimum, maximum)));

	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static BetweenResult<INonGenericEnumerableItems<IEnumerable>, Times>Between(
		this IThat<IEnumerable?> subject,
		int minimum)
		=> new(maximum => new NonGenericItems<IEnumerable>(subject,
			EnumerableQuantifier.Between(minimum, maximum)));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	public static BetweenResult<IStructEnumerableItems<ImmutableArray<TItem>, TItem>, Times>Between<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int minimum)
		=> new(maximum => new StructItems<ImmutableArray<TItem>, TItem>(subject,
			EnumerableQuantifier.Between(minimum, maximum)));
#endif
}
