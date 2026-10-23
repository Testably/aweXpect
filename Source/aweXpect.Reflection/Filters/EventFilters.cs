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
	public partial class EventsOfType : Filtered.Events
	{
		private readonly Filter.GenericFilter<EventInfo> _filter;
		private readonly Filtered.Events _source;

		internal EventsOfType(Filtered.Events source, Filter.GenericFilter<EventInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}
	}
}
