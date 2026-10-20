using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that override a base class event.
	/// </summary>
	public static Filtered.Events WhichOverride(this Filtered.Events @this)
		=> @this.Which(Filter.Suffix<EventInfo>(
			eventInfo => eventInfo.IsOverride(),
			"which override a base event "));

	/// <summary>
	///     Filters for events that do not override a base class event.
	/// </summary>
	public static Filtered.Events WhichDoNotOverride(this Filtered.Events @this)
		=> @this.Which(Filter.Suffix<EventInfo>(
			eventInfo => !eventInfo.IsOverride(),
			"which do not override a base event "));
}
