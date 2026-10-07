using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectCollectionMatchResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityOptions<TElement> options,
	CollectionMatchOptions collectionMatchOptions)
	: AndOrResult<TType, TThat, ObjectCollectionMatchResult<TType, TThat, TElement>>(expectationBuilder,
			returnValue),
		IOptionsProvider<CollectionMatchOptions>,
		IObjectEqualityResult<ObjectCollectionMatchResult<TType, TThat, TElement>, TElement>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => collectionMatchOptions;
}
