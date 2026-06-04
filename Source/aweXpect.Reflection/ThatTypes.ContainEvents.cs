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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain events matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching event. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited events are considered.
	/// </remarks>
	public static TypeContainingMembersResult<IEnumerable<Type?>> ContainEvents(
		this IThat<IEnumerable<Type?>> subject,
		Func<Filtered.Events, Filtered.Events> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<EventInfo, Filtered.Events>(subject, types => types.Events(memberScope), filter);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> contain events matching the
	///     <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default each type must contain at least one matching event. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.<br />
	///     The <paramref name="memberScope" /> controls whether inherited events are considered.
	/// </remarks>
	public static TypeContainingMembersResult<IAsyncEnumerable<Type?>> ContainEvents(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Func<Filtered.Events, Filtered.Events> filter,
		MemberScope memberScope = MemberScope.DeclaredOnly)
		=> Contain<EventInfo, Filtered.Events>(subject, types => types.Events(memberScope), filter);
#endif
}
