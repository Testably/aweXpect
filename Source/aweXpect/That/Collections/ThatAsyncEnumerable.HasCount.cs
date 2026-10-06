#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core;
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
			CollectionCountResult<AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>>((quantifier, isNegated)
				=> new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
					expectationBuilder.AddConstraint((Quantifier: quantifier, IsNegated: isNegated),
						static (state, it, grammars)
							=> new AsyncCollectionCountConstraint<TItem>(it, grammars, state.Quantifier)
								.InvertIf(state.IsNegated)),
					subject));
	}

	/// <summary>
	///     Verifies that the <paramref name="subject" /> has exactly <paramref name="expected" /> items.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		HasCount<TItem>(this IThat<IAsyncEnumerable<TItem>?> subject, int? expected)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint(expected,
				static (expectedCount, it, grammars)
					=> new AsyncCollectionCountConstraint<TItem>(it, grammars,
						EnumerableQuantifier.Exactly(expectedCount))),
			subject);
	}
}
#endif
