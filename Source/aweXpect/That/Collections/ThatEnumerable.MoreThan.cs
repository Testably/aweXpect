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
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> MoreThan<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int minimum)
		=> new(subject, EnumerableQuantifier.MoreThan(minimum, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements MoreThan(
		this IThat<IEnumerable<string?>?> subject,
		int minimum)
		=> new(subject, EnumerableQuantifier.MoreThan(minimum, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ElementsForEnumerable<IEnumerable> MoreThan(
		this IThat<IEnumerable> subject,
		int minimum)
		=> new(subject, EnumerableQuantifier.MoreThan(minimum, subject.Get().ExpectationBuilder.ExpectationGrammars));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<TItem>, TItem> MoreThan<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int minimum)
		=> new(subject, EnumerableQuantifier.MoreThan(minimum, subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<string?>> MoreThan(
		this IThat<ImmutableArray<string?>> subject,
		int minimum)
		=> new(subject, EnumerableQuantifier.MoreThan(minimum, subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif
}
