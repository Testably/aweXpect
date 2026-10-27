using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a <see cref="Signaler" /> was signaled a given number of times, which allows
///     specifying the timeout via <see cref="SignalerOptionsExtensions" />.
/// </summary>
/// <remarks>
///     The number of times is already given, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountResult" /> to specify how often a callback must be signaled.
/// </remarks>
public class SignaledResult
	: AndOrResult<SignalerResult, IThat<Signaler?>, SignaledResult>,
		IOptionsProvider<SignalerOptions>
{
	private readonly IOptionsHolderSource<SignalerOptions> _options;

	internal SignaledResult(
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
