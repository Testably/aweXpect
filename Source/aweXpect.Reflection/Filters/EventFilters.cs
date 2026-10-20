using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

/// <summary>
///     Extensions events to filter <see cref="Filtered.Events" />.
/// </summary>
public static partial class EventFilters
{
	/// <summary>
	///     Additional filters on events with a handler of a specific type.
	/// </summary>
	public partial class EventsOfType(Filtered.Events inner, IChangeableFilter<EventInfo> filter)
		: Filtered.Events(inner);
}
