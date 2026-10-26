using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an expectation on the members of a type, allowing to restrict them to the declared members via
///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" /> or to add the members declared on
///     <see cref="object" />, <see cref="System.ValueType" /> and <see cref="System.Enum" /> via
///     <see cref="IncludingObjectMembers" />.
/// </summary>
/// <remarks>
///     By default the inherited members are included, but neither those declared on <see cref="object" />,
///     <see cref="System.ValueType" /> or <see cref="System.Enum" /> nor those a derived type cannot access. The two
///     options exclude each other.
/// </remarks>
public sealed class MemberScopeResult<TType, TValue> : InheritanceScopeResult<TType, TValue>
{
	private readonly OptionsHolder<MemberScopes> _scope;
	private readonly IThat<TValue> _subject;

	internal MemberScopeResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<MemberScopes> scope)
		: base(expectationBuilder, subject, scope)
	{
		_subject = subject;
		_scope = scope;
	}

	/// <summary>
	///     Also considers the inherited members declared on <see cref="object" />, <see cref="System.ValueType" /> and
	///     <see cref="System.Enum" />.
	/// </summary>
	public AndOrResult<TType, IThat<TValue>> IncludingObjectMembers()
	{
		_scope.IncludingObjectMembers();
		return new AndOrResult<TType, IThat<TValue>>(ExpectationBuilder, _subject);
	}
}
