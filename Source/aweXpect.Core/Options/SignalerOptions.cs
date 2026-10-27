using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Signaling;

namespace aweXpect.Options;

/// <summary>
///     The timeout options for <see cref="Signaler" /> and <see cref="Signaler{TParameter}" />.
/// </summary>
/// <remarks>
///     An immutable value: <see cref="Within(TimeSpan)" /> returns new options, and <see langword="default" /> has no
///     timeout, like <see cref="Default" />.<br />
///     The predicates that filter the signals of a <see cref="Signaler{TParameter}" /> are in the
///     <see cref="SignalerPredicateOptions{TParameter}" />.
/// </remarks>
[ImmutableOptions]
public readonly struct SignalerOptions
{
	/// <summary>
	///     The complement of the ticks of the timeout, or <c>0</c> without a timeout.
	/// </summary>
	/// <remarks>
	///     The complement of a valid timeout is never <c>0</c>, as that would be a negative timeout of one tick, so the
	///     value is as small as a <see cref="TimeSpan" />.
	/// </remarks>
	private readonly long _timeout;

	private SignalerOptions(long timeout)
	{
		_timeout = timeout;
	}

	/// <summary>
	///     The options without a timeout.
	/// </summary>
	public static SignalerOptions Default => default;

	/// <summary>
	///     The timeout to use for the recording.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.
	/// </remarks>
	public TimeSpan? Timeout => _timeout == 0 ? null : new TimeSpan(~_timeout);

	/// <summary>
	///     Specifies the <paramref name="timeout" /> to use for the recording.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.<br />
	///     A later timeout replaces the earlier one.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
#pragma warning disable CA1822 // An instance method like the option methods of the other options, so that options.Within(…) compiles
	public SignalerOptions Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		return new SignalerOptions(~timeout.Ticks);
	}
#pragma warning restore CA1822

	/// <inheritdoc cref="object.ToString()" />
	/// <remarks>
	///     An infinite timeout is omitted, because it does not add any information to the expectation.
	/// </remarks>
	public override string ToString()
		=> Timeout is not { } timeout || timeout == System.Threading.Timeout.InfiniteTimeSpan
			? ""
			: $" within {Formatter.Format(timeout)}";
}
