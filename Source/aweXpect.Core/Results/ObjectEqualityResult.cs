using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying
///     options on the <see cref="ObjectMatchType{TSubject}" /> via <see cref="ObjectEqualityOptionsExtensions" />.
/// </summary>
public class ObjectEqualityResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	OptionsSlot<ObjectMatchType<TElement>> options)
	: AndOrResult<TType, TThat, ObjectEqualityResult<TType, TThat, TElement>>(expectationBuilder, returnValue),
		IObjectEqualityResult<ObjectEqualityResult<TType, TThat, TElement>, TElement>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<ObjectMatchType<TElement>> IOptionsProvider<ObjectMatchType<TElement>>.Options => options;
}
