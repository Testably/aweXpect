using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying
///     options on the <see cref="ObjectMatchType{TElement}" /> via <see cref="ObjectMatchTypeExtensions" />.
/// </summary>
public class ObjectEqualityResult<TType, TThat, TElement>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	IOptionsHolderSource<ObjectMatchType<TElement>> options)
	: AndOrResult<TType, TThat, ObjectEqualityResult<TType, TThat, TElement>>(expectationBuilder, returnValue),
		IObjectEqualityResult<ObjectEqualityResult<TType, TThat, TElement>, TElement>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<ObjectMatchType<TElement>> IOptionsProvider<ObjectMatchType<TElement>>.Options => options.Holder;
}
