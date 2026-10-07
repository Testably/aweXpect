using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Helpers;

/// <remarks>
///     A class, so that the message resolved by one use is kept for every later use and the reason is awaited at most
///     once.
/// </remarks>
internal sealed class AsyncBecauseReason(Task<string?> reason) : IBecauseReason
{
	/// <summary>
	///     Writes the message; created once per reason instead of a closure at the start of every
	///     <see cref="ApplyTo" />.
	/// </summary>
	private Action<StringBuilder>? _appendMessage;

	private bool _isResolved;
	private string? _message;

	public async ValueTask<ConstraintResult>
		ApplyTo(ConstraintResult result)
	{
		// The reason is only needed for a failure message, so a broken reason provider must not fail a met expectation.
		if (result.Outcome == Outcome.Success)
		{
			if (!_isResolved)
			{
				ObserveExceptions(reason);
			}

			return result;
		}

		await Resolve(CancellationToken.None);
		if (_message is null)
		{
			return result;
		}

		return result.AppendExpectationText(_appendMessage ??= stringBuilder => stringBuilder.Append(_message));
	}

	/// <summary>
	///     Applies the reason to the <paramref name="result" /> of the whole expectation without awaiting it.
	/// </summary>
	/// <remarks>
	///     The reason is resolved when the evaluation in the <paramref name="context" /> fails, which a combination can
	///     still cause for a met <paramref name="result" />. The evaluation limits how long it waits for the reason.
	/// </remarks>
	public ConstraintResult ApplyPending(ConstraintResult result, EvaluationContext.EvaluationContext context)
	{
		ResolveOnFailureOf(context);
		if (_isResolved && _message is null)
		{
			return result;
		}

		return result.AppendExpectationText(_appendMessage ??= stringBuilder => stringBuilder.Append(_message));
	}

	private static string CreateMessage(string reason)
	{
		const string prefix = "because";
		string message = reason.Trim();

		return !message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			? $", {prefix} {message}"
			: $", {message}";
	}

	/// <summary>
	///     Marks a faulted <paramref name="task" /> as observed, so that a reason that is never awaited cannot surface
	///     later as an <see cref="TaskScheduler.UnobservedTaskException" />.
	/// </summary>
	private static void ObserveExceptions(Task<string?> task)
		=> task.ContinueWith(static t => _ = t.Exception, CancellationToken.None,
			TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
			TaskScheduler.Default);

	/// <summary>
	///     Applies the reason to the <paramref name="result" /> of the expectations on a member.
	/// </summary>
	/// <remarks>
	///     An outer negation or combination can still fail a met member, so the message is appended to a met
	///     <paramref name="result" /> as well, and the reason is resolved when the evaluation in the
	///     <paramref name="context" /> fails.<br />
	///     The same holds for a member that is only evaluated for its expectation text, as an outer combination can
	///     still meet the expectation.<br />
	///     A reason that is awaited right away is abandoned when the <paramref name="cancellationToken" /> of the
	///     evaluation is canceled.
	/// </remarks>
	public async ValueTask<ConstraintResult> ApplyToMember(ConstraintResult result, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (context is ExpectationTextEvaluationContext { Evaluation: { } evaluation, })
		{
			ResolveOnFailureOf(evaluation);
		}
		else if (result.Outcome != Outcome.Success || context is ExpectationTextEvaluationContext)
		{
			await Resolve(cancellationToken);
		}
		else
		{
			ResolveOnFailureOf(context);
		}

		if (_isResolved && _message is null)
		{
			return result;
		}

		return result.AppendExpectationText(_appendMessage ??= stringBuilder => stringBuilder.Append(_message));
	}

	/// <summary>
	///     Registers the reason, unless it is already resolved, to be resolved when the evaluation in the
	///     <paramref name="context" /> fails.
	/// </summary>
	public void ResolveOnFailureOf(IEvaluationContext context)
	{
		if (_isResolved)
		{
			return;
		}

		ObserveExceptions(reason);
		(context as EvaluationContext.EvaluationContext)?.ResolveOnFailure(this);
	}

	/// <summary>
	///     Awaits the reason, unless it was already resolved, and caches its message.
	/// </summary>
	/// <remarks>
	///     A reason that is still pending when the <paramref name="cancellationToken" /> is canceled is abandoned, and
	///     the message says so, because the failure of the expectation must be reported nevertheless.
	/// </remarks>
	public async Task Resolve(CancellationToken cancellationToken)
	{
		if (_isResolved)
		{
			return;
		}

		string? resolvedReason;
		try
		{
			resolvedReason = await reason.AbandonOnCancellation(cancellationToken).ConfigureAwait(false);
		}
		catch (OperationCanceledException) when (reason is { IsCanceled: false, IsFaulted: false, })
		{
			resolvedReason = "the reason was not available in time";
		}
		catch (Exception exception)
		{
			resolvedReason = $"the reason did throw {Formatter.Format(exception.GetType()).PrependAOrAn()}: " +
			                 exception.Message.DisplayWhitespace();
		}

		if (!string.IsNullOrWhiteSpace(resolvedReason))
		{
			_message = CreateMessage(resolvedReason);
		}

		_isResolved = true;
	}

	/// <summary>
	///     The message of the reason, or an empty string while it is not resolved.
	/// </summary>
	public override string ToString()
		=> _message ?? "";
}
