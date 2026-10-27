using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a string collection matches another collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="CollectionMatchOptionsExtensions" /> and
///     <see cref="StringEqualityOptionsExtensions" />.
/// </remarks>
public class StringCollectionMatchResult<TCollection, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<StringEqualityOptions> options,
	IOptionsHolderSource<CollectionMatchOptions> collectionMatchOptions)
	: AndOrResult<TCollection, TThat, StringCollectionMatchResult<TCollection, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<CollectionMatchOptions>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<CollectionMatchOptions> IOptionsProvider<CollectionMatchOptions>.Options => collectionMatchOptions.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => options.Holder;
}
