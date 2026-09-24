using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often a <see cref="Signaler" /> was signaled, which allows specifying the timeout.
/// </summary>
public class SignalCountResult(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler> returnValue,
	Quantifier quantifier,
	SignalerOptions options)
	: CountResult<SignalerResult, IThat<Signaler>, SignalCountResult>(expectationBuilder, returnValue, quantifier),
		IOptionsProvider<SignalerOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => options;

	/// <summary>
	///     Specifies a timeout for waiting on the callback.
	/// </summary>
	public SignalCountResult Within(TimeSpan timeout)
	{
		options.Timeout = timeout;
		return this;
	}
}

/// <summary>
///     The result for verifying how often a <see cref="Signaler{TParameter}" /> was signaled, which allows specifying
///     the timeout and filtering the signals by their parameter.
/// </summary>
public class SignalCountResult<TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler<TParameter>> returnValue,
	Quantifier quantifier,
	SignalerOptions<TParameter> options)
	: SignalCountResult<TParameter, SignalCountResult<TParameter>>(expectationBuilder, returnValue, quantifier,
		options);

/// <summary>
///     The result for verifying how often a <see cref="Signaler{TParameter}" /> was signaled, which allows specifying
///     the timeout and filtering the signals by their parameter.
/// </summary>
public class SignalCountResult<TParameter, TSelf>(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler<TParameter>> returnValue,
	Quantifier quantifier,
	SignalerOptions<TParameter> options)
	: CountResult<SignalerResult<TParameter>, IThat<Signaler<TParameter>>, TSelf>(expectationBuilder, returnValue,
			quantifier),
		IOptionsProvider<SignalerOptions>
	where TSelf : SignalCountResult<TParameter, TSelf>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => options;

	/// <summary>
	///     Specifies a timeout for waiting on the callback.
	/// </summary>
	public TSelf Within(TimeSpan timeout)
	{
		options.Timeout = timeout;
		return (TSelf)this;
	}

	/// <summary>
	///     Specifies a predicate to filter for signals with a matching parameter.
	/// </summary>
	public TSelf With(
		Func<TParameter, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		options.WithPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace());
		return (TSelf)this;
	}
}
