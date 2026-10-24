using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" />.
/// </remarks>
public class CollectionMatchResult<TType, TThat, TElement>
	: AndOrResult<TType, TThat, CollectionMatchResult<TType, TThat, TElement>>,
		IOptionsProvider<CollectionMatchOptions>
{
	private readonly CollectionMatchOptions _collectionMatchOptions;

	internal CollectionMatchResult(
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
