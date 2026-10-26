using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection contains or is contained in another collection, optionally
///     properly.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectProperCollectionMatchResult<TCollection, TThat, TItem>
	: AndOrResult<TCollection, TThat, ObjectProperCollectionMatchResult<TCollection, TThat, TItem>>,
		IOptionsProvider<CollectionMatchOptions>,
		ICollectionContainmentOptions,
		IProperContainmentOptions,
		IObjectEqualityResult<ObjectProperCollectionMatchResult<TCollection, TThat, TItem>, TItem>
{
	private readonly OptionsHolder<CollectionMatchOptions> _collectionMatchOptions;
	private readonly OptionsHolder<ObjectMatchType<TItem>> _options;

	internal ObjectProperCollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		OptionsHolder<ObjectMatchType<TItem>> options,
		OptionsHolder<CollectionMatchOptions> collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ObjectMatchType<TItem>> IOptionsProvider<ObjectMatchType<TItem>>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionMatchOptions> IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;
}
