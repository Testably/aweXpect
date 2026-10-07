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
public class ObjectCountResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	ObjectEqualityOptions<TElement> options)
	: AndOrResult<TType, TThat, ObjectCountResult<TType, TThat, TElement>>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>,
		IObjectEqualityResult<ObjectCountResult<TType, TThat, TElement>, TElement>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;
}
