using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain fields matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching field. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited fields are considered.
	/// </remarks>
	public static TypeContainingMembersResult<IEnumerable<Type?>> ContainFields(
		this IThat<IEnumerable<Type?>> subject,
		Func<Filtered.Fields, Filtered.Fields> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<FieldInfo, Filtered.Fields>(subject, types => types.Fields(memberScope), filter);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain fields matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching field. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited fields are considered.
	/// </remarks>
	public static TypeContainingMembersResult<IAsyncEnumerable<Type?>> ContainFields(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Func<Filtered.Fields, Filtered.Fields> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<FieldInfo, Filtered.Fields>(subject, types => types.Fields(memberScope), filter);
#endif
}
