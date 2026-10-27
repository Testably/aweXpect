using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index.
/// </summary>
/// <remarks>
///     The index is specified via <see cref="CollectionIndexOptionsExtensions" />.
/// </remarks>
public class HasItemResult<TCollection, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<CollectionIndexOptions> collectionIndexOptions)
	: AndOrResult<TCollection, TThat, HasItemResult<TCollection, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionIndexOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions.Holder;
}
