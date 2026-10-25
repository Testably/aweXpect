using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionIndexOptionsExtensions" /> and
///     <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectHasItemResult<TCollection, TThat, TItem>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<CollectionIndexOptions> collectionIndexOptions,
	IOptionsHolderSource<ObjectMatchType<TItem>> options)
	: AndOrResult<TCollection, TThat, ObjectHasItemResult<TCollection, TThat, TItem>>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionIndexOptions>,
		IObjectEqualityResult<ObjectHasItemResult<TCollection, TThat, TItem>, TItem>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ObjectMatchType<TItem>> IOptionsProvider<ObjectMatchType<TItem>>.Options => options.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions.Holder;
}
