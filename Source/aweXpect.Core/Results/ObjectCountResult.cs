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
	Quantifier quantifier,
	ObjectEqualityOptions<TItem> options)
	: AndOrResult<TType, TThat, ObjectCountResult<TType, TThat, TItem>>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>,
		IObjectEqualityResult<ObjectCountResult<TType, TThat, TItem>, TItem>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TItem> IOptionsProvider<ObjectEqualityOptions<TItem>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;
}
