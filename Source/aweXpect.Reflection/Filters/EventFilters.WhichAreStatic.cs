using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are static.
	/// </summary>
	public static Filtered.Events WhichAreStatic(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => eventInfo.IsReallyStatic(),
			"static "));

	/// <summary>
	///     Filters for events that are not static.
	/// </summary>
	public static Filtered.Events WhichAreNotStatic(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => !eventInfo.IsReallyStatic(),
			"non-static "));
}
