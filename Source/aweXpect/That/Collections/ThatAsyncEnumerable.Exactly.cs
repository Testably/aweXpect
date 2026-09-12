#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection exactly <paramref name="expected" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> Exactly<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject,
		int expected)
		=> new(subject,
			EnumerableQuantifier.Exactly(expected, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection exactly <paramref name="expected" /> items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements Exactly(
		this IThat<IAsyncEnumerable<string?>?> subject,
		int expected)
		=> new(subject,
			EnumerableQuantifier.Exactly(expected, subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
