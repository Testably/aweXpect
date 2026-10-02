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
}
