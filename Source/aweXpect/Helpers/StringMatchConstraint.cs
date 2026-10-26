using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Helpers;

/// <summary>
///     A constraint that compares the subject with the expected <see langword="string" />, see
///     <see cref="StringMatchConstraint.IsMetBy" />.
/// </summary>
internal interface IStringMatchConstraint
{
	/// <summary>
	///     Starts the comparison of the <paramref name="actual" /> value in the <paramref name="context" />.
	/// </summary>
	ValueTask<MatchResult> StartMatch(string? actual, IEvaluationContext context, CancellationToken cancellationToken);

	/// <summary>
	///     Completes the constraint with the <paramref name="matchResult" /> of the comparison.
	/// </summary>
	ConstraintResult CompleteMatch(MatchResult matchResult);
}

internal static class StringMatchConstraint
{
	/// <summary>
	///     Evaluates the <paramref name="constraint" /> on the <paramref name="actual" /> value.
	/// </summary>
	/// <remarks>
	///     A comparison that completes synchronously, as it does for all built-in match types, returns the result without
	///     starting a state machine. An exception is returned in a faulted task, so that awaiting the result throws it,
	///     as for an asynchronous constraint.
	/// </remarks>
	public static ValueTask<ConstraintResult> IsMetBy(IStringMatchConstraint constraint, string? actual,
		IEvaluationContext context, CancellationToken cancellationToken)
	{
		ValueTask<MatchResult> match;
		try
		{
			match = constraint.StartMatch(actual, context, cancellationToken);
		}
		catch (Exception exception)
		{
			return new ValueTask<ConstraintResult>(Task.FromException<ConstraintResult>(exception));
		}

		return match.IsCompletedSuccessfully
			? new ValueTask<ConstraintResult>(constraint.CompleteMatch(match.Result))
			: CompleteMatchAsync(constraint, match);
	}

	private static async ValueTask<ConstraintResult> CompleteMatchAsync(IStringMatchConstraint constraint,
		ValueTask<MatchResult> match)
		=> constraint.CompleteMatch(await match);
}
