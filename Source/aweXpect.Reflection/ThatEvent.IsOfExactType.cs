using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of exactly type <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static EventOfTypeResult<EventInfo, IThat<EventInfo?>> IsOfExactType<THandler>(
		this IThat<EventInfo?> subject)
		=> IsOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of exactly type <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static EventOfTypeResult<EventInfo, IThat<EventInfo?>> IsOfExactType(
		this IThat<EventInfo?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, true);
		return new EventOfTypeResult<EventInfo, IThat<EventInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not have a handler of exactly type
	///     <typeparamref name="THandler" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<EventInfo, IThat<EventInfo?>> IsNotOfExactType<THandler>(
		this IThat<EventInfo?> subject)
		=> IsNotOfExactType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> does not have a handler of exactly type
	///     <paramref name="handlerType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<EventInfo, IThat<EventInfo?>> IsNotOfExactType(
		this IThat<EventInfo?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, true);
		return new AndOrResult<EventInfo, IThat<EventInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
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
			_typeFilterOptions.RegisterType(handlerType, true);
			return this;
		}
	}
}
