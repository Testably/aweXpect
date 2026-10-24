using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types whose fields, properties and events are all nullable.
	/// </summary>
	/// <remarks>
	///     The inherited members are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="InheritanceScopeResult.Directly" /> or add the members of these roots with
	///     <see cref="MemberScopeResult.IncludingObjectMembers" />.
	/// </remarks>
	public static MemberScopeResult WhichOnlyHaveNullableMembers(this Filtered.Types @this)
		=> new(@this, memberScope => Filter.Suffix<Type>(
			type => type.GetNotNullableMembers(memberScope).Length == 0,
			DescribeOnlyHaving(memberScope, "nullable")));

	/// <summary>
	///     Filters for types whose fields, properties and events are all non-nullable.
	/// </summary>
	/// <remarks>
	///     The inherited members are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access. Restrict them with
	///     <see cref="InheritanceScopeResult.Directly" /> or add the members of these roots with
	///     <see cref="MemberScopeResult.IncludingObjectMembers" />.
	/// </remarks>
	public static MemberScopeResult WhichOnlyHaveNonNullableMembers(this Filtered.Types @this)
		=> new(@this, memberScope => Filter.Suffix<Type>(
			type => type.GetNullableMembers(memberScope).Length == 0,
			DescribeOnlyHaving(memberScope, "non-nullable")));

	private static string DescribeOnlyHaving(MemberScopes memberScope, string nullability)
		=> memberScope.IsDirect()
			? $"which only declare {nullability} members "
			: $"which only have {nullability} members {memberScope.ObjectMembersText()}";
}
