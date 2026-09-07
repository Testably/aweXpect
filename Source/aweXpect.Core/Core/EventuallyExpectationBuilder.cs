using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
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
	///     budget can be cancelled a fraction of a millisecond before the stopwatch agrees.
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
	/// <inheritdoc />
	internal override async Task<ConstraintResult> IsMet(Node rootNode,
		EvaluationContext.EvaluationContext context,
		ITimeSystem timeSystem,
		TimeSpan? timeout,
		CancellationToken cancellationToken)
	{
		if (subject is null)
		{
			ConstraintResult missingSubject = await rootNode.IsMetBy(default(TValue), context, cancellationToken);
			return missingSubject.Fail("it was <null>", default(TValue));
		}

		TimeSpan retryTimeout = GetRetryTimeout();
		TimeSpan? cancellationTimeout = Timeout is null ? timeout : null;
		if (cancellationTimeout is null)
		{
			return await IsMetRepeatedly(subject, rootNode, context, retryTimeout, cancellationToken);
		}

		using CancellationTokenSource cancellationCts =
			CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cancellationCts.CancelAfter(cancellationTimeout.Value);
		return await IsMetRepeatedly(subject, rootNode, context, retryTimeout, cancellationCts.Token);
	}

	private TimeSpan GetRetryTimeout()
	{
		TimeSpan retryTimeout = Timeout ?? Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get();
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
		TimeSpan interval = Customize.aweXpect.Settings().DefaultCheckInterval.Get();
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
			Exception? failure = null;
			TValue? data = default;
			try
			{
				data = await subject(cancellationToken);
				Customize.aweXpect.TraceWriter.Value?.WriteMessage($"Checking expectation for {Subject} {data}");
			}
			catch (Exception exception)
			{
				failure = exception;
				Customize.aweXpect.TraceWriter.Value?.WriteMessage(
					$"Checking expectation for {Subject} threw an exception");
			}

			ConstraintResult? result = null;
			if (failure is null)
			{
				result = await rootNode.IsMetBy(data, currentContext, cancellationToken);
				if (result.Outcome == Outcome.Success)
				{
					return result;
				}
			}

			TimeSpan remaining = retryTimeout - Elapsed();
			if (isLastAttempt || remaining <= TimeSpan.Zero)
			{
				result ??= await rootNode.IsMetBy(data, currentContext, System.Threading.CancellationToken.None);
				return AppendTimeout(WithFailureCause(result, failure), retryTimeout);
			}

			if (cancellationToken.IsCancellationRequested)
			{
				result ??= await rootNode.IsMetBy(data, currentContext,
					System.Threading.CancellationToken.None);
				return new UndecidedResult(WithFailureCause(result, failure));
			}

			TimeSpan wait = NextInterval(interval, remaining);
			// The timer of the wait can complete a fraction of a millisecond before the stopwatch agrees, so an
			// attempt that would only leave a sliver of the budget is the last one; otherwise the sliver becomes an
			// additional wait and evaluation right at the deadline.
			isLastAttempt = remaining - wait < EventuallyExpectationBuilder.CancellationTolerance;

			// The wait is not cancelled by the token itself: Task.Delay would register its own callback on it and
			// the cancellation callbacks run in reverse order, so the wait could continue before the callback
			// above recorded when the cancellation was requested.
			using (CancellationTokenSource waitCts = new())
			{
				Task delay = Task.Delay(wait, waitCts.Token);
				if (await Task.WhenAny(delay, cancellation.Task) != delay)
				{
					waitCts.Cancel();
					isLastAttempt = retryTimeout - Elapsed() < EventuallyExpectationBuilder.CancellationTolerance;
				}
			}

			currentContext = new EvaluationContext.EvaluationContext();
			RestoreContexts(initialContexts);
		}
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

	private ConstraintResult WithFailureCause(ConstraintResult result, Exception? failure)
		=> failure is null ? result : new ConstraintResult.FromException(result, failure, this);

	private void RestoreContexts(List<ResultContext> initialContexts)
		=> UpdateContexts(contexts =>
		{
			contexts.Clear();
			foreach (ResultContext resultContext in initialContexts)
			{
				contexts.Add(resultContext);
			}
		});

	/// <summary>
	///     A <see cref="ConstraintResult" /> for expectations that were cancelled before they could be verified.
	/// </summary>
	private sealed class UndecidedResult(ConstraintResult inner) : ConstraintResult(inner.Grammars)
	{
		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome
		{
			get => Outcome.Undecided;

			// The outcome of a cancelled expectation is always undecided, so the value is discarded.
			protected set => _ = value;
		}

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it could not be verified, because it was already cancelled");

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<T>([NotNullWhen(true)] out T? value) where T : default
			=> inner.TryGetValue(out value);

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
		{
			inner.Negate();
			return this;
		}
	}
}
