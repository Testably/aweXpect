#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> All<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new(subject,
			EnumerableQuantifier.All(subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection all items…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements All(
		this IThat<IAsyncEnumerable<string?>?> subject)
		=> new(subject,
			EnumerableQuantifier.All(subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
