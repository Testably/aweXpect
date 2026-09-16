#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the <paramref name="subject" /> has an item count of…
	/// </summary>
	[GuaranteesNotNull]
	public static CollectionCountResult<AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>>
		HasCount<TItem>(this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new
			CollectionCountResult<AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>>(
				(quantifier, isNegated)
					=> new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
						expectationBuilder.AddConstraint((it, grammars)
							=> new AsyncCollectionCountConstraint<TItem>(expectationBuilder, it, grammars, quantifier)
								.InvertIf(isNegated)),
						subject));
	}

	/// <summary>
	///     Verifies that the <paramref name="subject" /> has exactly <paramref name="expected" /> items.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		HasCount<TItem>(this IThat<IAsyncEnumerable<TItem>?> subject, int expected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AsyncCollectionCountConstraint<TItem>(expectationBuilder, it, grammars,
					EnumerableQuantifier.Exactly(expected))),
			subject);
	}
}
#endif
