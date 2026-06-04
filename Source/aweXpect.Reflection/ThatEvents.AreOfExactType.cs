using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <typeparamref name="THandler" />.
	/// </summary>
	public static EventsOfTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>>
		AreOfExactType<THandler>(
			this IThat<IEnumerable<EventInfo?>> subject)
		=> AreOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <paramref name="handlerType" />.
	/// </summary>
	public static EventsOfTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreOfExactType(
		this IThat<IEnumerable<EventInfo?>> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, true);
		return new EventsOfTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <typeparamref name="THandler" />.
	/// </summary>
	public static EventsOfTypeResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>>
		AreOfExactType<THandler>(
			this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> AreOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of exactly type
	///     <paramref name="handlerType" />.
	/// </summary>
	public static EventsOfTypeResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>>
		AreOfExactType(
			this IThat<IAsyncEnumerable<EventInfo?>> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, true);
		return new EventsOfTypeResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	public sealed partial class EventsOfTypeResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact type <typeparamref name="THandler" />.
		/// </summary>
		public EventsOfTypeResult<TValue, TResult> OrOfExactType<THandler>()
			=> OrOfExactType(typeof(THandler));

		/// <summary>
		///     Allow an alternative exact type <paramref name="handlerType" />.
		/// </summary>
		public EventsOfTypeResult<TValue, TResult> OrOfExactType(Type handlerType)
		{
			typeFilterOptions.RegisterType(handlerType, true);
			return this;
		}
	}
}
