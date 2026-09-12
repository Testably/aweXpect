#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements<TItem> Any<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new(subject,
			EnumerableQuantifier.AtLeast(1, subject.Get().ExpectationBuilder.ExpectationGrammars));

	/// <summary>
	///     Verifies that in the collection any (at least one) item…
	/// </summary>
	[GuaranteesNotNull]
	public static Elements Any(
		this IThat<IAsyncEnumerable<string?>?> subject)
		=> new(subject,
			EnumerableQuantifier.AtLeast(1, subject.Get().ExpectationBuilder.ExpectationGrammars));
}
#endif
