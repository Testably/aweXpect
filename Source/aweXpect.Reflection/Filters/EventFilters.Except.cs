using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Excludes events that satisfy the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     This allows defining exemptions, e.g. all events matching a rule except specific ones.
	/// </remarks>
	public static Filtered.Events Except(this Filtered.Events @this,
		Func<EventInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<EventInfo>(
			eventInfo => !predicate(eventInfo),
			$"except {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
