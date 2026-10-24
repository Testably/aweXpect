using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filters for types that contain events matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default a type passes when it contains at least one matching event. Append a quantifier
	///     (e.g. <see cref="TypesContainingMembers.Exactly(Times)" />) to require a specific count.<br />
	///     The inherited events are considered, but neither those declared on <see cref="object" />,
	///     <see cref="ValueType" /> or <see cref="Enum" /> nor those a type cannot access.
	///     Restrict them with <see cref="TypesContainingMembers.MemberScopeResult.Directly" /> or add the members of these
	///     roots with <see cref="TypesContainingMembers.MemberScopeResult.IncludingObjectMembers" />.
	/// </remarks>
	public static TypesContainingMembers.MemberScopeResult WhichContainEvents(this Filtered.Types @this,
		Func<Filtered.Events, Filtered.Events> filter)
		=> ContainingMembers<EventInfo, Filtered.Events>(@this, (types, memberScope) => types.Events(memberScope), filter);
}
