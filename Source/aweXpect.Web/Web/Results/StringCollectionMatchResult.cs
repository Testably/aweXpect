using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Web.Results;

/// <summary>
///     The result for verifying that string values match the expected values.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="StringEqualityOptionsExtensions" />.
/// </remarks>
public class StringCollectionMatchResult<TType, TThat>
	: AndOrResult<TType, TThat, StringCollectionMatchResult<TType, TThat>>,
		IOptionsProvider<CollectionMatchOptions>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	private readonly CollectionMatchOptions _collectionMatchOptions;
	private readonly StringEqualityOptions _options;

	internal StringCollectionMatchResult(
		ExpectationBuilder expectationBuilder,
		TThat returnValue,
		StringEqualityOptions options,
		CollectionMatchOptions collectionMatchOptions)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_collectionMatchOptions = collectionMatchOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	CollectionMatchOptions IOptionsProvider<CollectionMatchOptions>.Options => _collectionMatchOptions;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
}
