using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often an item occurs in a collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="QuantifierExtensions" /> and
///     <see cref="ObjectMatchTypeExtensions" />.
/// </remarks>
public class ObjectCountResult<TCollection, TThat, TItem>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<Quantifier> quantifier,
	IOptionsHolderSource<ObjectMatchType<TItem>> options)
	: AndOrResult<TCollection, TThat, ObjectCountResult<TCollection, TThat, TItem>>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>,
		IObjectEqualityResult<ObjectCountResult<TCollection, TThat, TItem>, TItem>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ObjectMatchType<TItem>> IOptionsProvider<ObjectMatchType<TItem>>.Options => options.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<Quantifier> IOptionsProvider<Quantifier>.Options => quantifier.Holder;
}
