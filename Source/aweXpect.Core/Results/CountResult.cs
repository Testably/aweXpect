using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The result of an expectation with an underlying value of type <typeparamref name="TType" />.
///     <para />
///     In addition to the combinations from <see cref="AndOrResult{TType,TThat}" />, allows specifying
///     options on the <paramref name="quantifier" /> via <see cref="QuantifierExtensions" />.
/// </summary>
public class CountResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	TThat returnValue,
	Quantifier quantifier)
	: AndOrResult<TType, TThat, CountResult<TType, TThat>>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;
}
