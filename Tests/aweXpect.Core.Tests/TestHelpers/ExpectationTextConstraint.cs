using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A constraint that throws when it is evaluated, and provides its expectation as a separate result.
/// </summary>
internal sealed class ExpectationTextConstraint<T>(string expectation, string negatedExpectation)
	: IValueConstraint<T>, IExpectationTextConstraint
{
	public ValueTask<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> new(
			new ConstraintResult.ExpectationOnly<T>(ExpectationGrammars.None, expectation, negatedExpectation));

	public ConstraintResult IsMetBy(T actual)
		=> throw new InvalidOperationException("The constraint must not be evaluated.");

	public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append("the constraint itself");
}
