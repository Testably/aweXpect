#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> AtMost<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int maximum)
		=> new(subject,
			EnumerableQuantifier.AtMost(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection at most <paramref name="maximum" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements AtMost(
		this IThat<IAsyncEnumerable<string?>?> subject,
		int maximum)
		=> new(subject,
			EnumerableQuantifier.AtMost(maximum, subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
