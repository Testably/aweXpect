#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection no items…
	/// </summary>
	[GuaranteesNotNull]
	public static IQuantifiedAsyncEnumerableItems<TItem> None<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new Items<TItem>(subject,
			EnumerableQuantifier.None(subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
