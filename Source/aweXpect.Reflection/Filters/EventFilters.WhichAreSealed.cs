using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are sealed.
	/// </summary>
	public static Filtered.Events WhichAreSealed(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => eventInfo.IsReallySealed(),
			"sealed "));

	/// <summary>
	///     Filters for events that are not sealed.
	/// </summary>
	public static Filtered.Events WhichAreNotSealed(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => !eventInfo.IsReallySealed(),
			"non-sealed "));
}
