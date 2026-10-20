using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are virtual.
	/// </summary>
	public static Filtered.Events WhichAreVirtual(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => eventInfo.IsReallyVirtual(),
			"virtual "));

	/// <summary>
	///     Filters for events that are not virtual.
	/// </summary>
	public static Filtered.Events WhichAreNotVirtual(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => !eventInfo.IsReallyVirtual(),
			"non-virtual "));
}
