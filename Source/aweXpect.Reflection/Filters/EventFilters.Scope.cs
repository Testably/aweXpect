using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     A filterable collection of <see cref="EventInfo" /> that considers inherited attributes, allowing to restrict the
	///     filter to the attributes declared directly on the event via <see cref="Directly" />.
	/// </summary>
	public sealed class InheritanceScopeResult : Filtered.Events
	{
		private readonly AttributeFilter<EventInfo> _filter;
		private readonly Filtered.Events _source;

		internal InheritanceScopeResult(Filtered.Events source, AttributeFilter<EventInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Only considers the attributes declared directly on the event, not those inherited from a base type or an
		///     overridden member.
		/// </summary>
		public InheritanceScopeResult Directly()
			=> new(_source, _filter.Directly());
	}
}
