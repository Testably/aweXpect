using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Constraints;

/// <summary>
///     A constraint that creates its <see cref="ConstraintResult" /> only during the evaluation, and therefore provides
///     the expectation separately, for when the value is not available (e.g. a <see langword="null" /> subject or a
///     throwing member).
/// </summary>
/// <remarks>
///     Without it, the constraint itself is used for the expectation, without evaluating it.
/// </remarks>
public interface IExpectationTextConstraint : IConstraint
{
	/// <summary>
	///     Returns the unevaluated result, whose expectation text also reflects a negation, without accessing any value.
	/// </summary>
	/// <remarks>
	///     Nested expectations must be evaluated with the given <paramref name="context" />, so that they are not
	///     evaluated either.
	/// </remarks>
	public Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken);
}
