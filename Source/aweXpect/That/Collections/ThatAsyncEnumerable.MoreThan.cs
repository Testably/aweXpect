#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> MoreThan<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int minimum)
		=> new(subject,
			EnumerableQuantifier.MoreThan(minimum, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection more than <paramref name="minimum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements MoreThan(
		this IThat<IAsyncEnumerable<string?>?> subject,
		int minimum)
		=> new(subject,
			EnumerableQuantifier.MoreThan(minimum, subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
