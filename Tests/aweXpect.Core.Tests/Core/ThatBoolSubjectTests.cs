using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public sealed class ThatBoolSubjectTests
{
	[Test]
	[Arguments(false, Outcome.Success)]
	[Arguments(true, Outcome.Failure)]
	public async Task IsTrueConstraint_WhenNegated_ShouldRenderTheNegatedExpectationAndResult(bool actual,
		Outcome expectedOutcome)
	{
		ThatBoolSubject.IsTrueConstraint sut = new(ExpectationGrammars.None);
		sut.IsMetBy(actual);

		ConstraintResult negated = sut.Negate();

		await That(negated.Outcome).IsEqualTo(expectedOutcome);
		await That(negated.GetExpectationText()).IsEqualTo("is not True");
		await That(negated.GetResultText()).IsEqualTo("it was");
	}

	[Test]
	public async Task WhenAwaitedWithoutExpectation_AndFalse_ShouldFail()
	{
		bool subject = false;

		async Task Act()
		{
#pragma warning disable aweXpect0001
			ThatBoolSubject sut = That(subject);
#pragma warning restore aweXpect0001
			await sut;
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is True,
			             but it was False
			             """);
	}

	[Test]
	public async Task WhenAwaitedWithoutExpectation_AndTrue_ShouldSucceed()
	{
		bool subject = true;

		async Task Act()
		{
#pragma warning disable aweXpect0001
			ThatBoolSubject sut = That(subject);
#pragma warning restore aweXpect0001
			await sut;
		}

		await That(Act).DoesNotThrow();
	}
}
