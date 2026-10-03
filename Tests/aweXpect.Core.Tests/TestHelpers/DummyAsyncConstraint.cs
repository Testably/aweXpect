using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Tests.TestHelpers;

internal class DummyAsyncConstraint<T>(Func<T, Task<ConstraintResult>> callback) : IAsyncConstraint<T>
{
	public ValueTask<ConstraintResult> IsMetBy(T actual, CancellationToken cancellationToken)
		=> new(callback(actual));

	public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
}
