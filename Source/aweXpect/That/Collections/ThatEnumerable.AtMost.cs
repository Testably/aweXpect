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
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> AtMost<TItem>(
		this IThat<IEnumerable<TItem>?> subject,
		int maximum)
		=> new(subject, EnumerableQuantifier.AtMost(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements AtMost(
		this IThat<IEnumerable<string?>?> subject,
		int maximum)
		=> new(subject, EnumerableQuantifier.AtMost(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[OverloadResolutionPriority(-1)]
	[GuaranteesNotNull]
	public static ElementsForEnumerable<IEnumerable> AtMost(
		this IThat<IEnumerable> subject,
		int maximum)
		=> new(subject, EnumerableQuantifier.AtMost(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<TItem>, TItem> AtMost<TItem>(
		this IThat<ImmutableArray<TItem>> subject,
		int maximum)
		=> new(subject, EnumerableQuantifier.AtMost(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static ElementsForStructEnumerable<ImmutableArray<string?>> AtMost(
		this IThat<ImmutableArray<string?>> subject,
		int maximum)
		=> new(subject, EnumerableQuantifier.AtMost(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));
#endif
}
