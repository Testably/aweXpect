using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Helpers;

/// <summary>
///     What a repeated check does after <see cref="Polling.WaitForNextCheck" />.
/// </summary>
internal enum PollStep
{
	/// <summary>
	///     Check again, further checks may follow.
	/// </summary>
	Check,

	/// <summary>
	///     Check a last time, the budget is used up after it.
	/// </summary>
	LastCheck,

	/// <summary>
	///     The budget is used up, so no further check is made.
	/// </summary>
	Elapsed,

	/// <summary>
	///     The evaluation was canceled before the budget was used up, so the checks decide nothing.
	/// </summary>
	Canceled,
}

/// <summary>
///     Waits between the checks of a repeated check, until a budget is used up or the evaluation is canceled.
/// </summary>
/// <remarks>
///     A wait is shortened to the remaining budget, so the last check is made when the budget is used up and none after
///     it. Whether a cancellation during a wait still lets the last check decide is decided by
///     <see cref="EvaluationCancellation.HasWaitElapsed(TimeSpan, TimeSpan)" />.
/// </remarks>
internal sealed class Polling : IDisposable
{
	/// <summary>
	///     The largest wait that <see cref="Task.Delay(TimeSpan, CancellationToken)" /> accepts on every target framework.
	/// </summary>
	private static readonly TimeSpan MaximumWait = TimeSpan.FromMilliseconds(int.MaxValue);

	private readonly TimeSpan _budget;
	private readonly TaskCompletionSource<bool> _canceled = new(TaskCreationOptions.RunContinuationsAsynchronously);
	private readonly EvaluationCancellation _cancellation;
	private readonly TimeSpan _interval;
	private readonly CancellationTokenRegistration _registration;
	private readonly long _startTimestamp;
	private TimeSpan? _canceledAt;
	private bool _isLastCheck;

	private Polling(long startTimestamp, TimeSpan budget, TimeSpan interval, EvaluationCancellation cancellation)
	{
		_startTimestamp = startTimestamp;
		_budget = budget;
		_interval = interval;
		_cancellation = cancellation;
		_registration = cancellation.Token.Register(() =>
		{
			// The time has to be recorded before the wait is released, so that the loop always observes it.
			_canceledAt ??= GetElapsedTime(_startTimestamp);
			_canceled.TrySetResult(true);
		});
	}

	/// <summary>
	///     The time since the start, or until the cancellation, once one arrived.
	/// </summary>
	public TimeSpan Elapsed => _canceledAt ?? GetElapsedTime(_startTimestamp);

	/// <summary>
	///     The remaining budget, or <see cref="TimeSpan.MaxValue" /> for an unlimited budget.
	/// </summary>
	public TimeSpan Remaining => IsUnlimited ? TimeSpan.MaxValue : _budget - Elapsed;

	private bool IsUnlimited => _budget == Timeout.InfiniteTimeSpan || _budget == TimeSpan.MaxValue;

	/// <inheritdoc />
	public void Dispose() => _registration.Dispose();

	/// <summary>
	///     Starts polling with the <paramref name="budget" />, measured from the <paramref name="startTimestamp" />.
	/// </summary>
	/// <param name="startTimestamp">The <see cref="Stopwatch.GetTimestamp()" /> before the first check.</param>
	/// <param name="budget">
	///     The time the checks may take; <see cref="Timeout.InfiniteTimeSpan" /> or <see cref="TimeSpan.MaxValue" /> is
	///     unlimited.
	/// </param>
	/// <param name="interval">The time between two checks; a non-positive interval checks again without waiting.</param>
	/// <param name="cancellation">The cancellation of the evaluation.</param>
	public static Polling Start(long startTimestamp, TimeSpan budget, TimeSpan interval,
		EvaluationCancellation cancellation)
		=> new(startTimestamp, budget, interval, cancellation);

	/// <summary>
	///     Waits for the next check.
	/// </summary>
	public async Task<PollStep> WaitForNextCheck()
	{
		TimeSpan remaining = Remaining;
		if (_isLastCheck || remaining <= TimeSpan.Zero)
		{
			return PollStep.Elapsed;
		}

		TimeSpan wait = NextWait(remaining);
		// The timer of the wait can complete a fraction of a millisecond before the stopwatch agrees, so a wait that
		// would only leave a sliver of the budget is the last one.
		_isLastCheck = remaining - wait < EvaluationCancellation.Tolerance;
		if (await IsCanceledDuring(wait))
		{
			if (!_cancellation.HasWaitElapsed(_budget, Elapsed))
			{
				return PollStep.Canceled;
			}

			_isLastCheck = true;
		}

		return _isLastCheck ? PollStep.LastCheck : PollStep.Check;
	}

	/// <summary>
	///     Waits at most the <paramref name="remaining" /> budget.
	/// </summary>
	/// <remarks>
	///     The result is capped at <see cref="MaximumWait" />, because an unlimited budget does not limit the interval.
	/// </remarks>
	private TimeSpan NextWait(TimeSpan remaining)
	{
		if (_interval <= TimeSpan.Zero)
		{
			return TimeSpan.Zero;
		}

		TimeSpan wait = _interval < remaining ? _interval : remaining;
		return wait < MaximumWait ? wait : MaximumWait;
	}

	/// <summary>
	///     Waits for the <paramref name="wait" /> and returns whether the cancellation cut it short.
	/// </summary>
	/// <remarks>
	///     A wait of zero still yields, so that checking without waiting does not block the thread. The wait is not
	///     canceled by the token itself: <see cref="Task.Delay(TimeSpan, CancellationToken)" /> would register its own
	///     callback, and the callbacks run in reverse order, so the wait could continue before the time of the
	///     cancellation was recorded.
	/// </remarks>
	private async Task<bool> IsCanceledDuring(TimeSpan wait)
	{
		if (wait <= TimeSpan.Zero)
		{
			await Task.Yield();
			return _canceled.Task.IsCompleted;
		}

		using CancellationTokenSource waitCts = new();
		Task delay = Task.Delay(wait, waitCts.Token);
		if (await Task.WhenAny(_canceled.Task, delay) != _canceled.Task)
		{
			return false;
		}

		waitCts.Cancel();
		return true;
	}

	private static TimeSpan GetElapsedTime(long startTimestamp)
		=> TimeSpan.FromTicks((long)((Stopwatch.GetTimestamp() - startTimestamp) *
		                             ((double)TimeSpan.TicksPerSecond / Stopwatch.Frequency)));
}
