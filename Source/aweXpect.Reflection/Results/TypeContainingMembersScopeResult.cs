using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of an assertion that a <see cref="System.Type" /> (or a collection of types) contains matching
///     members, allowing to specify a quantifier for the number of matching members via
///     <see cref="QuantifierExtensions" />, to restrict them to the declared members via <see cref="Directly" /> or to
///     add the members declared on <see cref="object" />, <see cref="System.ValueType" /> and <see cref="System.Enum" />
///     via <see cref="IncludingObjectMembers" />.
/// </summary>
/// <remarks>
///     By default the inherited members are included, but neither those declared on <see cref="object" />,
///     <see cref="System.ValueType" /> or <see cref="System.Enum" /> nor those a derived type cannot access. The two
///     options exclude each other.
/// </remarks>
public sealed class TypeContainingMembersScopeResult<TType, TValue> : TypeContainingMembersResult<TType, TValue>
{
	private readonly ScopeOptions _scope;

	internal TypeContainingMembersScopeResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsSlot<Quantifier> quantifier,
		ScopeOptions scope)
		: base(expectationBuilder, subject, quantifier)
	{
		_scope = scope;
	}

	/// <summary>
	///     Only considers the members declared directly on the type.
	/// </summary>
	public TypeContainingMembersResult<TType, TValue> Directly()
	{
		_scope.Directly();
		return this;
	}

	/// <summary>
	///     Also considers the inherited members declared on <see cref="object" />, <see cref="System.ValueType" /> and
	///     <see cref="System.Enum" />.
	/// </summary>
	public TypeContainingMembersResult<TType, TValue> IncludingObjectMembers()
	{
		_scope.IncludingObjectMembers();
		return this;
	}
}
