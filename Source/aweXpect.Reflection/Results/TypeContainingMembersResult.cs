using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an assertion that a <see cref="System.Type" /> (or a collection of types) contains matching
///     members, allowing to specify a quantifier for the number of matching members via
///     <see cref="QuantifierExtensions" />.
/// </summary>
public class TypeContainingMembersResult<TType, TValue>
	: AndOrResult<TType, IThat<TValue>, TypeContainingMembersResult<TType, TValue>>,
		IOptionsProvider<Quantifier>
{
	private readonly Quantifier _quantifier;

	internal TypeContainingMembersResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		Quantifier quantifier)
		: base(expectationBuilder, subject)
	{
		_quantifier = quantifier;
	}

	/// <inheritdoc cref="IOptionsProvider{Quantifier}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => _quantifier;
}
