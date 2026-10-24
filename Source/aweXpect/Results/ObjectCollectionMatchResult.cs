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
public class ObjectCollectionMatchResult<TType, TThat, TItem>
	: AndOrResult<TType, TThat, ObjectCollectionMatchResult<TType, TThat, TItem>>,
		IOptionsProvider<CollectionMatchOptions>,
		IObjectEqualityResult<ObjectCollectionMatchResult<TType, TThat, TItem>, TItem>
{
	private readonly CollectionMatchOptions _collectionMatchOptions;
	private readonly ObjectEqualityOptions<TItem> _options;

	internal ObjectCollectionMatchResult(
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
