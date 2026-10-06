using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Constraints;

public partial class ConstraintResultTests
{
	public sealed class WithOtherExceptionsTests
	{
		[Test]
		[Arguments(true)]
		[Arguments(false)]
		public async Task IsExpectationOnly_ShouldBeTheOneOfTheInnerResult(bool isExpectationOnly)
		{
			ConstraintResult inner = isExpectationOnly
				? new ConstraintResult.ExpectationOnly<int>(ExpectationGrammars.None, "foo")
				: new DummyConstraintResult(Outcome.Failure, "foo");
			ConstraintResult sut = new ConstraintResult.WithOtherExceptions(inner, [new Exception("bar"),]);

			await That(sut.IsExpectationOnly).IsEqualTo(isExpectationOnly);
		}

		[Test]
		public async Task LeadingAndTrailingSubject_ShouldBeTheOnesOfTheInnerResult()
		{
			ThatBoolSubject.IsTrueConstraint inner = new(ExpectationGrammars.None);
			inner.IsMetBy(false);
			ConstraintResult sut = new ConstraintResult.WithOtherExceptions(inner, [new Exception("bar"),]);

			await That(sut.LeadingSubject).IsEqualTo("it");
			await That(sut.TrailingSubject).IsEqualTo("it");
		}

		[Test]
		public async Task Negate_ShouldNegateTheInnerResult()
		{
			ThatBoolSubject.IsTrueConstraint inner = new(ExpectationGrammars.None);
			inner.IsMetBy(true);
			ConstraintResult sut = new ConstraintResult.WithOtherExceptions(inner, [new Exception("bar"),]);

			ConstraintResult negated = sut.Negate();

			await That(negated).IsSameAs(sut);
			await That(negated.Outcome).IsEqualTo(Outcome.Failure)
				.Because("the outcome is the one of the negated inner result");
			await That(negated.GetExpectationText()).IsEqualTo("is not True");
			await That(negated.GetResultText()).IsEqualTo("it was");
		}
	}
}
