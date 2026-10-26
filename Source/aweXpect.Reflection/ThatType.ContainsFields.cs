using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> contains fields matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default the assertion succeeds when the type contains at least one matching field. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TSelf}(TSelf, Times)" />) to require a specific count.<br />
	///     The inherited fields are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those the type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TValue}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<Type, Type?> ContainsFields(
		this IThat<Type?> subject,
		Func<Filtered.Fields, Filtered.Fields> filter)
		=> ContainsMembers<FieldInfo, Filtered.Fields>(subject,
			(types, memberScope) => types.Fields(memberScope), filter);
}
