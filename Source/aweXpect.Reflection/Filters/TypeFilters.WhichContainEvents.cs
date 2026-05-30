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
	///     (e.g. <see cref="TypesContainingMembers.Exactly(Times)" />) to require a specific count.
	/// </remarks>
	public static TypesContainingMembers WhichContainEvents(this Filtered.Types @this,
		Func<Filtered.Events, Filtered.Events> filter)
		=> Containing<EventInfo, Filtered.Events>(@this, types => types.Events(), filter);
}
