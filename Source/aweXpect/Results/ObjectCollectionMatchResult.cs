using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectCollectionMatchResult<TCollection, TThat, TItem>
	: AndOrResult<TCollection, TThat, ObjectCollectionMatchResult<TCollection, TThat, TItem>>,
		IOptionsProvider<CollectionMatchOptions>,
		IObjectEqualityResult<ObjectCollectionMatchResult<TCollection, TThat, TItem>, TItem>
{
	private readonly OptionsHolder<CollectionMatchOptions> _collectionMatchOptions;
	private readonly OptionsHolder<ObjectMatchType<TItem>> _options;

	internal ObjectCollectionMatchResult(
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
