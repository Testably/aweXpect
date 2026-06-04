using System;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filter for events with a handler of type <typeparamref name="THandler" />.
	/// </summary>
	public static EventsOfType OfType<THandler>(this Filtered.Events @this)
		=> OfType(@this, typeof(THandler));

	/// <summary>
	///     Filter for events with a handler of type <paramref name="handlerType" />.
	/// </summary>
	public static EventsOfType OfType(this Filtered.Events @this,
		Type handlerType)
	{
		IChangeableFilter<EventInfo> filter = Filter.Suffix<EventInfo>(
			eventInfo => eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType),
			$"of type {Formatter.Format(handlerType)} ");
		return new EventsOfType(@this.Which(filter), filter);
	}

	public partial class EventsOfType
	{
		/// <summary>
		///     Allow an alternative type <typeparamref name="THandler" />.
		/// </summary>
		public EventsOfType OrOfType<THandler>()
			=> OrOfType(typeof(THandler));

		/// <summary>
		///     Allow an alternative type <paramref name="handlerType" />.
		/// </summary>
		public EventsOfType OrOfType(Type handlerType)
		{
			filter.UpdateFilter(
				(result, eventInfo)
					=> result || eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType),
				description
					=> $"{description}or of type {Formatter.Format(handlerType)} ");
			return this;
		}
	}
}
