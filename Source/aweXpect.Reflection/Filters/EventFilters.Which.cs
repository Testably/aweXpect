using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Events Which(this Filtered.Events @this,
		Func<EventInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix<EventInfo>(eventInfo => UserCode.Invoke(predicate, eventInfo, "the predicate"),
			$"matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
