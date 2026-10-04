#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[GuaranteesNotNull]
	public static AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem> HasSingle<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		PredicateOptions<TItem> options = new();
		AsyncHasSingleConstraint<TItem> constraint = null!;
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				constraint = new AsyncHasSingleConstraint<TItem>(it, grammars, options)),
			options,
			_ => Task.FromResult(constraint.SingleItem));
	}
}
#endif
