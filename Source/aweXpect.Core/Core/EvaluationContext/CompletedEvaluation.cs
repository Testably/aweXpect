using aweXpect.Core.Constraints;

namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     The completed evaluation of an expectation, which a callback registered with
///     <see cref="IEvaluationContext.OnCompleted" /> receives.
/// </summary>
public sealed class CompletedEvaluation
{
	internal CompletedEvaluation(Outcome outcome)
	{
		Outcome = outcome;
	}

	/// <summary>
	///     The outcome of the whole expectation, after all negations.
	/// </summary>
	/// <remarks>
	///     It is <see cref="Outcome.Success" />, <see cref="Outcome.Failure" /> or <see cref="Outcome.Undecided" />, the
	///     latter when the evaluation was canceled, while a timeout fails the expectation. A failure that a negation
	///     would have kept is a <see cref="Outcome.Failure" /> as well, as nothing negates the whole expectation any more.
	/// </remarks>
	public Outcome Outcome { get; }
}
