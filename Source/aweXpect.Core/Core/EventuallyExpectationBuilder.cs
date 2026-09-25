using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.TimeSystem;
using aweXpect.Customization;

namespace aweXpect.Core;

/// <summary>
///     The retry constants of <see cref="EventuallyExpectationBuilder{TValue}" />.
/// </summary>
/// <remarks>
///     They do not depend on the value type, so they are kept outside of the generic type to have a single instance
///     instead of one per closed constructed type.
/// </remarks>
internal static class EventuallyExpectationBuilder
{
	/// <summary>
	///     How close to the end of the retry budget a cancellation still counts as the budget having elapsed, and
	///     how much of the budget may remain after a wait for that wait to still be the last one.
	/// </summary>
	/// <remarks>
	///     <see cref="Task.Delay(TimeSpan, CancellationToken)" /> truncates to whole milliseconds and its timer does
	///     not share the clock of the stopwatch that measures the retry budget, so a wait that consumed the whole
	///     budget can be canceled a fraction of a millisecond before the stopwatch agrees.
	/// </remarks>
	public static readonly TimeSpan CancellationTolerance = TimeSpan.FromMilliseconds(2);

	/// <summary>
	///     The largest interval that <see cref="Task.Delay(TimeSpan, CancellationToken)" /> accepts.
	/// </summary>
	public static readonly TimeSpan MaximumInterval = TimeSpan.FromMilliseconds(int.MaxValue);
}

/// <summary>
///     An <see cref="ExpectationBuilder" /> that repeatedly re-evaluates the <paramref name="subject" />
///     until the expectations are met or the timeout expires.
/// </summary>
internal class EventuallyExpectationBuilder<TValue>(
	Func<CancellationToken, Task<TValue>>? subject,
	string subjectExpression)
	: ExpectationBuilder(subjectExpression)
{
	private TimeSpan? _interval;
	private TimeSpan? _retryTimeout;

	/// <summary>
	///     Sets the <paramref name="interval" /> in which the subject is re-evaluated.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="interval" /> is not positive.</exception>
	/// <exception cref="InvalidOperationException">An interval is already set.</exception>
	public void CheckEvery(TimeSpan interval)
	{
		if (interval <= TimeSpan.Zero)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(interval), "The interval must be positive."));
		}

		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_interval is not null, nameof(CheckEvery));
		_interval = interval;
	}

	/// <summary>
	///     Sets the <paramref name="timeout" /> until the expectations must be met.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A timeout is already set.</exception>
	public void Within(TimeSpan timeout)
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_retryTimeout is not null, nameof(Within));
		_retryTimeout = timeout;
	}

	/// <inheritdoc />
	internal override async Task<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		ConstraintResult result = await IsMetEventually(rootNode, context, timeout, cancellationToken);
		return result.PrependExpectationText(sb => sb.Append("eventually "));
	}

	private async Task<ConstraintResult> IsMetEventually(Node rootNode,
		EvaluationContext.EvaluationContext context,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		if (subject is null)
		{
			ConstraintResult missingSubject = await rootNode.IsMetBy(default(TValue),
				EvaluationContext.ExpectationTextEvaluationContext.For(context), cancellationToken);
			return missingSubject.Fail("it was <null>", default(TValue));
		}

		TimeSpan retryTimeout = GetRetryTimeout();
		TimeSpan? cancellationTimeout = timeout < retryTimeout ? timeout : null;
		if (cancellationTimeout is null)
		{
			return await IsMetRepeatedly(subject, rootNode, context, retryTimeout, cancellationToken);
		}

		using CancellationTokenSource cancellationCts =
			CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cancellationCts.CancelAfter(cancellationTimeout.Value.ToTimerTimeout());
		ConstraintResult result =
			await IsMetRepeatedly(subject, rootNode, context, retryTimeout, cancellationCts.Token);
		if (result.Outcome == Outcome.Undecided && cancellationCts.IsCancellationRequested &&
		    !cancellationToken.IsCancellationRequested)
		{
			return AppendTimeout(new ConstraintResult.FromException(result,
					ExpectationBuilder<TValue>.CreateTimeoutException(cancellationTimeout.Value,
						new OperationCanceledException(cancellationCts.Token)),
					DefaultCurrentSubject, cancellationTimeout.Value),
				retryTimeout);
		}

		return result;
	}

	private TimeSpan GetRetryTimeout()
	{
		TimeSpan retryTimeout = _retryTimeout ?? Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get();
		if (retryTimeout == System.Threading.Timeout.InfiniteTimeSpan)
		{
			return TimeSpan.MaxValue;
		}

		return retryTimeout < TimeSpan.Zero ? TimeSpan.Zero : retryTimeout;
	}

	private async Task<ConstraintResult> IsMetRepeatedly(Func<CancellationToken, Task<TValue>> subject,
		Node rootNode,
		EvaluationContext.EvaluationContext context,
		TimeSpan retryTimeout,
		CancellationToken cancellationToken)
	{
		TimeSpan interval = _interval ?? Customize.aweXpect.Settings().DefaultCheckInterval.Get();
		List<ResultContext> initialContexts = new(GetContexts());
		EvaluationContext.EvaluationContext currentContext = context;
		Stopwatch stopwatch = new();
		stopwatch.Start();

		TimeSpan? cancelledAt = null;
		TaskCompletionSource<bool> cancellation = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using CancellationTokenRegistration registration = cancellationToken.Register(() =>
		{
			// The time has to be recorded before the wait is released, so that the attempt that continues after
			// the cancellation always observes it.
			cancelledAt ??= stopwatch.Elapsed;
			cancellation.TrySetResult(true);
		});
		TimeSpan Elapsed() => cancelledAt ?? stopwatch.Elapsed;

		bool isLastAttempt = false;
		while (true)
		{
			(TValue? data, Exception? failure, bool hasTimedOut) = await EvaluateSubject(subject, retryTimeout,
				retryTimeout - Elapsed(), interval, cancellationToken);

			ConstraintResult? result = null;
			if (failure is null)
			{
				try
				{
					result = await rootNode.IsMetBy(data, currentContext, cancellationToken);
				}
				catch (OperationCanceledException exception) when (cancellationToken.IsCancellationRequested)
				{
					failure = exception;
				}

				if (result?.Outcome == Outcome.Success)
				{
					return result;
				}
			}

			TimeSpan remaining = retryTimeout - Elapsed();
			bool isCanceled = failure is OperationCanceledException && cancellationToken.IsCancellationRequested;
			if (!isCanceled && (isLastAttempt || hasTimedOut || remaining <= TimeSpan.Zero))
			{
				result ??= await rootNode.IsMetBy(data, EvaluationContext.ExpectationTextEvaluationContext.For(currentContext),
					System.Threading.CancellationToken.None);
				return AppendTimeout(WithFailureCause(result, failure, hasTimedOut ? retryTimeout : null),
					retryTimeout);
			}

			if (cancellationToken.IsCancellationRequested)
			{
				result ??= await rootNode.IsMetBy(data, EvaluationContext.ExpectationTextEvaluationContext.For(currentContext),
					System.Threading.CancellationToken.None);
				return new ConstraintResult.FromCancellation(WithFailureCause(result, failure));
			}

			TimeSpan wait = NextInterval(interval, remaining);
			// The timer of the wait can complete a fraction of a millisecond before the stopwatch agrees, so an
			// attempt that would only leave a sliver of the budget is the last one; otherwise the sliver becomes an
			// additional wait and evaluation right at the deadline.
			isLastAttempt = remaining - wait < EventuallyExpectationBuilder.CancellationTolerance;
			if (await IsCancelledDuring(wait, cancellation.Task))
			{
				isLastAttempt = retryTimeout - Elapsed() < EventuallyExpectationBuilder.CancellationTolerance;
			}

			currentContext = new EvaluationContext.EvaluationContext();
			RestoreContexts(initialContexts);
		}
	}

	/// <summary>
	///     Evaluates the <paramref name="subject" /> for one attempt, which
	///     <see cref="CreateAttemptCancellation" /> bounds.
	/// </summary>
	private async Task<(TValue? Data, Exception? Failure, bool HasTimedOut)> EvaluateSubject(
		Func<CancellationToken, Task<TValue>> subject,
		TimeSpan retryTimeout,
		TimeSpan remaining,
		TimeSpan interval,
		CancellationToken cancellationToken)
	{
		using CancellationTokenSource? attemptCts =
			CreateAttemptCancellation(retryTimeout, remaining, interval, cancellationToken);
		CancellationToken attemptToken = attemptCts?.Token ?? cancellationToken;
		try
		{
			TValue data = await subject(attemptToken).AbandonOnCancellation(attemptToken);
			Customize.aweXpect.TraceWriter.Value?.WriteMessage($"Checking expectation for {Subject} {data}");
			return (data, null, false);
		}
		catch (Exception exception)
		{
			bool hasTimedOut = exception is OperationCanceledException &&
			                   attemptCts?.IsCancellationRequested == true &&
			                   !cancellationToken.IsCancellationRequested;
			Customize.aweXpect.TraceWriter.Value?.WriteMessage(
				$"Checking expectation for {Subject} threw an exception");
			return (default, hasTimedOut
				? ExpectationBuilder<TValue>.CreateTimeoutException(retryTimeout, exception)
				: exception, hasTimedOut);
		}
	}

	/// <summary>
	///     Waits for the <paramref name="wait" /> and returns whether the <paramref name="cancellation" /> cut it short.
	/// </summary>
	/// <remarks>
	///     The wait is not canceled by the token itself: <see cref="Task.Delay(TimeSpan, CancellationToken)" /> would
	///     register its own callback on it and the cancellation callbacks run in reverse order, so the wait could
	///     continue before the callback in <see cref="IsMetRepeatedly" /> recorded when the cancellation was requested.
	/// </remarks>
	private static async Task<bool> IsCancelledDuring(TimeSpan wait, Task cancellation)
	{
		using CancellationTokenSource waitCts = new();
		Task delay = Task.Delay(wait, waitCts.Token);
		if (await Task.WhenAny(delay, cancellation) == delay)
		{
			return false;
		}

		waitCts.Cancel();
		return true;
	}

	/// <summary>
	///     Waits at most until the retry budget (<paramref name="remaining" />) is used up. A non-positive
	///     <paramref name="interval" /> re-evaluates the subject as fast as possible.
	/// </summary>
	/// <remarks>
	///     The result is capped at <see cref="EventuallyExpectationBuilder.MaximumInterval" />, because an unlimited
	///     retry budget does not limit the interval and
	///     <see cref="Task.Delay(TimeSpan, CancellationToken)" /> rejects larger values.
	/// </remarks>
	private static TimeSpan NextInterval(TimeSpan interval, TimeSpan remaining)
	{
		if (interval <= TimeSpan.Zero)
		{
			return TimeSpan.Zero;
		}

		TimeSpan maximum = EventuallyExpectationBuilder.MaximumInterval;
		TimeSpan next = interval < remaining ? interval : remaining;
		return next < maximum ? next : maximum;
	}

	/// <summary>
	///     Appends the retry budget to the expectation of the failed <paramref name="result" />. An unlimited budget
	///     is omitted, because it does not add any information to the failure message.
	/// </summary>
	private static ConstraintResult AppendTimeout(ConstraintResult result, TimeSpan timeout)
	{
		if (timeout == TimeSpan.MaxValue)
		{
			return result;
		}

		return result.AppendExpectationText(sb => sb.Append(" within ").Append(Formatter.Format(timeout)));
	}

	/// <summary>
	///     Bounds an attempt by the <paramref name="remaining" /> retry budget, but gives it at least one
	///     <paramref name="interval" /> (or the whole <paramref name="retryTimeout" />, if shorter) to finish, or returns
	///     <see langword="null" /> for an unlimited budget, which only the <paramref name="cancellationToken" /> bounds.
	/// </summary>
	/// <remarks>
	///     The last attempt is made when the budget is used up, so without the minimum it would be abandoned before an
	///     asynchronous subject had a chance to finish.
	/// </remarks>
	private static CancellationTokenSource? CreateAttemptCancellation(TimeSpan retryTimeout,
		TimeSpan remaining,
		TimeSpan interval,
		CancellationToken cancellationToken)
	{
		if (retryTimeout == TimeSpan.MaxValue)
		{
			return null;
		}

		TimeSpan minimum = interval < retryTimeout ? interval : retryTimeout;
		TimeSpan limit = remaining > minimum ? remaining : minimum;
		CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter((limit > TimeSpan.Zero ? limit : TimeSpan.Zero).ToTimerTimeout());
		return cts;
	}

	private static ConstraintResult WithFailureCause(ConstraintResult result, Exception? failure,
		TimeSpan? exceededTimeout = null)
		=> failure is null
			? result
			: new ConstraintResult.FromException(result, failure, DefaultCurrentSubject, exceededTimeout);

	private void RestoreContexts(List<ResultContext> initialContexts)
		=> UpdateContexts(contexts =>
		{
			contexts.Clear();
			foreach (ResultContext resultContext in initialContexts)
			{
				contexts.Add(resultContext);
			}
		});
}
