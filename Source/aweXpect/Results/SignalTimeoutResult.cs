using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for the absence of a signal, which allows specifying the timeout.
/// </summary>
/// <remarks>
///     Absence is not an occurrence, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountResult" /> to count how often a callback was signaled.
/// </remarks>
public class SignalTimeoutResult(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler> returnValue,
	SignalerOptions options)
	: AndOrResult<SignalerResult, IThat<Signaler>>(expectationBuilder, returnValue),
		IOptionsProvider<SignalerOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => options;

	/// <summary>
	///     Specifies a timeout for waiting on the callback.
	/// </summary>
	public SignalTimeoutResult Within(TimeSpan timeout)
	{
		options.Timeout = timeout;
		return this;
	}
}

/// <summary>
///     The result for the absence of a signal with <typeparamref name="TParameter" />, which allows specifying the
///     timeout.
/// </summary>
/// <remarks>
///     Absence is not an occurrence, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountWhoseResult{TParameter}" /> to count how often a callback was signaled.
/// </remarks>
public class SignalTimeoutResult<TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler<TParameter>> returnValue,
	SignalerOptions<TParameter> options)
	: AndOrResult<SignalerResult<TParameter>, IThat<Signaler<TParameter>>>(expectationBuilder, returnValue),
		IOptionsProvider<SignalerOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => options;

	/// <summary>
	///     Specifies a timeout for waiting on the callback.
	/// </summary>
	public SignalTimeoutResult<TParameter> Within(TimeSpan timeout)
	{
		options.Timeout = timeout;
		return this;
	}

	/// <summary>
	///     Specifies a predicate to filter for signals with a matching parameter.
	/// </summary>
	public SignalTimeoutResult<TParameter> With(
		Func<TParameter, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		options.WithPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace());
		return this;
	}
}
