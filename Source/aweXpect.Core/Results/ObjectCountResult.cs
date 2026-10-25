using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often an item occurs in a collection.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="QuantifierExtensions" /> and
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </remarks>
public class ObjectCountResult<TType, TThat, TItem>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	OptionsSlot<Quantifier> quantifier,
	OptionsSlot<ObjectMatchType<TItem>> options)
	: AndOrResult<TType, TThat, ObjectCountResult<TType, TThat, TItem>>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>,
		IObjectEqualityResult<ObjectCountResult<TType, TThat, TItem>, TItem>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<ObjectMatchType<TItem>> IOptionsProvider<ObjectMatchType<TItem>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<Quantifier> IOptionsProvider<Quantifier>.Options => quantifier;
}
