using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying how often a <see cref="Signaler" /> was signaled, which allows specifying the timeout.
/// </summary>
/// <remarks>
///     The options are specified via <see cref="QuantifierExtensions" /> and <see cref="SignalerOptionsExtensions" />.
/// </remarks>
public class SignalCountResult
	: AndOrResult<SignalerResult, IThat<Signaler?>, SignalCountResult>,
		IOptionsProvider<Quantifier>,
		IOptionsProvider<SignalerOptions>
{
	private readonly OptionsHolder<SignalerOptions> _options;
	private readonly OptionsHolder<Quantifier> _quantifier;

	internal SignalCountResult(
		ExpectationBuilder expectationBuilder,
		IThat<Signaler?> returnValue,
		OptionsHolder<Quantifier> quantifier,
		OptionsHolder<SignalerOptions> options)
		: base(expectationBuilder, returnValue)
	{
		_quantifier = quantifier;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<Quantifier> IOptionsProvider<Quantifier>.Options => _quantifier;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<SignalerOptions> IOptionsProvider<SignalerOptions>.Options => _options;
}
