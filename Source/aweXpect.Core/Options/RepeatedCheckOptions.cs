using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Nodes;
using aweXpect.Core.TimeSystem;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Options;

/// <summary>
///     The options for a <see cref="RepeatedCheckResult{TType,TThat}" />.
/// </summary>
public class RepeatedCheckOptions
{
	private TimeSpan? _interval;
	private bool _isTimeoutSpecified;

	/// <summary>
	///     The interval in which the condition should be checked.
	/// </summary>
	/// <remarks>
	///     Defaults to the current <c>Customize.aweXpect.Settings().DefaultCheckInterval</c> if not specified.
	/// </remarks>
	public TimeSpan Interval => _interval ?? Customize.aweXpect.Settings().DefaultCheckInterval.Get();

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

		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_interval is not null, nameof(CheckEvery));
		_interval = interval;
	}

	/// <summary>
	///     Makes the <paramref name="check" /> and repeats it in the <see cref="Interval" /> until it succeeds or the
	///     <see cref="Timeout" /> has elapsed.
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
	///             succeeds or the evaluation is canceled.
	///         </item>
	///         <item>
	///             The cancellation of the evaluation is only observed while waiting for the next check, so the first
	///             check is made even when the evaluation is already canceled.
	///         </item>
	///         <item>
	///             A cancellation during a wait lets one last check decide, when it counts as the
	///             <see cref="Timeout" /> having elapsed (see
	///             <see cref="EvaluationCancellation.HasWaitElapsed(TimeSpan, TimeSpan)" />): when it came at the
	///             <see cref="Timeout" />, or when the effective timeout of the evaluation is not shorter than the
	///             <see cref="Timeout" />. Any other cancellation, e.g. by the caller or by a shorter timeout, ends the
	///             checks with <see cref="Outcome.Undecided" />.
	///         </item>
	///         <item>
	///             The first check receives the <paramref name="context" />, so that it shares the materialized collections
	///             with the other expectations of the evaluation. During an evaluation, every further check receives a new
	///             context of its own, so that it reads the collections (e.g. with
	///             <see cref="EvaluationContextExtensions.UseMaterializedEnumerable{TItem}(IEvaluationContext, System.Collections.Generic.IEnumerable{TItem})" />)
	///             again instead of replaying the items that a previous check read.
	///         </item>
	///         <item>
	///             An exception of code of the caller that the <paramref name="check" /> called through
	///             <see cref="UserCode" /> counts as not met when the check is repeated, so the check is made again. When
	///             the last check threw it, it is thrown again, which fails the expectation with the exception. Any other
	///             exception thrown by the <paramref name="check" /> is not caught.
	///         </item>
	///     </list>
	/// </remarks>
	/// <param name="check">
	///     The check, which receives the evaluation context for the check and returns whether the expectation is met. For a
	///     negated constraint, it returns <see langword="true" /> when the condition is not met.
	/// </param>
	/// <param name="context">The evaluation context that the constraint received.</param>
	/// <returns>
	///     <see cref="Outcome.Success" /> when a check returned <see langword="true" />, <see cref="Outcome.Failure" /> when
	///     the last check returned <see langword="false" />, and <see cref="Outcome.Undecided" /> when the evaluation was
	///     canceled before the <see cref="Timeout" />, by the caller or by a shorter timeout.
	/// </returns>
	public async ValueTask<Outcome> CheckRepeatedly(Func<IEvaluationContext, ValueTask<bool>> check,
		IEvaluationContext context)
	{
		if (!IsRepeated)
		{
			return await check(context) ? Outcome.Success : Outcome.Failure;
		}

		long startTimestamp = GetTimeSystem(context).GetTimestamp();
		(bool isMet, UserCodeException? exception) = await Check(check, context);
		if (isMet)
		{
			return Outcome.Success;
		}

		using Polling polling =
			Polling.Start(GetTimeSystem(context), startTimestamp, Timeout, Interval, context.Cancellation);
		EvaluationContext? checkContext = null;
		while (true)
		{
			switch (await polling.WaitForNextCheck())
			{
				case PollStep.Elapsed:
					if (exception is not null)
					{
						ExceptionDispatchInfo.Capture(exception).Throw();
					}

					return Outcome.Failure;
				case PollStep.Canceled:
					return Outcome.Undecided;
			}

			if (context is EvaluationContext evaluationContext)
			{
				checkContext = await evaluationContext.StartCheck(checkContext);
			}

			(isMet, exception) = await Check(check, checkContext ?? context);
			if (isMet)
			{
				return Outcome.Success;
			}
		}
	}

	/// <summary>
	///     The time system of the evaluation, or the real one for a <paramref name="context" /> of another implementation.
	/// </summary>
	private static ITimeSystem GetTimeSystem(IEvaluationContext context)
		=> context is EvaluationContext evaluationContext
			? evaluationContext.TimeSystem
			: RealTimeSystem.Instance;

	/// <remarks>
	///     A cancellation of the evaluation still aborts it, like everywhere else.
	/// </remarks>
	private static async ValueTask<(bool IsMet, UserCodeException? Exception)> Check(
		Func<IEvaluationContext, ValueTask<bool>> check, IEvaluationContext context)
	{
		try
		{
			return (await check(context), null);
		}
		catch (UserCodeException exception) when (!MemberExceptionResult.IsCancellationOf(exception.Exception,
			                                          context.Cancellation.Token))
		{
			return (false, exception);
		}
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
