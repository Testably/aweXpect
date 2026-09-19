using aweXpect.Core.Constraints;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A constraint with an expectation text that throws when it is evaluated.
/// </summary>
internal sealed class NotEvaluatedConstraint<T>(string expectation, string negatedExpectation)
	: ConstraintResult.ExpectationOnly<T>(ExpectationGrammars.None, expectation, negatedExpectation),
		IValueConstraint<T>
{
	ConstraintResult IValueConstraint<T>.IsMetBy(T actual)
		=> throw new InvalidOperationException("The constraint must not be evaluated.");
}
