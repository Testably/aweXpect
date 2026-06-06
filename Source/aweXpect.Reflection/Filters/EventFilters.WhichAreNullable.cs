using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filters for events that are nullable.
	/// </summary>
	public static Filtered.Events WhichAreNullable(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => eventInfo.IsNullable(),
			"nullable "));

	/// <summary>
	///     Filters for events that are not nullable.
	/// </summary>
	public static Filtered.Events WhichAreNotNullable(this Filtered.Events @this)
		=> @this.Which(Filter.Prefix<EventInfo>(
			eventInfo => !eventInfo.IsNullable(),
			"non-nullable "));
}
