using System;
using System.Threading;
using aweXpect.Core.Helpers;
using aweXpect.Core.Internal;
using aweXpect.Core.TimeSystem;

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

	/// <summary>
	///     How long after the start of the evaluation a wait can start and still count as elapsed when the
	///     <see cref="Timeout" /> ends it.
	/// </summary>
	/// <remarks>
	///     It covers what lies between the start of the timer of the timeout and the start of the first wait: evaluating
	///     the subject and reaching the constraint, which takes a few milliseconds even when that code is compiled on its
	///     first use or the thread is preempted. A wait of an earlier constraint takes longer, and the timeout that
	///     remains after it no longer covers the wait.
	/// </remarks>
	internal static readonly TimeSpan StartSlack = TimeSpan.FromMilliseconds(50);

	private readonly CancellationToken _callerToken;
	private readonly TimeSpan? _outerTimeout;
	private readonly long _startTimestamp;
	private readonly CancellationTokenSource? _timeoutCts;
	private readonly ITimeSystem _timeSystem;

	/// <param name="timeout">The timeout that cancels the evaluation.</param>
	/// <param name="callerToken">The token with which the caller cancels the evaluation.</param>
	/// <param name="outerTimeout">
	///     The timeout of the expectation, when it does not cancel the evaluation and therefore is not the
	///     <paramref name="timeout" />: it still limits what is awaited after the evaluation, see
	///     <see cref="ForRemainingTimeout" />.
	/// </param>
	/// <param name="timeSystem">
	///     The time system of the evaluation, on which the timeouts elapse; the real one when it is
	///     <see langword="null" />.
	/// </param>
	internal EvaluationCancellation(TimeSpan? timeout, CancellationToken callerToken, TimeSpan? outerTimeout = null,
		ITimeSystem? timeSystem = null)
	{
		Timeout = timeout;
		_callerToken = callerToken;
		_outerTimeout = outerTimeout ?? timeout;
		_timeSystem = timeSystem ?? RealTimeSystem.Instance;
		Token = callerToken;
		if (_outerTimeout is not null)
		{
			_startTimestamp = _timeSystem.GetTimestamp();
		}

		if (timeout is not null)
		{
			_timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(callerToken);
			_timeSystem.CancelAfter(_timeoutCts, timeout.Value.ToTimerTimeout());
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
	///     Creates the cancellation of an evaluation with the <paramref name="timeout" /> and the
	///     <paramref name="callerToken" />, whose <paramref name="timeout" /> elapses on the
	///     <paramref name="timeSystem" />, or on the real one when it is <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     Most evaluations have neither, so they share <see cref="None" />, which holds no timer to release.
	/// </remarks>
	internal static EvaluationCancellation Create(TimeSpan? timeout, CancellationToken callerToken,
		ITimeSystem? timeSystem = null)
		=> timeout is null && !callerToken.CanBeCanceled
			? None
			: new EvaluationCancellation(timeout, callerToken, null, timeSystem);

	/// <summary>
	///     Whether a wait of at most <paramref name="waitTimeout" /> that a cancellation ended after
	///     <paramref name="waited" /> counts as elapsed, so that the result at that time decides instead of the
	///     cancellation.
	/// </summary>
	/// <remarks>
	///     This is the case, when the cancellation came at the end of the wait, or when the <see cref="Timeout" /> elapsed,
	///     is not shorter than the <paramref name="waitTimeout" /> and the wait started with the evaluation: its timer
	///     started before the wait, so it can expire slightly before the wait does. A cancellation by the caller, a
	///     shorter <see cref="Timeout" />, a <see cref="Timeout" /> that elapsed during a wait which started later in the
	///     evaluation (e.g. the wait of a second constraint) and every cancellation of an infinite
	///     <paramref name="waitTimeout" /> end the wait without a decision.
	/// </remarks>
	public bool HasWaitElapsed(TimeSpan waitTimeout, TimeSpan waited)
	{
		if (waitTimeout == System.Threading.Timeout.InfiniteTimeSpan || waitTimeout == TimeSpan.MaxValue)
		{
			return false;
		}

		if (waitTimeout - waited < Tolerance)
		{
			return true;
		}

		// The timeout elapses that long after the start of the evaluation, so the part of it that the wait did not
		// cover is the time by which the wait started after the evaluation.
		return Reason == CancellationReason.Timeout && Timeout >= waitTimeout && Timeout - waited < StartSlack;
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
	///     Returns the cancellation for what is still awaited after the evaluation released the timer of the
	///     <see cref="Timeout" />: its token is canceled by the caller or when the rest of the timeout of the
	///     expectation elapses, which is measured from the start of the evaluation.
	/// </summary>
	/// <remarks>
	///     Without a timeout it is this instance, as the token of the caller needs no timer. The returned cancellation
	///     must be released as well.
	/// </remarks>
	internal EvaluationCancellation ForRemainingTimeout()
	{
		if (_outerTimeout is not { } timeout)
		{
			return this;
		}

		TimeSpan remaining = timeout - _timeSystem.GetElapsedTime(_startTimestamp);
		return new EvaluationCancellation(remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero, _callerToken, null,
			_timeSystem);
	}

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
