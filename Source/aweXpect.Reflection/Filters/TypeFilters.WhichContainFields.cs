using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that contain fields matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default a type passes when it contains at least one matching field. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TResult}(TResult, Times)" />) to require a specific count.<br />
	///     The inherited fields are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access.
	///     Restrict them with <see cref="TypesContainingMembersScopeResult.Directly" /> or add the members of these
	///     roots with <see cref="TypesContainingMembersScopeResult.IncludingObjectMembers" />.
	/// </remarks>
	public static TypesContainingMembersScopeResult WhichContainFields(this Filtered.Types @this,
		Func<Filtered.Fields, Filtered.Fields> filter)
		=> ContainingMembers<FieldInfo, Filtered.Fields>(@this, (types, memberScope) => types.Fields(memberScope), filter);
}
