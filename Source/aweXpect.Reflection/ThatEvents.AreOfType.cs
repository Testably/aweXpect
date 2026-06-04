using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <typeparamref name="THandler" /> (or a
	///     subtype).
	/// </summary>
	public static EventsOfTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>>
		AreOfType<THandler>(
			this IThat<IEnumerable<EventInfo?>> subject)
		=> AreOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <paramref name="handlerType" /> (or a
	///     subtype).
	/// </summary>
	public static EventsOfTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>> AreOfType(
		this IThat<IEnumerable<EventInfo?>> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, false);
		return new EventsOfTypeResult<IEnumerable<EventInfo?>, IThat<IEnumerable<EventInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<EventInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <typeparamref name="THandler" /> (or a
	///     subtype).
	/// </summary>
	public static EventsOfTypeResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>>
		AreOfType<THandler>(
			this IThat<IAsyncEnumerable<EventInfo?>> subject)
		=> AreOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that all events in the filtered collection have a handler of type <paramref name="handlerType" /> (or a
	///     subtype).
	/// </summary>
	public static EventsOfTypeResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>>
		AreOfType(
			this IThat<IAsyncEnumerable<EventInfo?>> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, false);
		return new EventsOfTypeResult<IAsyncEnumerable<EventInfo?>, IThat<IAsyncEnumerable<EventInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<EventInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Result that allows chaining additional types for event collections.
	/// </summary>
	public sealed partial class EventsOfTypeResult<TValue, TResult>(
		ExpectationBuilder expectationBuilder,
		TResult subject,
		TypeFilterOptions typeFilterOptions)
		: AndOrResult<TValue, TResult>(expectationBuilder, subject),
			IOptionsProvider<TypeFilterOptions>
		where TResult : IThat<TValue>
	{
		/// <inheritdoc cref="IOptionsProvider{TypeFilterOptions}.Options" />
		TypeFilterOptions IOptionsProvider<TypeFilterOptions>.Options => typeFilterOptions;

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
			typeFilterOptions.RegisterType(handlerType, false);
			return this;
		}
	}

	private sealed class AreOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: CollectionConstraintResult<EventInfo?>(grammars),
			IValueConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, @event => typeFilterOptions.Matches(@event?.EventHandlerType));
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual)
			=> SetValue(actual, @event => typeFilterOptions.Matches(@event?.EventHandlerType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching events ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching events ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
