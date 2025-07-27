using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that satisfy the <paramref name="predicate" />.
	/// </summary>
	public static Filtered.Events WhichSatisfy(this Filtered.Events @this,
		Func<EventInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> @this.Which(Filter.Suffix(predicate, $"matching {doNotPopulateThisValue} "));
}
