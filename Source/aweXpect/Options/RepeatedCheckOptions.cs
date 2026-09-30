using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
	///     How close to the timeout a cancellation still counts as the timeout having elapsed, and how much of the
	///     timeout may remain after a wait for that wait to still be the last one.
	/// </summary>
	/// <remarks>
	///     <see cref="Task.Delay(TimeSpan, CancellationToken)" /> truncates to whole milliseconds and its timer does not
	///     share the clock of the stopwatch that measures the timeout.
	/// </remarks>
	private static readonly TimeSpan CancellationTolerance = TimeSpan.FromMilliseconds(2);

	/// <summary>
	///     The largest wait that <see cref="Task.Delay(TimeSpan, CancellationToken)" /> accepts on every target framework.
	/// </summary>
	private static readonly TimeSpan MaximumWait = TimeSpan.FromMilliseconds(int.MaxValue);

	private TimeSpan? _interval;
	private bool _isIntervalSpecified;
	private bool _isTimeoutSpecified;

	/// <summary>
	///     The interval in which the condition should be checked.
	/// </summary>
	/// <remarks>
	///     Defaults to <c>Customize.aweXpect.Settings().DefaultCheckInterval</c> if not specified.
	/// </remarks>
	public TimeSpan Interval
	{
		get
		{
			_interval ??= Customize.aweXpect.Settings().DefaultCheckInterval.Get();
			return _interval.Value;
		}
	}

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

		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIntervalSpecified, nameof(CheckEvery));
		_isIntervalSpecified = true;
		_interval = interval;
	}

	/// <summary>
	///     Makes the <paramref name="check" /> and repeats it in the <see cref="Interval" /> until it succeeds or the
	///     <see cref="Timeout" /> has elapsed, and returns whether it succeeded.
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
	///             succeeds or the <paramref name="cancellationToken" /> is canceled.
	///         </item>
	///         <item>
	///             A cancellation of the <paramref name="cancellationToken" /> counts as the <see cref="Timeout" />
	///             having elapsed, so that one last check decides the result, when it occurs at the
	///             <see cref="Timeout" />, or when the <paramref name="expectationBuilder" /> has a timeout that is not
	///             shorter than the <see cref="Timeout" /> and its cancellation token was not canceled. Any other
	///             cancellation, e.g. by the caller, and every cancellation with an infinite <see cref="Timeout" /> is
	///             thrown as <see cref="OperationCanceledException" />.
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
	/// <param name="expectationBuilder">
	///     The expectation builder of the constraint, whose timeout and cancellation decide how a cancellation of the
	///     <paramref name="cancellationToken" /> is treated.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that the constraint received.</param>
	public async Task<bool> CheckRepeatedly(Func<Task<bool>> check,
		ExpectationBuilder expectationBuilder,
		CancellationToken cancellationToken)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TimeSpan? canceledAt = null;
		TaskCompletionSource<bool> cancellation = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using CancellationTokenRegistration registration = cancellationToken.Register(() =>
		{
			// The time has to be recorded before the wait is released, so that the loop always observes it.
			canceledAt ??= stopwatch.Elapsed;
			cancellation.TrySetResult(true);
		});

		if (await check())
		{
			return true;
		}

		if (!IsRepeated)
		{
			return false;
		}

		TimeSpan interval = Interval;
		bool isLastCheck = false;
		while (!isLastCheck)
		{
			TimeSpan remaining = IsInfinite ? TimeSpan.MaxValue : Timeout - stopwatch.Elapsed;
			if (remaining <= TimeSpan.Zero)
			{
				return false;
			}

			TimeSpan wait = NextWait(interval, remaining);
			// The timer of the wait can complete a fraction of a millisecond before the stopwatch agrees, so a wait
			// that would only leave a sliver of the timeout is the last one.
			isLastCheck = remaining - wait < CancellationTolerance;
			if (await IsCanceledDuring(wait, cancellation.Task))
			{
				ThrowUnlessTimeoutIsReachedAt(canceledAt!.Value, expectationBuilder, cancellationToken);
				isLastCheck = true;
			}

			if (await check())
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Waits at most the <paramref name="remaining" /> time. A non-positive <paramref name="interval" /> checks again
	///     without waiting.
	/// </summary>
	/// <remarks>
	///     The result is capped at <see cref="MaximumWait" />, because an infinite timeout does not limit the interval.
	/// </remarks>
	private static TimeSpan NextWait(TimeSpan interval, TimeSpan remaining)
	{
		if (interval <= TimeSpan.Zero)
		{
			return TimeSpan.Zero;
		}

		TimeSpan wait = interval < remaining ? interval : remaining;
		return wait < MaximumWait ? wait : MaximumWait;
	}

	/// <summary>
	///     Throws the cancellation of the <paramref name="cancellationToken" />, unless it counts as the
	///     <see cref="Timeout" /> having elapsed.
	/// </summary>
	private void ThrowUnlessTimeoutIsReachedAt(TimeSpan canceledAt, ExpectationBuilder expectationBuilder,
		CancellationToken cancellationToken)
	{
		if (!IsTimeoutReachedAt(canceledAt, expectationBuilder))
		{
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	/// <summary>
	///     Whether a cancellation at <paramref name="canceledAt" /> counts as the <see cref="Timeout" /> having elapsed,
	///     so that the last check decides instead of the cancellation.
	/// </summary>
	/// <remarks>
	///     Like for <c>Eventually()</c>, the <see cref="Timeout" /> decides when the outer timeout of the
	///     <paramref name="expectationBuilder" /> is not shorter. The timer of the outer timeout starts before the subject
	///     is evaluated, so it can expire slightly before the <see cref="Timeout" /> does.
	/// </remarks>
	private bool IsTimeoutReachedAt(TimeSpan canceledAt, ExpectationBuilder expectationBuilder)
	{
		if (IsInfinite)
		{
			return false;
		}

		if (Timeout - canceledAt < CancellationTolerance)
		{
			return true;
		}

		return expectationBuilder.Timeout >= Timeout &&
		       expectationBuilder.CancellationToken?.IsCancellationRequested != true;
	}

	/// <summary>
	///     Waits for the <paramref name="wait" /> and returns whether the <paramref name="cancellation" /> cut it short.
	/// </summary>
	/// <remarks>
	///     A wait of zero still yields, so that checking without waiting does not block the thread.
	/// </remarks>
	private static async Task<bool> IsCanceledDuring(TimeSpan wait, Task cancellation)
	{
		if (wait <= TimeSpan.Zero)
		{
			await Task.Yield();
			return cancellation.IsCompleted;
		}

		using CancellationTokenSource waitCts = new();
		Task delay = Task.Delay(wait, waitCts.Token);
		if (await Task.WhenAny(cancellation, delay) != cancellation)
		{
			return false;
		}

		waitCts.Cancel();
		return true;
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
