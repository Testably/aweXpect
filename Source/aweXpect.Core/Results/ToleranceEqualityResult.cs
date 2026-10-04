using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />
///     that allows specifying a <typeparamref name="TTolerance" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying
///     options on the <see cref="ObjectEqualityWithToleranceOptions{TSubject,TTolerance}" /> via
///     <see cref="ObjectEqualityOptionsExtensions" />.
/// </summary>
public class ToleranceEqualityResult<TType, TThat, TElement, TTolerance>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	ObjectEqualityWithToleranceOptions<TElement, TTolerance> options)
	: AndOrResult<TType, TThat, ToleranceEqualityResult<TType, TThat, TElement, TTolerance>>(expectationBuilder,
			returnValue),
		IObjectEqualityWithToleranceResult<ToleranceEqualityResult<TType, TThat, TElement, TTolerance>, TElement,
			TTolerance>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityOptions<TElement> IOptionsProvider<ObjectEqualityOptions<TElement>>.Options => options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	ObjectEqualityWithToleranceOptions<TElement, TTolerance>
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>.Options => options;
}
