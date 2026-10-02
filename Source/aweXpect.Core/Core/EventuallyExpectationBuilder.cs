using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
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
		// An outer timeout that is not shorter than the retry budget does not cancel the attempts, so that the last
		// attempt at the end of the budget still decides.
		EvaluationCancellation cancellation = new(timeout < retryTimeout ? timeout : null, cancellationToken);
		context.Cancellation = cancellation;
		try
		{
			ConstraintResult result =
				await IsMetRepeatedly(subject, rootNode, context, retryTimeout, cancellation);
			if (result.Outcome == Outcome.Undecided && cancellation.Timeout is { } cancellationTimeout &&
			    cancellation.IsTimeoutElapsed)
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
		List<ResultContext> initialContexts = new(GetContexts());
		EvaluationContext.EvaluationContext currentContext = context;
		CancellationToken cancellationToken = cancellation.Token;
		using Polling polling = Polling.Start(Stopwatch.GetTimestamp(), retryTimeout, interval, cancellation);

		bool isLastAttempt = false;
		while (true)
		{
			(TValue? data, Exception? failure, bool hasTimedOut, bool isNullTask) = await EvaluateSubject(subject,
				retryTimeout, polling.Remaining, interval, cancellationToken);

			(ConstraintResult? result, failure) =
				await CheckAttempt(rootNode, data, failure, isNullTask, currentContext, cancellationToken);
			if (result?.Outcome == Outcome.Success)
			{
				return result;
			}

			bool isCanceled = failure is OperationCanceledException && cancellationToken.IsCancellationRequested;
			if (!isCanceled && (isLastAttempt || hasTimedOut || polling.Remaining <= TimeSpan.Zero))
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
				return AppendTimeout(new ConstraintResult.FromCancellation(WithFailureCause(result, failure)),
					retryTimeout);
			}

			// After a cancellation that decides nothing, one more attempt is made with the canceled token, so that
			// it is reported as canceled unless that attempt meets the expectations.
			isLastAttempt = await polling.WaitForNextCheck() is PollStep.LastCheck or PollStep.Elapsed;
			currentContext = await context.StartAttempt();
			RestoreContexts(initialContexts);
		}
	}
	/// <summary>
	///     Evaluates the <paramref name="subject" /> for one attempt, which
	///     <see cref="CreateAttemptCancellation" /> bounds.
	/// </summary>
	private async Task<(TValue? Data, Exception? Failure, bool HasTimedOut, bool IsNullTask)> EvaluateSubject(
		Func<CancellationToken, Task<TValue>> subject,
		TimeSpan retryTimeout,
		TimeSpan remaining,
		TimeSpan interval,
		CancellationToken cancellationToken)
	{
		using CancellationTokenSource? attemptCts =
			CreateAttemptCancellation(retryTimeout, remaining, interval, cancellationToken);
		CancellationToken attemptToken = attemptCts?.Token ?? cancellationToken;
		Task<TValue>? task = null;
		try
		{
			task = subject(attemptToken);
			if (task is null)
			{
				return (default, null, false, true);
			}

			TValue data = await task.AbandonOnCancellation(attemptToken);
			Customize.aweXpect.TraceWriter?.WriteMessage($"Checking expectation for {Subject} {data}");
			return (data, null, false, false);
		}
		catch (Exception exception)
		{
			bool hasTimedOut = exception is OperationCanceledException &&
			                   attemptCts?.IsCancellationRequested == true &&
			                   !cancellationToken.IsCancellationRequested;
			Customize.aweXpect.TraceWriter?.WriteMessage(
				$"Checking expectation for {Subject} threw an exception");
			AddOtherExceptions(task?.GetOtherExceptions(exception));
			return (default, hasTimedOut
				? ExpectationBuilder<TValue>.CreateTimeoutException(retryTimeout, exception)
				: exception, hasTimedOut, false);
		}
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
		bool isNullTask,
		EvaluationContext.EvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (failure is not null)
		{
			return (null, failure);
		}

		if (isNullTask)
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
