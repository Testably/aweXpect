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
public class StringProperCollectionMatchResult<TType, TThat>
	: AndOrResult<TType, TThat, StringProperCollectionMatchResult<TType, TThat>>,
		IOptionsProvider<CollectionMatchOptions>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions,
		ICollectionContainmentOptions,
		IProperContainmentOptions
{
	private readonly OptionsSlot<CollectionMatchOptions> _collectionMatchOptions;
	private readonly OptionsSlot<StringEqualityOptions> _options;

	internal StringProperCollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		OptionsSlot<StringEqualityOptions> options,
		OptionsSlot<CollectionMatchOptions> collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<CollectionMatchOptions> IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;
}
