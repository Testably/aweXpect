using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a collection matches another collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" />.
/// </remarks>
public class CollectionMatchResult<TType, TThat, TItem>
	: AndOrResult<TType, TThat, CollectionMatchResult<TType, TThat, TItem>>,
		IOptionsProvider<CollectionMatchOptions>
{
	private readonly OptionsHolder<CollectionMatchOptions> _collectionMatchOptions;

	internal CollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		OptionsHolder<CollectionMatchOptions> collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionMatchOptions> IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;
}
