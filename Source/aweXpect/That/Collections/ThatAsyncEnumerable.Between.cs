#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<Elements<TItem>> Between<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int minimum)
		=> new(maximum => new Elements<TItem>(subject,
			EnumerableQuantifier.Between(minimum, maximum, subject.Get().ExpectationBuilder.ExpectationGrammars)));

	/// <summary>
	///     Verifies that in the collection between <paramref name="minimum" />…
	/// </summary>
	[GuaranteesNotNull]
	public static BetweenResult<Elements> Between(
		this IThat<IAsyncEnumerable<string?>?> subject,
		int minimum)
		=> new(maximum => new Elements(subject,
			EnumerableQuantifier.Between(minimum, maximum, subject.Get().ExpectationBuilder.ExpectationGrammars)));
}
#endif
