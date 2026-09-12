using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
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
	public static BetweenResult<Elements<TItem>> Between<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int minimum)
		=> new(maximum => new Elements<TItem>(subject,
			EnumerableQuantifier.Between(minimum, maximum, subject.Get().ExpectationBuilder.ExpectationGrammars)));

	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<Elements> Between(
		this IThat<IEnumerable<string?>?> subject,
		int minimum)
		=> new(maximum => new Elements(subject,
			EnumerableQuantifier.Between(minimum, maximum, subject.Get().ExpectationBuilder.ExpectationGrammars)));

	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static BetweenResult<ElementsForEnumerable<IEnumerable>> Between(
		this IThat<IEnumerable> subject,
		int minimum)
		=> new(maximum => new ElementsForEnumerable<IEnumerable>(subject,
			EnumerableQuantifier.Between(minimum, maximum, subject.Get().ExpectationBuilder.ExpectationGrammars)));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<ElementsForStructEnumerable<ImmutableArray<TItem>, TItem>> Between<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int minimum)
		=> new(maximum => new ElementsForStructEnumerable<ImmutableArray<TItem>, TItem>(subject,
			EnumerableQuantifier.Between(minimum, maximum, subject.Get().ExpectationBuilder.ExpectationGrammars)));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<ElementsForStructEnumerable<ImmutableArray<string?>>> Between(
		this IThat<ImmutableArray<string?>> subject,
		int minimum)
		=> new(maximum => new ElementsForStructEnumerable<ImmutableArray<string?>>(subject,
			EnumerableQuantifier.Between(minimum, maximum, subject.Get().ExpectationBuilder.ExpectationGrammars)));
#endif
}
