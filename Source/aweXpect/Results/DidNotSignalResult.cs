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
public class DidNotSignalResult
	: AndOrResult<SignalerResult, IThat<Signaler>, DidNotSignalResult>,
		IOptionsProvider<SignalerOptions>
{
	private readonly SignalerOptions _options;

	internal DidNotSignalResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler> returnValue,
		SignalerOptions options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => _options;
}

/// <summary>
///     The result for the absence of a signal with <typeparamref name="TParameter" />, which allows specifying the
///     timeout and filtering the signals by their parameter via <see cref="SignalerOptionsExtensions" />.
/// </summary>
/// <remarks>
///     Absence is not an occurrence, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountWhoseResult{TParameter}" /> to count how often a callback was signaled.
/// </remarks>
public class DidNotSignalResult<TParameter>
	: AndOrResult<SignalerResult<TParameter>, IThat<Signaler<TParameter>>, DidNotSignalResult<TParameter>>,
		ISignalerResult<DidNotSignalResult<TParameter>, TParameter>
{
	private readonly SignalerOptions<TParameter> _options;

	internal DidNotSignalResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler<TParameter>> returnValue,
		SignalerOptions<TParameter> options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions<TParameter> IOptionsProvider<SignalerOptions<TParameter>>.Options => _options;
}
