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
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static EventsWith With<TAttribute>(this Filtered.Events @this, bool inherit = true)
		where TAttribute : Attribute
	{
		Filter.GenericFilter<EventInfo> filter = Filter.Suffix<EventInfo>(
			eventInfo => eventInfo.HasAttribute<TAttribute>(inherit: inherit),
			() => $"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} ");
		return new EventsWith(@this, filter);
	}

	/// <summary>
	///     Filter for events with attribute of type <typeparamref name="TAttribute" /> that
	///     match the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static EventsWith With<TAttribute>(this Filtered.Events @this,
		Func<TAttribute, bool>? predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		Filter.GenericFilter<EventInfo> filter = Filter.Suffix<EventInfo>(
			eventInfo => eventInfo.HasAttribute(predicate, inherit),
			() => $"with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} ");
		return new EventsWith(@this, filter);
	}

	/// <summary>
	///     Additional filters on events with an attribute.
	/// </summary>
	public class EventsWith : Filtered.Events
	{
		private readonly Filter.GenericFilter<EventInfo> _filter;
		private readonly Filtered.Events _source;

		internal EventsWith(Filtered.Events source, Filter.GenericFilter<EventInfo> filter)
			: base(source.Which(filter))
		{
			_source = source;
			_filter = filter;
		}

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public EventsWith OrWith<TAttribute>(bool inherit = true)
			where TAttribute : Attribute
			=> new(_source, _filter.Extend((result, eventInfo) => result || eventInfo.HasAttribute<TAttribute>(inherit: inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} "));

		/// <summary>
		///     Allow an alternative attribute of type <typeparamref name="TAttribute" /> that
		///     matches the <paramref name="predicate" />.
		/// </summary>
		/// <remarks>
		///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
		///     the attribute can be inherited from a base type.
		/// </remarks>
		public EventsWith OrWith<TAttribute>(
			Func<TAttribute, bool>? predicate,
			bool inherit = true,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			where TAttribute : Attribute
			=> new(_source, _filter.Extend(
				(result, eventInfo) => result || eventInfo.HasAttribute(predicate, inherit),
				description
					=> $"{description}or with {(inherit ? "" : DirectText)}{Formatter.Format(typeof(TAttribute))} matching {doNotPopulateThisValue.TrimExpression()} "));
	}
}
