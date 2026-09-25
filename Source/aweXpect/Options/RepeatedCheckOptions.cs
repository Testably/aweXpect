using System;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Options;

/// <summary>
///     The options for a <see cref="RepeatedCheckResult{TType,TThat}" />.
/// </summary>
public class RepeatedCheckOptions
{
	/// <summary>
	///     The default interval in which to check the condition again is <c>100ms</c>.
	/// </summary>
	public static readonly TimeSpan DefaultInterval = TimeSpan.FromMilliseconds(100);

	private ICheckInterval? _interval;
	private bool _isIntervalSpecified;
	private bool _isTimeoutSpecified;

	/// <summary>
	///     The interval in which the condition should be checked.
	/// </summary>
	public ICheckInterval Interval
	{
		get
		{
			_interval ??= new FixedCheckInterval(Customize.aweXpect.Settings().DefaultCheckInterval.Get());
			return _interval;
		}
		private set => _interval = value;
	}

	/// <summary>
	///     The timeout until the condition must be met.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> checks the condition again until it is met.
	/// </remarks>
	public TimeSpan Timeout { get; private set; } = TimeSpan.Zero;

	/// <summary>
	///     Whether the condition is checked again after the first check.
	/// </summary>
	internal bool IsRepeated => Timeout > TimeSpan.Zero || IsInfinite;

	private bool IsInfinite => Timeout == System.Threading.Timeout.InfiniteTimeSpan;

	/// <summary>
	///     Allows a <paramref name="timeout" /> until the condition must be met.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> imposes no limit.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A timeout is already set.</exception>
	public void Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isTimeoutSpecified, nameof(Within));
		_isTimeoutSpecified = true;
		Timeout = timeout;
	}

	/// <summary>
	///     Sets the interval in which the condition should be checked.
	/// </summary>
	/// <remarks>
	///     Defaults to <see cref="RepeatedCheckOptions.DefaultInterval" /> if not specified.
	/// </remarks>
	/// <exception cref="InvalidOperationException">An interval is already set.</exception>
	public void CheckEvery(TimeSpan interval)
	{
		if (interval <= TimeSpan.Zero)
		{
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(interval), "The interval must be positive."));
		}

		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIntervalSpecified, nameof(CheckEvery));
		_isIntervalSpecified = true;
		Interval = new FixedCheckInterval(interval);
	}

	/// <summary>
	///     Whether the <paramref name="elapsed" /> time still leaves room for another check.
	/// </summary>
	internal bool IsWithinTimeout(TimeSpan elapsed) => IsInfinite || elapsed <= Timeout;

	/// <inheritdoc cref="object.ToString()" />
	/// <remarks>
	///     An infinite timeout is omitted, because it does not add any information to the expectation.
	/// </remarks>
	public override string ToString()
	{
		if (!_isTimeoutSpecified || IsInfinite)
		{
			return "";
		}

		return $" within {Formatter.Format(Timeout)}";
	}
}
