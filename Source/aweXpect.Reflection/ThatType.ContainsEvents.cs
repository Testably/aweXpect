using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> contains events matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default the assertion succeeds when the type contains at least one matching event. Append a quantifier
	///     (e.g. <see cref="QuantifierExtensions.Exactly{TResult}(TResult, Times)" />) to require a specific count.<br />
	///     The inherited events are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those the type cannot access. Restrict them with
	///     <see cref="TypeContainingMembersScopeResult{TType, TThat}.Directly" /> or add the members of these roots with
	///     <see cref="TypeContainingMembersScopeResult{TType, TThat}.IncludingObjectMembers" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static TypeContainingMembersScopeResult<Type, Type?> ContainsEvents(
		this IThat<Type?> subject,
		Func<Filtered.Events, Filtered.Events> filter)
		=> ContainsMembers<EventInfo, Filtered.Events>(subject,
			(types, memberScope) => types.Events(memberScope), filter);
}
