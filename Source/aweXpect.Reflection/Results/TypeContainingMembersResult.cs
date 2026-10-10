using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an assertion that a <see cref="System.Type" /> (or a collection of types) contains matching
///     members, allowing to specify a quantifier for the number of matching members via
///     <see cref="QuantifierExtensions" />.
/// </summary>
public class TypeContainingMembersResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	Quantifier quantifier)
	: AndOrResult<TType, IThat<TThat>, TypeContainingMembersResult<TType, TThat>>(expectationBuilder, subject),
		IOptionsProvider<Quantifier>
{
	/// <inheritdoc cref="IOptionsProvider{Quantifier}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;
}
