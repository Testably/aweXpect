using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are abstract.
	/// </summary>
	public static Filtered.Events WhichAreAbstract(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => eventInfo.IsReallyAbstract(),
			"abstract "));

	/// <summary>
	///     Filters for events that are not abstract.
	/// </summary>
	public static Filtered.Events WhichAreNotAbstract(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => !eventInfo.IsReallyAbstract(),
			"non-abstract "));
}
