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
	: AndOrResult<SignalerResult, IThat<Signaler?>, DidNotSignalResult>,
		IOptionsProvider<SignalerOptions>
{
	private readonly IOptionsHolderSource<SignalerOptions> _options;

	internal DidNotSignalResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler?> returnValue,
		IOptionsHolderSource<SignalerOptions> options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<SignalerOptions> IOptionsProvider<SignalerOptions>.Options => _options.Holder;
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
	: AndOrResult<SignalerResult<TParameter>, IThat<Signaler<TParameter>?>, DidNotSignalResult<TParameter>>,
		IOptionsProvider<SignalerOptions>,
		ISignalerResult<DidNotSignalResult<TParameter>, TParameter>
{
	private readonly IOptionsHolderSource<SignalerOptions> _options;
	private readonly IOptionsHolderSource<SignalerPredicateOptions<TParameter>> _predicates;

	internal DidNotSignalResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler<TParameter>?> returnValue,
		IOptionsHolderSource<SignalerOptions> options,
		IOptionsHolderSource<SignalerPredicateOptions<TParameter>> predicates)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_predicates = predicates;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<SignalerOptions> IOptionsProvider<SignalerOptions>.Options => _options.Holder;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<SignalerPredicateOptions<TParameter>> IOptionsProvider<SignalerPredicateOptions<TParameter>>.Options
		=> _predicates.Holder;
}
