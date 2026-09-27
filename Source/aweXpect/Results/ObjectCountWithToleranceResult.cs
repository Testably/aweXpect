using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

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
