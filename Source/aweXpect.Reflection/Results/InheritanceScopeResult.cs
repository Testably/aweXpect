using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an expectation that considers inherited attributes, operators, members or relations, allowing to
///     restrict it to declared ones or direct relations via <see cref="Directly" />.
/// </summary>
public class InheritanceScopeResult<TType, TThat>
	: AndOrResult<TType, IThat<TThat>, InheritanceScopeResult<TType, TThat>>
{
	private readonly ScopeOptions _scope;

	internal InheritanceScopeResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		ScopeOptions scope)
		: base(expectationBuilder, subject)
	{
		_scope = scope;
	}

	/// <summary>
	///     Only considers the attributes, operators or members declared directly on the subject, or a direct relation
	///     (a direct base class or a directly implemented interface).
	/// </summary>
	public InheritanceScopeResult<TType, TThat> Directly()
	{
		_scope.Directly();
		return this;
	}
}
