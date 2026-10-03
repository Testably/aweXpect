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
	///     Creates the cancellation of an evaluation with the <paramref name="timeout" /> and the
	///     <paramref name="callerToken" />.
	/// </summary>
	/// <remarks>
	///     Most evaluations have neither, so they share <see cref="None" />, which holds no timer to release.
	/// </remarks>
	internal static EvaluationCancellation Create(TimeSpan? timeout, CancellationToken callerToken)
		=> timeout is null && !callerToken.CanBeCanceled ? None : new EvaluationCancellation(timeout, callerToken);

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
	///     Why the evaluation was canceled.
	/// </summary>
	public CancellationReason Reason
	{
		get
		{
			if (_callerToken.IsCancellationRequested)
			{
				return CancellationReason.Caller;
			}

			return _timeoutCts?.IsCancellationRequested == true ? CancellationReason.Timeout : CancellationReason.None;
		}
	}

	/// <summary>
	///     Whether a wait of at most <paramref name="waitTimeout" /> that a cancellation ended after
	///     <paramref name="waited" /> counts as elapsed, so that the result at that time decides instead of the
	///     cancellation.
	/// </summary>
	/// <remarks>
	///     This is the case, when the cancellation came at the end of the wait, or when the <see cref="Timeout" /> elapsed
	///     and is not shorter than the <paramref name="waitTimeout" />: its timer started before the wait, so it can expire
	///     slightly before the wait does. A cancellation by the caller, a shorter <see cref="Timeout" /> and every
	///     cancellation of an infinite <paramref name="waitTimeout" /> end the wait without a decision.
	/// </remarks>
	public bool HasWaitElapsed(TimeSpan waitTimeout, TimeSpan waited)
	{
		if (waitTimeout == System.Threading.Timeout.InfiniteTimeSpan || waitTimeout == TimeSpan.MaxValue)
		{
			return false;
		}

		return waitTimeout - waited < Tolerance || (Reason == CancellationReason.Timeout && Timeout >= waitTimeout);
	}

	internal bool HasTimedOut(Exception? exception)
		=> exception is OperationCanceledException && Reason == CancellationReason.Timeout;

	internal bool IsCanceledBy(Exception? exception)
		=> exception is OperationCanceledException && Reason == CancellationReason.Caller;

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
