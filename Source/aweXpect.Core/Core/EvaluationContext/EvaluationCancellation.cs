using System;
using System.Threading;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     The cancellation of one evaluation: the token that the constraints observe, and whether a cancellation came from
///     the timeout of the expectation or from the caller.
/// </summary>
public sealed class EvaluationCancellation
{
	/// <summary>
	///     How close to the end of a wait a cancellation still counts as the wait having elapsed.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Tasks.Task.Delay(TimeSpan, CancellationToken)" /> truncates to whole milliseconds
	///     and the timers of the waits and of the timeout do not share the clock of the stopwatch that measures them.
	/// </remarks>
	internal static readonly TimeSpan Tolerance = TimeSpan.FromMilliseconds(2);

	private readonly CancellationToken _callerToken;
	private readonly CancellationTokenSource? _timeoutCts;

	internal EvaluationCancellation(TimeSpan? timeout, CancellationToken callerToken)
	{
		Timeout = timeout;
		_callerToken = callerToken;
		Token = callerToken;
		if (timeout is not null)
		{
			_timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(callerToken);
			_timeoutCts.CancelAfter(timeout.Value.ToTimerTimeout());
			Token = _timeoutCts.Token;
		}
	}

	/// <summary>
	///     An evaluation without a timeout that cannot be canceled.
	/// </summary>
	public static EvaluationCancellation None { get; } = new(null, CancellationToken.None);

	/// <summary>
	///     The token for the evaluation, which is also canceled when the <see cref="Timeout" /> elapses.
	/// </summary>
	public CancellationToken Token { get; }

	/// <summary>
	///     The effective timeout of the evaluation: the tighter of <c>WithTimeout(…)</c> and the timeout of
	///     <c>Customize.aweXpect.Settings().TestCancellation</c>, or <see langword="null" /> without a timeout.
	/// </summary>
	public TimeSpan? Timeout { get; }

	/// <summary>
	///     Whether the <see cref="Timeout" /> elapsed, while the caller did not cancel the evaluation.
	/// </summary>
	public bool IsTimeoutElapsed
		=> _timeoutCts?.IsCancellationRequested == true && !_callerToken.IsCancellationRequested;

	/// <summary>
	///     Whether the caller canceled the evaluation, with <c>WithCancellation(…)</c> or with the token of
	///     <c>Customize.aweXpect.Settings().TestCancellation</c>.
	/// </summary>
	public bool IsCallerCanceled => _callerToken.IsCancellationRequested;

	/// <summary>
	///     Whether a cancellation that ended a wait of at most <paramref name="waitTimeout" /> after
	///     <paramref name="waited" /> counts as the <paramref name="waitTimeout" /> having elapsed, so that the result at
	///     that time decides instead of the cancellation.
	/// </summary>
	/// <remarks>
	///     This is the case, when the cancellation came at the end of the wait, or when the <see cref="Timeout" /> elapsed
	///     and is not shorter than the <paramref name="waitTimeout" />: its timer started before the wait, so it can expire
	///     slightly before the wait does. A cancellation by the caller, a shorter <see cref="Timeout" /> and every
	///     cancellation of an infinite <paramref name="waitTimeout" /> end the wait without a decision.
	/// </remarks>
	public bool CountsAsElapsed(TimeSpan waitTimeout, TimeSpan waited)
	{
		if (waitTimeout == System.Threading.Timeout.InfiniteTimeSpan || waitTimeout == TimeSpan.MaxValue)
		{
			return false;
		}

		return waitTimeout - waited < Tolerance || (IsTimeoutElapsed && Timeout >= waitTimeout);
	}

	internal bool HasTimedOut(Exception? exception)
		=> exception is OperationCanceledException && IsTimeoutElapsed;

	internal bool IsCanceledBy(Exception? exception)
		=> exception is OperationCanceledException && IsCallerCanceled;

	/// <summary>
	///     Releases the timer of the <see cref="Timeout" />.
	/// </summary>
	/// <remarks>
	///     Only the evaluation that created the cancellation releases it, so it is not part of the public API.
	/// </remarks>
	internal void Release() => _timeoutCts?.Dispose();

	/// <summary>
	///     Returns a scope that releases the timer of the <see cref="Timeout" /> when it is disposed.
	/// </summary>
	internal ReleaseScope ReleaseAtTheEnd() => new(this);

	/// <summary>
	///     Releases the <see cref="EvaluationCancellation" /> at the end of a <see langword="using" /> block without
	///     allocating.
	/// </summary>
	internal readonly struct ReleaseScope(EvaluationCancellation cancellation) : IDisposable
	{
		public void Dispose() => cancellation.Release();
	}
}
