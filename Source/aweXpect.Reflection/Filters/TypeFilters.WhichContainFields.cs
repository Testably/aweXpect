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
	///     (e.g. <see cref="TypesContainingMembers.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited fields are considered.
	/// </remarks>
	public static TypesContainingMembers WhichContainFields(this Filtered.Types @this,
		Func<Filtered.Fields, Filtered.Fields> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Containing<FieldInfo, Filtered.Fields>(@this, types => types.Fields(memberScope), filter);
}
