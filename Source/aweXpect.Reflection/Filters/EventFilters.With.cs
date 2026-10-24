using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	private const string DirectText = "direct ";

	/// <summary>
	///     Filter for events with attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the filter to the
	///     attributes declared directly on the event with <see cref="EventsWith.Directly" />.
	/// </remarks>
	public static EventsWith With<TAttribute>(this Filtered.Events @this)
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<EventInfo>(
			(eventInfo, inherit) => eventInfo.HasAttribute<TAttribute>(inherit: inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

	/// <summary>
	///     Filter for events with attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the filter to the
	///     attributes declared directly on the event with <see cref="EventsWith.Directly" />.
	/// </remarks>
	public static EventsWith With<TAttribute>(this Filtered.Events @this,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
		=> new(@this, new AttributeFilter<EventInfo>(
			(eventInfo, inherit) => eventInfo.HasAttribute(predicate, inherit),
			directly => $"with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

	/// <summary>
	///     Additional filters on events with an attribute.
	/// </summary>
	public class EventsWith : Filtered.Events
	{
		private readonly AttributeFilter<EventInfo> _filter;
		private readonly Filtered.Events _source;

		internal EventsWith(Filtered.Events source, AttributeFilter<EventInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		public EventsWith OrWith<TAttribute>()
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(eventInfo, inherit) => eventInfo.HasAttribute<TAttribute>(inherit: inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} "));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		public EventsWith OrWith<TAttribute>(
			Func<TAttribute, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Or(
				(eventInfo, inherit) => eventInfo.HasAttribute(predicate, inherit),
				directly => $"or with {(directly ? DirectText : "")}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));

		/// <summary>
		///     Only considers the attributes declared directly on the event, not those inherited from a base type or an overridden member.
		/// </summary>
		/// <remarks>
		///     Applies to all attributes of the filter, also to those added via <see cref="OrWith{TAttribute}()" />.
		/// </remarks>
		public EventsWith Directly()
			=> new(_source, _filter.Directly());
	}
}
