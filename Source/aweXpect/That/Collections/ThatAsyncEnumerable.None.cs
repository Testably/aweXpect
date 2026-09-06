#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection no items…
	/// </summary>
	public static Elements<TItem> None<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new(subject,
			EnumerableQuantifier.None(subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection no items…
	/// </summary>
	public static Elements None(
		this IThat<IAsyncEnumerable<string?>?> subject)
		=> new(subject,
			EnumerableQuantifier.None(subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
