using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> contains fields matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default the assertion succeeds when the type contains at least one matching field. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited fields are considered.
	/// </remarks>
	public static TypeContainingMembersResult<Type?> ContainsFields(
		this IThat<Type?> subject,
		Func<Filtered.Fields, Filtered.Fields> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contains<FieldInfo, Filtered.Fields>(subject, types => types.Fields(memberScope), filter);
}
