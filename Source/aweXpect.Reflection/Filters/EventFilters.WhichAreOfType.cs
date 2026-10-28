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
	public static EventsOfType WhichAreOfType<THandler>(this Filtered.Events @this)
		=> WhichAreOfType(@this, typeof(THandler));

	/// <summary>
	///     Filter for events with a handler of type <paramref name="handlerType" />.
	/// </summary>
	public static EventsOfType WhichAreOfType(this Filtered.Events @this,
		Type handlerType)
	{
		Filter.GenericFilter<EventInfo> filter = Filter.Suffix<EventInfo>(
			eventInfo => eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType),
			() => $"of type {Formatter.Format(handlerType)} ");
		return new EventsOfType(@this, filter);
	}

	/// <summary>
	///     Filter for events without a handler of type <typeparamref name="THandler" />.
	/// </summary>
	public static Filtered.Events WhichAreNotOfType<THandler>(this Filtered.Events @this)
		=> WhichAreNotOfType(@this, typeof(THandler));

	/// <summary>
	///     Filter for events without a handler of type <paramref name="handlerType" />.
	/// </summary>
	public static Filtered.Events WhichAreNotOfType(this Filtered.Events @this,
		Type handlerType)
		=> @this.Which(Filter.Suffix<EventInfo>(
			eventInfo => !eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType),
			() => $"not of type {Formatter.Format(handlerType)} "));

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
			=> new(_source, _filter.Extend(
				(result, eventInfo)
					=> result || eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType),
				description
					=> $"{description}or of type {Formatter.Format(handlerType)} "));
	}
}
