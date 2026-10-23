using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <typeparamref name="THandler" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static EventsOfTypeResult<IEnumerable<EventInfo?>?, IThat<IEnumerable<EventInfo?>?>>
		AreOfType<THandler>(
			this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <paramref name="handlerType" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static EventsOfTypeResult<IEnumerable<EventInfo?>?, IThat<IEnumerable<EventInfo?>?>> AreOfType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, false);
		return new EventsOfTypeResult<IEnumerable<EventInfo?>?, IThat<IEnumerable<EventInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <typeparamref name="THandler" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static EventsOfTypeResult<IAsyncEnumerable<EventInfo?>?, IThat<IAsyncEnumerable<EventInfo?>?>>
		AreOfType<THandler>(
			this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <paramref name="handlerType" /> (or a
	///     subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static EventsOfTypeResult<IAsyncEnumerable<EventInfo?>?, IThat<IAsyncEnumerable<EventInfo?>?>>
		AreOfType(
			this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, false);
		return new EventsOfTypeResult<IAsyncEnumerable<EventInfo?>?, IThat<IAsyncEnumerable<EventInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of type
	///     <typeparamref name="THandler" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfType<THandler>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		=> AreNotOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of type
	///     <paramref name="handlerType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>> AreNotOfType(
		this IThat<IEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, false);
		return new AndOrResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of type
	///     <typeparamref name="THandler" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>>
		AreNotOfType<THandler>(
			this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		=> AreNotOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that none of the events in the filtered collection have a handler of type
	///     <paramref name="handlerType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>> AreNotOfType(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, false);
		return new AndOrResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<EventInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}
#endif

	/// <summary>
	///     Result that allows chaining additional types for event collections.
	/// </summary>
	public sealed partial class EventsOfTypeResult<TValue, TResult>
		: AndOrResult<TValue, TResult>,
			IOptionsProvider<TypeFilterOptions>
		where TResult : IThat<TValue>
	{
		private readonly TypeFilterOptions _typeFilterOptions;

		internal EventsOfTypeResult(
			ExpectationBuilder expectationBuilder,
			TResult subject,
			TypeFilterOptions typeFilterOptions)
			: base(expectationBuilder, subject)
		{
			_typeFilterOptions = typeFilterOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{TypeFilterOptions}.Options" />
		TypeFilterOptions IOptionsProvider<TypeFilterOptions>.Options => _typeFilterOptions;

		/// <summary>
		///     Allow an alternative type <typeparamref name="THandler" /> (or a subtype).
		/// </summary>
		public EventsOfTypeResult<TValue, TResult> OrOfType<THandler>()
			=> OrOfType(typeof(THandler));

		/// <summary>
		///     Allow an alternative type <paramref name="handlerType" /> (or a subtype).
		/// </summary>
		public EventsOfTypeResult<TValue, TResult> OrOfType(Type handlerType)
		{
			_typeFilterOptions.RegisterType(handlerType, false);
			return this;
		}
	}

	private sealed class AreOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: CollectionConstraintResult<EventInfo?>(it, grammars),
			IContextConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				@event => typeFilterOptions.Matches(@event?.EventHandlerType), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, @event => typeFilterOptions.Matches(@event?.EventHandlerType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching events");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are not all ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching events");
	}

	private sealed class AreNotOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: CollectionConstraintResult<EventInfo?>(it, grammars),
			IContextConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				@event => @event is not null && !typeFilterOptions.Matches(@event.EventHandlerType),
				cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context,
				@event => @event is not null && !typeFilterOptions.Matches(@event.EventHandlerType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all not ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained events ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("also contain an event ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained events not ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}
	}
}
