#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection is empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsEmpty<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new AsyncIsEmptyConstraint<TItem>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the collection is not empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		IsNotEmpty<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new AsyncIsEmptyConstraint<TItem>(it, grammars).Invert()),
			subject);
}
#endif
