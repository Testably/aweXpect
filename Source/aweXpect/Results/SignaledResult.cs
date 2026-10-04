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
public class SignaledResult(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler> returnValue,
	SignalerOptions options)
	: AndOrResult<SignalerResult, IThat<Signaler>, SignaledResult>(expectationBuilder, returnValue),
		IOptionsProvider<SignalerOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => options;
}
