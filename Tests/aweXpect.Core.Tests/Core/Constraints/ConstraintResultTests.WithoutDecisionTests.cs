using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public partial class ConstraintResultTests
{
	public sealed class WithoutDecisionTests
	{
		[Test]
		public async Task Negate_ShouldNegateTheInnerResultAndStayFailed()
		{
			ThatBoolSubject.IsTrueConstraint inner = new(ExpectationGrammars.None);
			inner.IsMetBy(false);
			ConstraintResult sut = new ConstraintResult.WithoutDecision(inner);

			ConstraintResult negated = sut.Negate();

			await That(negated).IsSameAs(sut);
			await That(negated.Outcome).IsEqualTo(Outcome.Failure)
				.Because("an expectation that did not decide its outcome fails, whether it is negated or not");
			await That(inner.Outcome).IsEqualTo(Outcome.Success);
			await That(negated.GetExpectationText()).IsEqualTo("is not True");
			await That(negated.GetResultText())
				.IsEqualTo("it could not be verified, because the expectation did not decide its outcome");
		}
	}
}
