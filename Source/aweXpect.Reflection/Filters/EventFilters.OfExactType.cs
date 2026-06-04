using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filter for events with a handler of exact type <typeparamref name="THandler" />.
	/// </summary>
	public static EventsOfType OfExactType<THandler>(this Filtered.Events @this)
		=> OfExactType(@this, typeof(THandler));

	/// <summary>
	///     Filter for events with a handler of exact type <paramref name="handlerType" />.
	/// </summary>
	public static EventsOfType OfExactType(this Filtered.Events @this,
		Type handlerType)
	{
		IChangeableFilter<EventInfo> filter = Filter.Suffix<EventInfo>(
			eventInfo => eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType, true),
			$"of exact type {Formatter.Format(handlerType)} ");
		return new EventsOfType(@this.Which(filter), filter);
	}

	public partial class EventsOfType
	{
		/// <summary>
		///     Allow an alternative type <typeparamref name="THandler" /> exactly.
		/// </summary>
		public EventsOfType OrOfExactType<THandler>()
			=> OrOfExactType(typeof(THandler));

		/// <summary>
		///     Allow an alternative type <paramref name="handlerType" /> exactly.
		/// </summary>
		public EventsOfType OrOfExactType(Type handlerType)
		{
			filter.UpdateFilter(
				(result, eventInfo)
					=> result || eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType, true),
				description
					=> $"{description}or of exact type {Formatter.Format(handlerType)} ");
			return this;
		}
	}
}
