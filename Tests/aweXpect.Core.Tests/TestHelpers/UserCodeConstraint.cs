using aweXpect.Core.Constraints;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A constraint whose evaluation calls the <paramref name="userCode" /> through <see cref="UserCode" />.
/// </summary>
internal sealed class UserCodeConstraint<T>(Func<bool> userCode, string expectation, string negatedExpectation)
	: ConstraintResult.ExpectationOnly<T>(ExpectationGrammars.None, expectation, negatedExpectation),
		IValueConstraint<T>
{
	ConstraintResult IValueConstraint<T>.IsMetBy(T actual)
	{
		Outcome = UserCode.Invoke(userCode) ? Outcome.Success : Outcome.Failure;
		return this;
	}
}
