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
public class ObjectCollectionMatchResult<TType, TThat, TElement>
	: AndOrResult<TType, TThat, ObjectCollectionMatchResult<TType, TThat, TElement>>,
		IOptionsProvider<CollectionMatchOptions>,
		IObjectEqualityResult<ObjectCollectionMatchResult<TType, TThat, TElement>, TElement>
{
	private readonly CollectionMatchOptions _collectionMatchOptions;
	private readonly ObjectEqualityOptions<TElement> _options;

	internal ObjectCollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		ObjectEqualityOptions<TElement> options,
		CollectionMatchOptions collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;
}
