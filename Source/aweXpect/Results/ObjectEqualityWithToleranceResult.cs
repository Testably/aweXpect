using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />, allowing a
///     <typeparamref name="TTolerance" /> on the comparison.
/// </summary>
/// <remarks>
///     <seealso cref="ObjectEqualityResult{TType,TThat,TElement}" />
/// </remarks>
public class ObjectEqualityWithToleranceResult<TType, TThat, TElement, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
	: ObjectEqualityResult<TType, TThat, TElement>(expectationBuilder, returnValue, options)
{
	/// <summary>
	///     Specifies a <paramref name="tolerance" /> to apply on the comparison.
	/// </summary>
	public ObjectEqualityResult<TType, TThat, TElement> Within(TTolerance tolerance)
	{
		options.Within(tolerance);
		return this;
	}
}
