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
	private readonly OptionsSlot<SignalerOptions> _options;

	internal DidNotSignalResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler> returnValue,
		OptionsSlot<SignalerOptions> options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<SignalerOptions> IOptionsProvider<SignalerOptions>.Options => _options;
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
		IOptionsProvider<SignalerOptions>,
		ISignalerResult<DidNotSignalResult<TParameter>, TParameter>
{
	private readonly OptionsSlot<SignalerOptions> _options;
	private readonly OptionsSlot<SignalerPredicateOptions<TParameter>> _predicates;

	internal DidNotSignalResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler<TParameter>> returnValue,
		OptionsSlot<SignalerOptions> options,
		OptionsSlot<SignalerPredicateOptions<TParameter>> predicates)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_predicates = predicates;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<SignalerOptions> IOptionsProvider<SignalerOptions>.Options => _options;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsSlot<SignalerPredicateOptions<TParameter>> IOptionsProvider<SignalerPredicateOptions<TParameter>>.Options
		=> _predicates;
}
