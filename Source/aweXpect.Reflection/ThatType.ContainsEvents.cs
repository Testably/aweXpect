using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> contains events matching the <paramref name="filter" />.
	/// </summary>
	/// <remarks>
	///     By default the assertion succeeds when the type contains at least one matching event. Append a quantifier
	///     (e.g. <see cref="TypeContainingMembersResult{TThat}.Exactly(Times)" />) to require a specific count.
	/// </remarks>
	public static TypeContainingMembersResult<Type?> ContainsEvents(
		this IThat<Type?> subject,
		Func<Filtered.Events, Filtered.Events> filter)
		=> Contains<EventInfo, Filtered.Events>(subject, types => types.Events(), filter);
}
