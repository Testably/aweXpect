using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that in the collection no items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedEnumerableItems<TItem> None<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.None(subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection no items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static INonGenericEnumerableItems<IEnumerable> None(
		this IThat<IEnumerable?> subject)
		=> new NonGenericItems<IEnumerable>(subject,
			EnumerableQuantifier.None(subject.Get().ExpectationBuilder.ExpectationGrammars));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection no items…
	/// </summary>
	public static IStructEnumerableItems<ImmutableArray<TItem>, TItem> None<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
		=> new StructItems<ImmutableArray<TItem>, TItem>(subject,
			EnumerableQuantifier.None(subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif
}
