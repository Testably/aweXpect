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
public class SignalCountResult(
	ExpectationBuilder expectationBuilder,
	IThat<Signaler> returnValue,
	Quantifier quantifier,
	SignalerOptions options)
	: AndOrResult<SignalerResult, IThat<Signaler>, SignalCountResult>(expectationBuilder, returnValue),
		IOptionsProvider<Quantifier>,
		IOptionsProvider<SignalerOptions>
{
	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	Quantifier IOptionsProvider<Quantifier>.Options => quantifier;

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	SignalerOptions IOptionsProvider<SignalerOptions>.Options => options;
}
