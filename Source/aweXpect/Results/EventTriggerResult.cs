using System;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Recording;

namespace aweXpect.Results;

/// <summary>
///     A trigger result that also allows specifying parameter filters.
/// </summary>
public class EventTriggerResult<TSubject>(
	ExpectationBuilder expectationBuilder,
	IThat<IEventRecording<TSubject>> returnValue,
	TriggerEventFilter filter,
	Quantifier quantifier,
	RepeatedCheckOptions options)
	: CountResult<IEventRecording<TSubject>, IThat<IEventRecording<TSubject>>, EventTriggerResult<TSubject>>(
			expectationBuilder, returnValue, quantifier),
		EventTriggerResult<TSubject>.ICustomParameterFilter,
		IOptionsProvider<RepeatedCheckOptions>
	where TSubject : notnull
{
	/// <inheritdoc cref="ICustomParameterFilter.WithParameter{TParameter}(string, int?, Func{TParameter, bool})" />
	EventTriggerResult<TSubject> ICustomParameterFilter.WithParameter<TParameter>(
		string expression,
		int? position,
		Func<TParameter, bool> predicate)
	{
		predicate.ThrowIfNull();
		filter.AddPredicate(
			o => position == null
				? o.Any(x => x is TParameter p && predicate(p))
				: o.Length > position && o[position.Value] is TParameter m && predicate(m),
			expression);
		return this;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	RepeatedCheckOptions IOptionsProvider<RepeatedCheckOptions>.Options => options;

	/// <summary>
	///     Adds a predicate for the sender of the event.
	/// </summary>
	/// <remarks>
	///     The sender is expected to be the first parameter.
	/// </remarks>
	public EventTriggerResult<TSubject> WithSender(
		Func<object?, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		filter.AddPredicate(
			o => o.Length > 0 && predicate(o[0]),
			$" with sender {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Adds a predicate for the <see cref="EventArgs" /> of the event.
	/// </summary>
	/// <remarks>
	///     The event args are expected to be the second parameter.
	/// </remarks>
	public EventTriggerResult<TSubject> With<TEventArgs>(
		Func<TEventArgs, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEventArgs : EventArgs
	{
		predicate.ThrowIfNull();
		filter.AddPredicate(
			o => o.Length > 1 && o[1] is TEventArgs m && predicate(m),
			$" with {Formatter.Format(typeof(TEventArgs))} {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Adds a predicate that at least one parameter of type <typeparamref name="TParameter" /> must satisfy.
	/// </summary>
	/// <remarks>
	///     Parameters of other types are ignored; the event is excluded when no parameter of type
	///     <typeparamref name="TParameter" /> satisfies <paramref name="predicate" />.
	/// </remarks>
	public EventTriggerResult<TSubject> WithParameter<TParameter>(Func<TParameter, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		filter.AddPredicate(
			o => o.Any(x => x is TParameter m && predicate(m)),
			$" with {Formatter.Format(typeof(TParameter))} parameter {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Adds a parameter predicate on the parameter at the given zero-based <paramref name="position" /> of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	/// <remarks>
	///     The filter will exclude parameters where the type does not match.
	/// </remarks>
	public EventTriggerResult<TSubject> WithParameter<TParameter>(int position, Func<TParameter, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		filter.AddPredicate(
			o => o.Length > position && o[position] is TParameter m && predicate(m),
			$" with {Formatter.Format(typeof(TParameter))} parameter [{position}] {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> until the condition must be met.
	/// </summary>
	public EventTriggerResult<TSubject> Within(TimeSpan timeout)
	{
		options.Within(timeout);
		return this;
	}

	/// <summary>
	///     Gives access to additional methods for extensions.
	/// </summary>
	public interface ICustomParameterFilter
	{
		/// <summary>
		///     Adds a parameter predicate on the parameter at the given zero-based <paramref name="position" /> of type
		///     <typeparamref name="TParameter" /> and the <paramref name="expression" /> for extension methods.
		/// </summary>
		/// <remarks>
		///     This method is mainly intended for extension methods, as it allows overriding the default
		///     <paramref name="expression" />.<br />
		///     When <paramref name="position" /> is <see langword="null" />, the predicate applies to any parameter of type
		///     <typeparamref name="TParameter" />, so that at least one of them must satisfy it.
		/// </remarks>
		EventTriggerResult<TSubject> WithParameter<TParameter>(
			string expression,
			int? position,
			Func<TParameter, bool> predicate);
	}
}
