using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection, optionally
///     properly.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" />.
/// </remarks>
public class ProperCollectionMatchResult<TType, TThat, TItem>
	: AndOrResult<TType, TThat, ProperCollectionMatchResult<TType, TThat, TItem>>,
		IOptionsProvider<CollectionMatchOptions>,
		ICollectionContainmentOptions,
		IProperContainmentOptions
{
	private readonly CollectionMatchOptions _collectionMatchOptions;

	internal ProperCollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		CollectionMatchOptions collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;
}
