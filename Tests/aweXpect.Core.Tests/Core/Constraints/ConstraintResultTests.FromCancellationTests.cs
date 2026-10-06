using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public partial class ConstraintResultTests
{
	public sealed class FromCancellationTests
	{
		[Test]
		public async Task Negate_ShouldNegateTheInnerResultAndStayUndecided()
		{
			ThatBoolSubject.IsTrueConstraint inner = new(ExpectationGrammars.None);
			inner.IsMetBy(false);
			ConstraintResult sut = new ConstraintResult.FromCancellation(inner);

			ConstraintResult negated = sut.Negate();

			await That(negated).IsSameAs(sut);
			await That(negated.Outcome).IsEqualTo(Outcome.Undecided)
				.Because("a canceled expectation is undecided, whether it is negated or not");
			await That(inner.Outcome).IsEqualTo(Outcome.Success);
			await That(negated.GetExpectationText()).IsEqualTo("is not True");
			await That(negated.GetResultText())
				.IsEqualTo("it could not be verified, because the evaluation was already canceled");
		}
	}
}
