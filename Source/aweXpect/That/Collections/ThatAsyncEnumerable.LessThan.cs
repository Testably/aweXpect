#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection less than <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> LessThan<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int maximum)
		=> new(subject,
			EnumerableQuantifier.LessThan(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection less than <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements LessThan(
		this IThat<IAsyncEnumerable<string?>?> subject,
		int maximum)
		=> new(subject,
			EnumerableQuantifier.LessThan(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
