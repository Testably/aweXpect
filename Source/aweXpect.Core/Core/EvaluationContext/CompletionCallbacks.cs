using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     The callbacks registered with <see cref="IEvaluationContext.OnCompleted" />.
/// </summary>
internal static class CompletionCallbacks
{
	/// <summary>
	///     Adds the <paramref name="callback" /> to the <paramref name="callbacks" />, unless an equal delegate is already
	///     registered.
	/// </summary>
	/// <remarks>
	///     The list is only created for the first callback, as most evaluations register none.
	/// </remarks>
	public static void Add(ref List<Func<CompletedEvaluation, CancellationToken, ValueTask>>? callbacks,
		Func<CompletedEvaluation, CancellationToken, ValueTask> callback)
	{
		callbacks ??= [];
		if (!callbacks.Contains(callback))
		{
			callbacks.Add(callback);
		}
	}

	/// <summary>
	///     Runs the <paramref name="callbacks" /> one after the other with the <paramref name="outcome" /> of the whole
	///     expectation.
	/// </summary>
	public static async Task Run(List<Func<CompletedEvaluation, CancellationToken, ValueTask>> callbacks,
		Outcome outcome, CancellationToken cancellationToken)
	{
		CompletedEvaluation completedEvaluation =
			new(outcome == Outcome.FailureBothWays ? Outcome.Failure : outcome);
		foreach (Func<CompletedEvaluation, CancellationToken, ValueTask> callback in callbacks)
		{
			await callback(completedEvaluation, cancellationToken);
		}
	}
}
