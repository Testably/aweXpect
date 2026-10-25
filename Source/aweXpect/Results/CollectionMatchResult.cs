using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" />.
/// </remarks>
public class CollectionMatchResult<TCollection, TThat, TItem>
	: AndOrResult<TCollection, TThat, CollectionMatchResult<TCollection, TThat, TItem>>,
		IOptionsProvider<CollectionMatchOptions>
{
	private readonly IOptionsHolderSource<CollectionMatchOptions> _collectionMatchOptions;

	internal CollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		IOptionsHolderSource<CollectionMatchOptions> collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionMatchOptions> IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions.Holder;
}
