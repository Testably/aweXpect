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
	public static EventsOfType WhichAreOfExactType<THandler>(this Filtered.Events @this)
		=> WhichAreOfExactType(@this, typeof(THandler));

	/// <summary>
	///     Filter for events with a handler of exact type <paramref name="handlerType" />.
	/// </summary>
	public static EventsOfType WhichAreOfExactType(this Filtered.Events @this,
		Type handlerType)
	{
		Filter.GenericFilter<EventInfo> filter = Filter.Suffix<EventInfo>(
			eventInfo => eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType, true),
			() => $"of exact type {Formatter.Format(handlerType)} ");
		return new EventsOfType(@this, filter);
	}

	/// <summary>
	///     Filter for events without a handler of exact type <typeparamref name="THandler" />.
	/// </summary>
	public static Filtered.Events WhichAreNotOfExactType<THandler>(this Filtered.Events @this)
		=> WhichAreNotOfExactType(@this, typeof(THandler));

	/// <summary>
	///     Filter for events without a handler of exact type <paramref name="handlerType" />.
	/// </summary>
	public static Filtered.Events WhichAreNotOfExactType(this Filtered.Events @this,
		Type handlerType)
		=> @this.Which(Filter.Suffix<EventInfo>(
			eventInfo => !eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType, true),
			() => $"not of exact type {Formatter.Format(handlerType)} "));

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
			=> new(_source, _filter.Extend(
				(result, eventInfo)
					=> result || eventInfo.EventHandlerType!.IsOrInheritsFrom(handlerType, true),
				description
					=> $"{description}or of exact type {Formatter.Format(handlerType)} "));
	}
}
