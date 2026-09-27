using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

#pragma warning disable S110 // The result hierarchy is intentionally deep, so that each continuation inherits the complete vocabulary of its base
/// <summary>
///     The result for verifying how often an item occurs in a collection, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectCountResult{TType,TThat,TElement}" />
/// </remarks>
public class ObjectCountWithToleranceResult<TType, TThat, TElement, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier,
	ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
	: ObjectCountResult<TType, TThat, TElement>(expectationBuilder, returnValue, quantifier, options)
{
	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the comparison.
	/// </summary>
	public ObjectCountResult<TType, TThat, TElement> Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return this;
	}
}
