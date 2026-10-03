using System;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Signaling;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that a <see cref="Signaler" /> was signaled a given number of times, which allows
///     specifying the timeout.
/// </summary>
/// <remarks>
///     The number of times is already given, so this result intentionally carries no quantifiers: use
///     <see cref="SignalCountResult" /> to specify how often a callback must be signaled.
/// </remarks>
public class SignaledResult(
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
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A timeout is already set.</exception>
	public SignaledResult Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(options.Timeout is not null, nameof(Within));
		options.Timeout = timeout;
		return this;
	}
}
