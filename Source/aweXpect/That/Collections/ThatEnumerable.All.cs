using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
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
	public static Elements<TItem> All<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new(subject, EnumerableQuantifier.All(subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements All(
		this IThat<IEnumerable<string?>?> subject)
		=> new(subject, EnumerableQuantifier.All(subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ElementsForEnumerable<IEnumerable> All(
		this IThat<IEnumerable> subject)
		=> new(subject, EnumerableQuantifier.All(subject.Get().ExpectationBuilder.ExpectationGrammars));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<TItem>, TItem> All<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
		=> new(subject, EnumerableQuantifier.All(subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<string?>> All(
		this IThat<ImmutableArray<string?>> subject)
		=> new(subject, EnumerableQuantifier.All(subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif
}
