using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection, optionally
///     properly.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectProperCollectionMatchResult<TType, TThat, TItem>
	: AndOrResult<TType, TThat, ObjectProperCollectionMatchResult<TType, TThat, TItem>>,
		IOptionsProvider<CollectionMatchOptions>,
		ICollectionContainmentOptions,
		IProperContainmentOptions,
		IObjectEqualityResult<ObjectProperCollectionMatchResult<TType, TThat, TItem>, TItem>
{
	private readonly CollectionMatchOptions _collectionMatchOptions;
	private readonly ObjectEqualityOptions<TItem> _options;

	internal ObjectProperCollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		ObjectEqualityOptions<TItem> options,
		CollectionMatchOptions collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;
}
