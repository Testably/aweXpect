using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection, optionally
///     properly, allowing a <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
	CollectionMatchOptions collectionMatchOptions)
	: AndOrResult<TType, TThat, ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>>(
			expectationBuilder, returnValue),
		IOptionsProvider<CollectionMatchOptions>,
		ICollectionContainmentOptions,
		IProperContainmentOptions,
		IObjectEqualityWithToleranceResult<
			ObjectProperCollectionMatchWithToleranceResult<TType, TThat, TItem, TTolerance>, TItem, TTolerance>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TItem, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TItem, TTolerance>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => collectionMatchOptions;
}
