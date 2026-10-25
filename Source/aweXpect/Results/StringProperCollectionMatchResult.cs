using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string collection contains or is contained in another collection, optionally
///     properly.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="StringEqualityOptionsExtensions" />.
/// </remarks>
public class StringProperCollectionMatchResult<TCollection, TThat>
	: AndOrResult<TCollection, TThat, StringProperCollectionMatchResult<TCollection, TThat>>,
		IOptionsProvider<CollectionMatchOptions>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions,
		ICollectionContainmentOptions,
		IProperContainmentOptions
{
	private readonly IOptionsHolderSource<CollectionMatchOptions> _collectionMatchOptions;
	private readonly IOptionsHolderSource<StringEqualityOptions> _options;

	internal StringProperCollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		IOptionsHolderSource<StringEqualityOptions> options,
		IOptionsHolderSource<CollectionMatchOptions> collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionMatchOptions> IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options.Holder;
}
