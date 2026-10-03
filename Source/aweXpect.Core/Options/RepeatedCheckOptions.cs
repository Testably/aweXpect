using System;
using System.Diagnostics;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Core.Helpers;
using aweXpect.Results;

namespace aweXpect.Options;

/// <summary>
///     The options for a <see cref="RepeatedCheckResult{TType,TThat}" />.
/// </summary>
public class RepeatedCheckOptions
{
	private TimeSpan? _interval;
	private bool _isTimeoutSpecified;

	/// <summary>
	///     The interval in which the condition should be checked.
	/// </summary>
	/// <remarks>
	///     Defaults to the current <c>Customize.aweXpect.Settings().DefaultCheckInterval</c> if not specified.
	/// </remarks>
	public TimeSpan Interval => _interval ?? Customize.aweXpect.Settings().DefaultCheckInterval.Get();

	/// <summary>
	///     The timeout until the condition must be met.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> checks the condition again until it is met.
	/// </remarks>
	public TimeSpan Timeout { get; private set; } = TimeSpan.Zero;

	/// <summary>
	///     Whether <see cref="CheckRepeatedly" /> checks the condition again after the first check.
	/// </summary>
	/// <remarks>
	///     This is the case when <see cref="Within(TimeSpan)" /> set a positive timeout or
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" />.
	/// </remarks>
	public bool IsRepeated => Timeout > TimeSpan.Zero || IsInfinite;

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
	///     Defaults to <c>Customize.aweXpect.Settings().DefaultCheckInterval</c> if not specified.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="interval" /> is not positive.</exception>
	/// <exception cref="InvalidOperationException">An interval is already set.</exception>
	public void CheckEvery(TimeSpan interval)
	{
		if (interval <= TimeSpan.Zero)
		{
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(interval), "The interval must be positive."));
		}

		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_interval is not null, nameof(CheckEvery));
		_interval = interval;
	}

	/// <summary>
	///     Makes the <paramref name="check" /> and repeats it in the <see cref="Interval" /> until it succeeds or the
	///     <see cref="Timeout" /> has elapsed.
	/// </summary>
	/// <remarks>
	///     <list type="bullet">
	///         <item>
	///             The first check is made immediately. When it succeeds, or when <see cref="IsRepeated" /> is
	///             <see langword="false" />, no further check is made.
	///         </item>
	///         <item>
	///             A wait is shortened to the remaining time, so the last check is made at the <see cref="Timeout" />
	///             and none after it. The <see cref="Timeout" /> is measured from the start of each call.
	///         </item>
	///         <item>
	///             With <see cref="System.Threading.Timeout.InfiniteTimeSpan" />, the check is repeated until it
	///             succeeds or the evaluation is canceled.
	///         </item>
	///         <item>
	///             The cancellation of the evaluation is only observed while waiting for the next check, so the first
	///             check is made even when the evaluation is already canceled.
	///         </item>
	///         <item>
	///             A cancellation during a wait lets one last check decide, when it counts as the
	///             <see cref="Timeout" /> having elapsed (see
	///             <see cref="EvaluationCancellation.HasWaitElapsed(TimeSpan, TimeSpan)" />): when it came at the
	///             <see cref="Timeout" />, or when the effective timeout of the evaluation is not shorter than the
	///             <see cref="Timeout" />. Any other cancellation, e.g. by the caller or by a shorter timeout, ends the
	///             checks with <see cref="Outcome.Undecided" />.
	///         </item>
	///         <item>
	///             An exception thrown by the <paramref name="check" /> is not caught.
	///         </item>
	///     </list>
	/// </remarks>
	/// <param name="check">
	///     The check, which returns whether the expectation is met. For a negated constraint, it returns
	///     <see langword="true" /> when the condition is not met.
	/// </param>
	/// <param name="context">The evaluation context that the constraint received.</param>
	/// <returns>
	///     <see cref="Outcome.Success" /> when a check returned <see langword="true" />, <see cref="Outcome.Failure" /> when
	///     the last check returned <see langword="false" />, and <see cref="Outcome.Undecided" /> when the evaluation was
	///     canceled before the <see cref="Timeout" />, by the caller or by a shorter timeout.
	/// </returns>
	public async Task<Outcome> CheckRepeatedly(Func<Task<bool>> check, IEvaluationContext context)
	{
		long startTimestamp = Stopwatch.GetTimestamp();
		if (await check())
		{
			return Outcome.Success;
		}

		if (!IsRepeated)
		{
			return Outcome.Failure;
		}

		using Polling polling = Polling.Start(startTimestamp, Timeout, Interval, context.Cancellation);
		while (true)
		{
			switch (await polling.WaitForNextCheck())
			{
				case PollStep.Elapsed:
					return Outcome.Failure;
				case PollStep.Canceled:
					return Outcome.Undecided;
			}

			if (await check())
			{
				return Outcome.Success;
			}
		}
	}

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
