using System;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatEvent
{
	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of type <typeparamref name="THandler" /> (or a subtype).
	/// </summary>
	public static EventOfTypeResult<EventInfo?, IThat<EventInfo?>> IsOfType<THandler>(
		this IThat<EventInfo?> subject)
		=> IsOfType(subject, typeof(THandler));

	/// <summary>
	///     Verifies that the <see cref="EventInfo" /> has a handler of type <paramref name="handlerType" /> (or a subtype).
	/// </summary>
	public static EventOfTypeResult<EventInfo?, IThat<EventInfo?>> IsOfType(
		this IThat<EventInfo?> subject, Type handlerType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(handlerType, false);
		return new EventOfTypeResult<EventInfo?, IThat<EventInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Result that allows chaining additional types for a single event.
	/// </summary>
	public sealed partial class EventOfTypeResult<TValue, TResult>(
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
		public EventOfTypeResult<TValue, TResult> OrOfType<THandler>()
			=> OrOfType(typeof(THandler));

		/// <summary>
		///     Allow an alternative type <paramref name="handlerType" /> (or a subtype).
		/// </summary>
		public EventOfTypeResult<TValue, TResult> OrOfType(Type handlerType)
		{
			typeFilterOptions.RegisterType(handlerType, false);
			return this;
		}
	}

	private sealed class IsOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
		: ConstraintResult.WithNotNullValue<EventInfo?>(it, grammars),
			IValueConstraint<EventInfo?>
	{
		public ConstraintResult IsMetBy(EventInfo? actual)
		{
			Actual = actual;
			Outcome = typeFilterOptions.Matches(actual?.EventHandlerType)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.HasFlag(ExpectationGrammars.Negated) ? "is not " : "is ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was of type ").Append(Formatter.Format(Actual?.EventHandlerType));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalExpectation(stringBuilder, indentation);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it did");
	}
}
