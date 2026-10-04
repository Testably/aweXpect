using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" />.
/// </remarks>
public class CollectionContainmentResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	CollectionMatchOptions collectionMatchOptions)
	: AndOrResult<TType, TThat, CollectionContainmentResult<TType, TThat, TElement>>(expectationBuilder,
			returnValue),
		IOptionsProvider<CollectionMatchOptions>,
		ICollectionContainmentOptions
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => collectionMatchOptions;
}
