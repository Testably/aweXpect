using System;
using aweXpect.Core.Helpers;
using aweXpect.Signaling;

namespace aweXpect.Options;

/// <summary>
///     The timeout options for <see cref="Signaler" /> and <see cref="Signaler{TParameter}" />.
/// </summary>
/// <remarks>
///     The predicates that filter the signals of a <see cref="Signaler{TParameter}" /> are in the
///     <see cref="SignalerPredicateOptions{TParameter}" />.
/// </remarks>
public class SignalerOptions
{
	/// <summary>
	///     The timeout to use for the recording.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.
	/// </remarks>
	public TimeSpan? Timeout { get; private set; }

	/// <summary>
	///     Specifies the <paramref name="timeout" /> to use for the recording.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.<br />
	///     A later timeout replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	public SignalerOptions Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		Timeout = timeout;
		return this;
	}

	/// <inheritdoc cref="object.ToString()" />
	/// <remarks>
	///     An infinite timeout is omitted, because it does not add any information to the expectation.
	/// </remarks>
	public override string ToString()
		=> Timeout is null || Timeout == System.Threading.Timeout.InfiniteTimeSpan
			? ""
			: $" within {Formatter.Format(Timeout.Value)}";
}
