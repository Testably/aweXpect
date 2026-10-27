using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TCollection" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TCollection,TThat}" />, allows specifying
///     the <see cref="IComparer{TElement}" /> used to verify the order of items.
/// </summary>
public class CollectionOrderResult<TCollection, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<CollectionOrderOptions<TElement>> options)
	: AndOrResult<TCollection, TThat>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionOrderOptions<TElement>>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionOrderOptions<TElement>> IOptionsProvider<CollectionOrderOptions<TElement>>.Options => options.Holder;

	/// <summary>
	///     Use the given <paramref name="comparer" /> to verify the order of items.
	/// </summary>
	public AndOrResult<TCollection, TThat> Using(IComparer<TElement> comparer)
	{
		options.Holder.SetComparer(comparer);
		return this;
	}
}
