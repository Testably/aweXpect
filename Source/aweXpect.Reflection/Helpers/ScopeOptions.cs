using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The inheritance scope of a single expectation, which its result changes after the constraint was added and
///     which the constraint reads when it is evaluated.
/// </summary>
internal sealed class ScopeOptions
{
	/// <summary>
	///     The scope of the members.
	/// </summary>
	public MemberScope MemberScope { get; private set; }

	/// <summary>
	///     Whether only declared elements or direct relations are considered.
	/// </summary>
	public bool IsDirect => MemberScope.IsDirect();

	/// <summary>
	///     Restricts the scope to declared elements or direct relations.
	/// </summary>
	public void Directly() => MemberScope |= MemberScope.DeclaredOnly;

	/// <summary>
	///     Adds the members declared on <see cref="object" />, <see cref="System.ValueType" /> and
	///     <see cref="System.Enum" />.
	/// </summary>
	public void IncludingObjectMembers() => MemberScope |= MemberScope.IncludingObjectMembers;
}
