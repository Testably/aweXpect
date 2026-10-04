using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection has a matching item, optionally at a given index, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionIndexOptionsExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>(
	ExpectationBuilder expectationBuilder,
	IThat<TCollection?> collection,
	CollectionIndexOptions collectionIndexOptions,
	ObjectEqualityWithToleranceOptions<TItem, TTolerance> options)
	: AndOrResult<TCollection, IThat<TCollection?>, ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>>(
			expectationBuilder, collection),
		IOptionsProvider<CollectionIndexOptions>,
		IObjectEqualityWithToleranceResult<ObjectHasItemWithToleranceResult<TCollection, TItem, TTolerance>, TItem,
			TTolerance>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TItem, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>.Options => options;
}
