using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index.
/// </summary>
/// <remarks>
///     The index is specified via <see cref="CollectionIndexOptionsExtensions" />.
/// </remarks>
public class HasItemResult<TCollection>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions)
	: AndOrResult<TCollection, IThat<TCollection?>, HasItemResult<TCollection>>(expectationBuilder, collection),
		IOptionsProvider<CollectionIndexOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;
}
