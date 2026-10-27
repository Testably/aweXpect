using System;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Recording;

namespace aweXpect.Results;

/// <summary>
///     A trigger result that also allows specifying parameter filters.
/// </summary>
/// <remarks>
///     The number of times is specified via <see cref="QuantifierExtensions" />.
/// </remarks>
public class EventTriggerResult<TSubject>
	: AndOrResult<IEventRecording<TSubject>, IThat<IEventRecording<TSubject>?>, EventTriggerResult<TSubject>>,
		EventTriggerResult<TSubject>.ICustomParameterFilter,
		IOptionsProvider<Quantifier>,
		IOptionsProvider<RepeatedCheckOptions>
	where TSubject : notnull
{
	private readonly IOptionsHolderSource<TriggerEventFilter> _filter;
	private readonly IOptionsHolderSource<RepeatedCheckOptions> _options;
	private readonly IOptionsHolderSource<Quantifier> _quantifier;

	internal EventTriggerResult(
		ExpectationBuilder expectationBuilder,
		IThat<IEventRecording<TSubject>?> returnValue,
		IOptionsHolderSource<TriggerEventFilter> filter,
		IOptionsHolderSource<Quantifier> quantifier,
		IOptionsHolderSource<RepeatedCheckOptions> options)
		: base(expectationBuilder, returnValue)
	{
		_filter = filter;
		_quantifier = quantifier;
		_options = options;
	}

	/// <inheritdoc cref="ICustomParameterFilter.WithParameter{TParameter}(string, int?, Func{TParameter, bool})" />
	EventTriggerResult<TSubject> ICustomParameterFilter.WithParameter<TParameter>(
		string expression,
		int? position,
		Func<TParameter, bool> predicate)
	{
		ThrowHelper.ThrowIfPositionIsNegative(position);
		predicate.ThrowIfNull();
		_filter.Holder.Value = _filter.Holder.Value.AddPredicate(
			o => position == null
				? o.Any(x => x is TParameter p && UserCode.Invoke(predicate, p, "the predicate"))
				: MatchesAt(o, position.Value, predicate),
			expression);
		return this;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<Quantifier> IOptionsProvider<Quantifier>.Options => _quantifier.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<RepeatedCheckOptions> IOptionsProvider<RepeatedCheckOptions>.Options => _options.Holder;

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
		_filter.Holder.Value = _filter.Holder.Value.AddPredicate(
			o => o.Length > 0 && UserCode.Invoke(predicate, o[0], "the predicate"),
			$" with sender {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Adds a predicate for the <see cref="EventArgs" /> of the event.
	/// </summary>
	/// <remarks>
	///     The event args are expected to be the second parameter. Event args that are <see langword="null" /> are
	///     passed to the <paramref name="predicate" />, event args of another type do not match.
	/// </remarks>
	public EventTriggerResult<TSubject> With<TEventArgs>(
		Func<TEventArgs, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TEventArgs : EventArgs
	{
		predicate.ThrowIfNull();
		_filter.Holder.Value = _filter.Holder.Value.AddPredicate(
			o => MatchesAt(o, 1, predicate),
			$" with {Formatter.Format(typeof(TEventArgs))} {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Adds a predicate that at least one parameter of type <typeparamref name="TParameter" /> must satisfy.
	/// </summary>
	/// <remarks>
	///     Parameters of other types and parameters that are <see langword="null" /> are ignored; the event is excluded
	///     when no parameter of type <typeparamref name="TParameter" /> satisfies <paramref name="predicate" />.
	/// </remarks>
	public EventTriggerResult<TSubject> WithParameter<TParameter>(Func<TParameter, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		_filter.Holder.Value = _filter.Holder.Value.AddPredicate(
			o => o.Any(x => x is TParameter m && UserCode.Invoke(predicate, m, "the predicate")),
			$" with {Formatter.Format(typeof(TParameter))} parameter {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Adds a parameter predicate on the parameter at the given zero-based <paramref name="position" /> of type
	///     <typeparamref name="TParameter" />.
	/// </summary>
	/// <remarks>
	///     A parameter of another type does not match. A parameter that is <see langword="null" /> is passed to the
	///     <paramref name="predicate" />, unless <typeparamref name="TParameter" /> is a non-nullable value type.
	/// </remarks>
	public EventTriggerResult<TSubject> WithParameter<TParameter>(int position, Func<TParameter, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfPositionIsNegative(position);
		predicate.ThrowIfNull();
		_filter.Holder.Value = _filter.Holder.Value.AddPredicate(
			o => MatchesAt(o, position, predicate),
			$" with {Formatter.Format(typeof(TParameter))} parameter [{position}] {doNotPopulateThisValue.TrimCommonWhiteSpace()}");
		return this;
	}

	/// <summary>
	///     Allows a <paramref name="timeout" /> until the condition must be met.
	/// </summary>
	public EventTriggerResult<TSubject> Within(TimeSpan timeout)
	{
		_options.Holder.Within(timeout);
		return this;
	}

	private static bool MatchesAt<TParameter>(object?[] parameters, int position, Func<TParameter, bool> predicate)
		=> parameters.Length > position && parameters[position] switch
		{
			TParameter parameter => UserCode.Invoke(predicate, parameter, "the predicate"),
			// A null parameter has no type to test, so it matches whenever TParameter can hold null.
			null => default(TParameter) is null && UserCode.Invoke(predicate, default(TParameter)!, "the predicate"),
			_ => false,
		};

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
		///     <typeparamref name="TParameter" />, so that at least one of them must satisfy it; parameters that are
		///     <see langword="null" /> are ignored.<br />
		///     Otherwise a parameter of another type at the <paramref name="position" /> does not match, and a parameter
		///     that is <see langword="null" /> is passed to the <paramref name="predicate" />, unless
		///     <typeparamref name="TParameter" /> is a non-nullable value type.
		/// </remarks>
		EventTriggerResult<TSubject> WithParameter<TParameter>(
			string expression,
			int? position,
			Func<TParameter, bool> predicate);
	}
}
