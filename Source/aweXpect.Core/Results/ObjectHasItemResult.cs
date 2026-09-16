using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying that a collection has a specific item at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="HasItemResult{TCollection}" />
/// </remarks>
public class ObjectHasItemResult<TCollection, TItem>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection> collection,
	CollectionIndexOptions collectionIndexOptions,
	ObjectEqualityOptions<TItem> options)
	: ObjectHasItemResult<TCollection, TItem,
		ObjectHasItemResult<TCollection, TItem>>(
		expectationBuilder,
		collection,
		collectionIndexOptions,
		options);

/// <summary>
///     The result for verifying that a collection has a specific item at a given index.
/// </summary>
/// <remarks>
///     <seealso cref="HasItemResult{TCollection}" />
/// </remarks>
public class ObjectHasItemResult<TCollection, TItem, TSelf>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection> collection,
	CollectionIndexOptions collectionIndexOptions,
	ObjectEqualityOptions<TItem> options)
	: HasItemResult<TCollection>(expectationBuilder, collection, collectionIndexOptions),
		IOptionsProvider<ObjectEqualityOptions<TItem>>
	where TSelf : ObjectHasItemResult<TCollection, TItem, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => options;

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing <see langword="object" />s.
	/// </summary>
	public TSelf Using(IEqualityComparer<object> comparer)
	{
		options.Using(comparer);
		return (TSelf)this;
	}
}
