using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Core.Constraints;

/// <summary>
///     An async constraint on type <typeparamref name="TValue" />.
/// </summary>
public interface IAsyncConstraint<in TValue> : IConstraint
{
	/// <summary>
	///     Checks if the <paramref name="actual" /> value meets the expectation.
	/// </summary>
	ValueTask<ConstraintResult> IsMetBy(TValue actual, CancellationToken cancellationToken);
}
