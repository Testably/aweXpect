using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of exactly type <typeparamref name="THandler" />.
	/// </summary>
	public static EventOfTypeResult<EventInfo?, IThat<EventInfo?>> IsOfExactType<THandler>(
		this IThat<EventInfo?> subject)
		=> IsOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of exactly type <paramref name="handlerType" />.
	/// </summary>
	public static EventOfTypeResult<EventInfo?, IThat<EventInfo?>> IsOfExactType(
		this IThat<EventInfo?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, true);
		return new EventOfTypeResult<EventInfo?, IThat<EventInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	public sealed partial class EventOfTypeResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact type <typeparamref name="THandler" />.
		/// </summary>
		public EventOfTypeResult<TValue, TResult> OrOfExactType<THandler>()
			=> OrOfExactType(typeof(THandler));

		/// <summary>
		///     Allow an alternative exact type <paramref name="handlerType" />.
		/// </summary>
		public EventOfTypeResult<TValue, TResult> OrOfExactType(Type handlerType)
		{
			typeFilterOptions.RegisterType(handlerType, true);
			return this;
		}
	}
}
