using System;
using aweXpect.Core;
using aweXpect.Customization;
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
	public TimeSpan Timeout { get; private set; } = TimeSpan.Zero;

	/// <summary>
	///     Allows a <paramref name="timeout" /> until the condition must be met.
	/// </summary>
	public void Within(TimeSpan timeout)
	{
		if (timeout < TimeSpan.Zero)
		{
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(timeout), "The timeout must not be negative."));
		}

		Timeout = timeout;
	}

	/// <summary>
	///     Sets the interval in which the condition should be checked.
	/// </summary>
	/// <remarks>
	///     Defaults to <see cref="RepeatedCheckOptions.DefaultInterval" /> if not specified.
	/// </remarks>
	public void CheckEvery(TimeSpan interval)
	{
		if (interval <= TimeSpan.Zero)
		{
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(interval), "The interval must be positive."));
		}

		Interval = new FixedCheckInterval(interval);
	}

	/// <inheritdoc cref="object.ToString()" />
	public override string ToString()
	{
		if (Timeout == TimeSpan.Zero)
		{
			return "";
		}

		return $" within {Formatter.Format(Timeout)}";
	}
}
