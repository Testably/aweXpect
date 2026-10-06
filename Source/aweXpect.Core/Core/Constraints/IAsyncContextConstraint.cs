using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Constraints;

/// <summary>
///     An async constraint on type <typeparamref name="TValue" /> that uses the <see cref="IEvaluationContext" />.
/// </summary>
public interface IAsyncContextConstraint<in TValue> : IConstraint
{
	/// <summary>
	///     Checks if the <paramref name="actual" /> value meets the expectation using the <see cref="IEvaluationContext" />.
	/// </summary>
	ValueTask<ConstraintResult> IsMetBy(TValue actual, IEvaluationContext context,
		CancellationToken cancellationToken);
}
