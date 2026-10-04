using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for the absence of a signal, which allows specifying the timeout via
///     <see cref="SignalerOptionsExtensions" />.
/// </summary>
/// <remarks>
///     Absence is not an occurrence, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountResult" /> to count how often a callback was signaled.
/// </remarks>
public class DidNotSignalResult(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler> returnValue,
	SignalerOptions options)
	: AndOrResult<SignalerResult, IThat<Signaler>, DidNotSignalResult>(expectationBuilder, returnValue),
		IOptionsProvider<SignalerOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => options;
}

/// <summary>
///     The result for the absence of a signal with <typeparamref name="TParameter" />, which allows specifying the
///     timeout and filtering the signals by their parameter via <see cref="SignalerOptionsExtensions" />.
/// </summary>
/// <remarks>
///     Absence is not an occurrence, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountWhoseResult{TParameter}" /> to count how often a callback was signaled.
/// </remarks>
public class DidNotSignalResult<TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler<TParameter>> returnValue,
	SignalerOptions<TParameter> options)
	: AndOrResult<SignalerResult<TParameter>, IThat<Signaler<TParameter>>, DidNotSignalResult<TParameter>>(
			expectationBuilder, returnValue),
		ISignalerResult<DidNotSignalResult<TParameter>, TParameter>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions<TParameter> IOptionsProvider<SignalerOptions<TParameter>>.Options => options;
}
