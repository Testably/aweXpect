using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Tests.TestHelpers;

internal class DummyAsyncContextConstraint<T>(Func<T, Task<ConstraintResult>> callback) : IAsyncContextConstraint<T>
{
	public ValueTask<ConstraintResult> IsMetBy(T actual, IEvaluationContext context, CancellationToken cancellationToken) => new(callback(actual));

	public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
}
