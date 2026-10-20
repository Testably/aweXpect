using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are obsolete (marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Events WhichAreObsolete(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => eventInfo.IsObsolete(),
			"obsolete "));

	/// <summary>
	///     Filters for events that are not obsolete (not marked with the <see cref="System.ObsoleteAttribute" />).
	/// </summary>
	public static Filtered.Events WhichAreNotObsolete(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => !eventInfo.IsObsolete(),
			"non-obsolete "));
}
