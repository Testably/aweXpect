using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.Sources;
using aweXpect.Core.TimeSystem;
using aweXpect.Customization;

namespace aweXpect.Core;

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
	internal override async ValueTask<ConstraintResult> IsMet(Node rootNode,
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
		// An outer timeout that is not shorter than the retry budget does not cancel the attempts, so that the last
		// attempt at the end of the budget still decides. A budget of zero makes a single evaluation that only the
		// outer timeout bounds.
		EvaluationCancellation cancellation = new(
			timeout < retryTimeout || retryTimeout == TimeSpan.Zero ? timeout : null, cancellationToken);
		context.Cancellation = cancellation;
		try
		{
			ConstraintResult result =
				await IsMetRepeatedly(subject, rootNode, context, retryTimeout, cancellation);
			if (result.Outcome == Outcome.Undecided && cancellation.Timeout is { } cancellationTimeout &&
			    cancellation.Reason == CancellationReason.Timeout)
			{
				return new ConstraintResult.FromException(result,
					ExpectationBuilder<TValue>.CreateTimeoutException(cancellationTimeout,
						new OperationCanceledException(cancellation.Token)),
					DefaultCurrentSubject, cancellationTimeout);
			}

			return result;
		}
		finally
		{
			cancellation.Release();
		}
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
		EvaluationCancellation cancellation)
	{
		TimeSpan interval = _interval ?? Customize.aweXpect.Settings().DefaultCheckInterval.Get();
		EvaluationContext.EvaluationContext currentContext = context;
		CancellationToken cancellationToken = cancellation.Token;
		using Polling polling = Polling.Start(Stopwatch.GetTimestamp(), retryTimeout, interval, cancellation);

		bool isLastAttempt = false;
		bool wasChecked = false;
		while (true)
		{
			(TValue? data, Exception? failure, TimeSpan? exceededTimeout, NullSubjectKind nullKind) =
				await EvaluateSubject(subject, retryTimeout, polling, interval, cancellation);
			bool hasContexts = HasContexts(failure, nullKind, ref wasChecked);

			(ConstraintResult? result, failure) =
				await CheckAttempt(rootNode, data, failure, nullKind, currentContext, cancellationToken);
			if (result?.Outcome == Outcome.Success)
			{
				return result;
			}

			bool isCanceled = failure is OperationCanceledException && cancellationToken.IsCancellationRequested;
			if (!isCanceled && (isLastAttempt || exceededTimeout is not null || polling.Remaining <= TimeSpan.Zero))
			{
				result ??= await rootNode.IsMetBy(data, EvaluationContext.ExpectationTextEvaluationContext.For(currentContext),
					System.Threading.CancellationToken.None);
				return AppendTimeout(WithFailureCause(result, failure, hasContexts, exceededTimeout), retryTimeout);
			}

			if (cancellationToken.IsCancellationRequested)
			{
				result ??= await rootNode.IsMetBy(data, EvaluationContext.ExpectationTextEvaluationContext.For(currentContext),
					System.Threading.CancellationToken.None);
				return AppendTimeout(new ConstraintResult.FromCancellation(WithFailureCause(result, failure, hasContexts)),
					retryTimeout);
			}

			// After a cancellation that decides nothing, one more attempt is made with the canceled token, so that
			// it is reported as canceled unless that attempt meets the expectations.
			isLastAttempt = await polling.WaitForNextCheck() is PollStep.LastCheck or PollStep.Elapsed;
			currentContext = await context.StartAttempt();
			ResetOtherExceptions();
		}
	}

	/// <summary>
	///     Whether the contexts of the constraints describe this attempt.
	/// </summary>
	/// <remarks>
	///     The constraints keep what an earlier checked attempt saw, which does not describe a subject that failed.
	/// </remarks>
	private static bool HasContexts(Exception? failure, NullSubjectKind nullKind, ref bool wasChecked)
	{
		bool hasContexts = failure is null || !wasChecked;
		wasChecked |= failure is null && nullKind == NullSubjectKind.None;
		return hasContexts;
	}

	/// <summary>
	///     Evaluates the <paramref name="subject" /> for one attempt, which <see cref="GetAttemptLimit" /> bounds.
	/// </summary>
	private async Task<(TValue? Data, Exception? Failure, TimeSpan? ExceededTimeout, NullSubjectKind NullKind)>
		EvaluateSubject(
			Func<CancellationToken, Task<TValue>> subject,
			TimeSpan retryTimeout,
			Polling polling,
			TimeSpan interval,
			EvaluationCancellation cancellation)
	{
		CancellationToken cancellationToken = cancellation.Token;
		TimeSpan? limit = GetAttemptLimit(retryTimeout, polling.Remaining, interval);
		using CancellationTokenSource? attemptCts = CreateAttemptCancellation(limit, cancellationToken);
		CancellationToken attemptToken = attemptCts?.Token ?? cancellationToken;
		long startTimestamp = Stopwatch.GetTimestamp();
		Task<TValue>? task = null;
		try
		{
			task = subject(attemptToken);
			if (task is null)
			{
				return (default, null, null, NullSubjectKind.NullTaskReturned);
			}

			TValue data = await task.AbandonOnCancellation(attemptToken);
			Customize.aweXpect.TraceWriter?.WriteMessage($"Checking expectation for {Subject} {data}");
			if (GetExceededTimeout(retryTimeout, limit, startTimestamp, polling, cancellation) is { } exceededTimeout)
			{
				return (default, ExpectationBuilder<TValue>.CreateTimeoutException(exceededTimeout,
					new OperationCanceledException(attemptToken)), exceededTimeout, NullSubjectKind.None);
			}

			return (data, null, null, NullSubjectKind.None);
		}
		catch (Exception exception)
		{
			Customize.aweXpect.TraceWriter?.WriteMessage(
				$"Checking expectation for {Subject} threw an exception");
			AddOtherExceptions(task?.GetOtherExceptions(exception));
			TimeSpan? exceededTimeout;
			if (exception is OperationCanceledException && attemptToken.IsCancellationRequested)
			{
				exceededTimeout = cancellationToken.IsCancellationRequested ? null : retryTimeout;
			}
			else
			{
				exceededTimeout = GetExceededTimeout(retryTimeout, limit, startTimestamp, polling, cancellation);
			}

			return (default, exceededTimeout is null
				? exception
				: ExpectationBuilder<TValue>.CreateTimeoutException(exceededTimeout.Value, exception),
				exceededTimeout, NullSubjectKind.None);
		}
	}

	/// <summary>
	///     Returns the timeout that an attempt, which finished without being abandoned, exceeded: the effective timeout of
	///     the evaluation, when it is tighter than the <paramref name="retryTimeout" /> and elapsed, or the
	///     <paramref name="retryTimeout" />, when the attempt took longer than its <paramref name="limit" />.
	/// </summary>
	/// <remarks>
	///     A synchronous subject cannot be interrupted, so it also exceeded the timeout when it finished after the timeout
	///     elapsed. The measured durations decide, as the timers can fire late when the thread pool is busy. Without a
	///     <paramref name="limit" />, e.g. for <c>Within(TimeSpan.Zero)</c>, only the effective timeout bounds it.
	/// </remarks>
	private static TimeSpan? GetExceededTimeout(TimeSpan retryTimeout,
		TimeSpan? limit,
		long startTimestamp,
		Polling polling,
		EvaluationCancellation cancellation)
	{
		if (cancellation.Timeout is { } timeout &&
		    (cancellation.Reason == CancellationReason.Timeout || polling.Elapsed >= timeout))
		{
			return timeout;
		}

		if (limit > TimeSpan.Zero && Polling.GetElapsedTime(startTimestamp) >= limit)
		{
			return retryTimeout;
		}

		return null;
	}

	/// <summary>
	///     Checks the <paramref name="data" /> of an attempt without a <paramref name="failure" /> against the
	///     <paramref name="rootNode" />, and turns a cancellation during the check into the failure of the attempt.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> task of the subject fails the attempt without a check, as there is no value.
	/// </remarks>
	private static async Task<(ConstraintResult? Result, Exception? Failure)> CheckAttempt(Node rootNode,
		TValue? data,
		Exception? failure,
		NullSubjectKind nullKind,
		EvaluationContext.EvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (failure is not null)
		{
			return (null, failure);
		}

		if (nullKind == NullSubjectKind.NullTaskReturned)
		{
			ConstraintResult expectation = await rootNode.IsMetBy(data,
				EvaluationContext.ExpectationTextEvaluationContext.For(context), System.Threading.CancellationToken.None);
			return (expectation.Fail("it returned <null> instead of a task", data), null);
		}

		try
		{
			return (await rootNode.IsMetBy(data, context, cancellationToken), null);
		}
		catch (OperationCanceledException exception) when (cancellationToken.IsCancellationRequested)
		{
			return (null, exception);
		}
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
	///     <see langword="null" /> for an unlimited budget, which only the cancellation of the evaluation bounds.
	/// </summary>
	/// <remarks>
	///     The last attempt is made when the budget is used up, so without the minimum it would be abandoned before an
	///     asynchronous subject had a chance to finish. A <paramref name="retryTimeout" /> of zero, i.e.
	///     <c>Within(TimeSpan.Zero)</c>, makes a single evaluation that no subject could finish within, so it does not
	///     bound it either.
	/// </remarks>
	private static TimeSpan? GetAttemptLimit(TimeSpan retryTimeout, TimeSpan remaining, TimeSpan interval)
	{
		if (retryTimeout == TimeSpan.MaxValue || retryTimeout == TimeSpan.Zero)
		{
			return null;
		}

		TimeSpan minimum = interval < retryTimeout ? interval : retryTimeout;
		return remaining > minimum ? remaining : minimum;
	}

	private static CancellationTokenSource? CreateAttemptCancellation(TimeSpan? limit,
		CancellationToken cancellationToken)
	{
		if (limit is null)
		{
			return null;
		}

		CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter(limit.Value.ToTimerTimeout());
		return cts;
	}

	private static ConstraintResult WithFailureCause(ConstraintResult result, Exception? failure,
		bool hasContexts, TimeSpan? exceededTimeout = null)
		=> failure is null
			? result
			: new ConstraintResult.FromException(result, failure, DefaultCurrentSubject, exceededTimeout,
				hasContexts);
}
