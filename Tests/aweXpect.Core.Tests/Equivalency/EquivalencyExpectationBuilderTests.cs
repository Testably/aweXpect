using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyExpectationBuilderTests
{
	[Test]
	public async Task AppendExpectation_ShouldAppendTheExpectationOfTheRootNode()
	{
		EquivalencyExpectationBuilder sut = CreateIsGreaterThan2();
		StringBuilder sb = new();

		sut.AppendExpectation(sb);

		await That(sb.ToString()).IsEqualTo("is greater than 2");
	}

	[Test]
	public async Task IsMet_ShouldThrowNotSupportedException()
	{
		EquivalencyExpectationBuilder<int> sut = new();

		async Task Act() => await sut.IsMet(
			new ExpectationNode(), null!, new TimeSystemMock(), null, CancellationToken.None);

		await That(Act).Throws<NotSupportedException>()
			.WithMessage("Use IsMetBy for EquivalencyExpectationBuilder.");
	}

	[Test]
	public async Task IsMetBy_WhenTypeDoesNotMatch_ShouldFailAndDescribeTheActualType()
	{
		EquivalencyExpectationBuilder sut = CreateIsGreaterThan2();
		StringBuilder expectation = new();
		StringBuilder result = new();

		ConstraintResult constraintResult =
			await sut.IsMetBy("foo", new EvaluationContext.EvaluationContext(), CancellationToken.None);
		constraintResult.AppendExpectation(expectation);
		constraintResult.AppendResult(result);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(expectation.ToString()).IsEmpty()
			.Because("the expectations on the expected type are not evaluated for a value of another type");
		await That(result.ToString()).IsEqualTo(" was string");
	}

	[Test]
	public async Task IsMetBy_WhenTypeDoesNotMatch_ShouldNotChangeTheResultWhenNegated()
	{
		EquivalencyExpectationBuilder sut = CreateIsGreaterThan2();
		ConstraintResult constraintResult =
			await sut.IsMetBy("foo", new EvaluationContext.EvaluationContext(), CancellationToken.None);

		ConstraintResult negated = constraintResult.Negate();

		await That(negated).IsSameAs(constraintResult);
		await That(negated.Outcome).IsEqualTo(Outcome.Failure)
			.Because("a value of another type fails in both polarities");
	}

	[Test]
	public async Task IsMetBy_WhenTypeDoesNotMatch_ShouldStoreTheValue()
	{
		EquivalencyExpectationBuilder sut = CreateIsGreaterThan2();
		ConstraintResult constraintResult =
			await sut.IsMetBy("foo", new EvaluationContext.EvaluationContext(), CancellationToken.None);

		bool hasString = constraintResult.TryGetStoredValue(out string? storedString);
		bool hasInt = constraintResult.TryGetStoredValue(out int storedInt);

		await That(hasString).IsTrue();
		await That(storedString).IsEqualTo("foo");
		await That(hasInt).IsFalse();
		await That(storedInt).IsEqualTo(0);
	}

	[Test]
	public async Task IsMetBy_WhenValueIsNullForAReferenceType_ShouldEvaluateTheExpectationsAndDescribeNull()
	{
		It.IsEquivalent<string> isEquivalent = It.Is<string>();
		_ = isEquivalent.That.IsEmpty();
		EquivalencyExpectationBuilder sut =
			(EquivalencyExpectationBuilder)((IExpectThat<string>)isEquivalent).ExpectationBuilder;
		StringBuilder expectation = new();
		StringBuilder result = new();

		ConstraintResult constraintResult =
			await sut.IsMetBy<string?>(null, new EvaluationContext.EvaluationContext(), CancellationToken.None);
		constraintResult.AppendExpectation(expectation);
		constraintResult.AppendResult(result);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.FailureBothWays);
		await That(expectation.ToString()).IsEqualTo("is empty")
			.Because("null is a valid value of a reference type, so the expectations are evaluated for it");
		await That(result.ToString()).IsEqualTo(" was <null>");
	}

	[Test]
	public async Task IsMetBy_WhenValueIsNullForAValueType_ShouldFailWithoutEvaluatingTheExpectations()
	{
		EquivalencyExpectationBuilder sut = CreateIsGreaterThan2();
		StringBuilder expectation = new();

		ConstraintResult constraintResult =
			await sut.IsMetBy<string?>(null, new EvaluationContext.EvaluationContext(), CancellationToken.None);
		constraintResult.AppendExpectation(expectation);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure)
			.Because("null is no valid value of a non-nullable value type");
		await That(expectation.ToString()).IsEmpty();
		await That(sut.ToString()).IsEqualTo("is int");
	}

	[Test]
	public async Task ToString_WhenNotEvaluated_ShouldOnlyContainTheType()
	{
		EquivalencyExpectationBuilder sut = CreateIsGreaterThan2();

		string? result = sut.ToString();

		await That(result).IsEqualTo("is int")
			.Because("the expectations are only described after they were evaluated");
	}

	private static EquivalencyExpectationBuilder CreateIsGreaterThan2()
	{
		It.IsEquivalent<int> isEquivalent = It.Is<int>();
		isEquivalent.That.IsGreaterThan(2);
		return (EquivalencyExpectationBuilder)((IExpectThat<int>)isEquivalent).ExpectationBuilder;
	}
}
