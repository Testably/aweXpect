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
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> Any<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new(subject, EnumerableQuantifier.AtLeast(1, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements Any(
		this IThat<IEnumerable<string?>?> subject)
		=> new(subject, EnumerableQuantifier.AtLeast(1, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ElementsForEnumerable<IEnumerable> Any(
		this IThat<IEnumerable> subject)
		=> new(subject, EnumerableQuantifier.AtLeast(1, subject.Get().ExpectationBuilder.ExpectationGrammars));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<TItem>, TItem> Any<TItem>(
		this IThat<ImmutableArray<TItem>> subject)
		=> new(subject, EnumerableQuantifier.AtLeast(1, subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<string?>> Any(
		this IThat<ImmutableArray<string?>> subject)
		=> new(subject, EnumerableQuantifier.AtLeast(1, subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif
}
