using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Internal.Tests.ThatTests.Exceptions;

public class HasRecursiveInnerExceptionsConstraintTests
{
	[Test]
	[Arguments(ExpectationGrammars.None, "has recursive inner exceptions")]
	[Arguments(ExpectationGrammars.Negated, "does not have recursive inner exceptions")]
	[Arguments(ExpectationGrammars.Nested, "whose recursive inner exceptions are")]
	[Arguments(ExpectationGrammars.Nested | ExpectationGrammars.Negated, "whose recursive inner exceptions are not")]
	[Arguments(ExpectationGrammars.Active, "with recursive inner exceptions")]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated, "without recursive inner exceptions")]
	public async Task AppendExpectation_ShouldAppendExpectedText(ExpectationGrammars grammar, string expected)
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new(
			"it", grammar);
		StringBuilder sb = new();

		sut.AppendExpectation(sb, "");

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Test]
	[Arguments(ExpectationGrammars.None)]
	[Arguments(ExpectationGrammars.Nested)]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Nested)]
	public async Task AppendResult_WhenExceptionHasNoInnerExceptions_ShouldAppendResultText(
		ExpectationGrammars grammar)
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new(
			"it", grammar);
		StringBuilder sb = new();

		sut.IsMetBy(new AggregateException());
		sut.AppendResult(sb, "");

		await That(sb.ToString()).IsEqualTo("it had no inner exceptions");
	}

	[Test]
	[Arguments(false)]
	[Arguments(true)]
	public async Task IsMetBy_ShouldRequireAtLeastOneRecursiveInnerException(bool hasInnerException)
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new(
			"it", ExpectationGrammars.None);
		Exception subject = hasInnerException
			? new AggregateException(new AggregateException(new Exception("inner")))
			: new AggregateException();

		ConstraintResult result = sut.IsMetBy(subject);

		await That(result.Outcome).IsEqualTo(hasInnerException ? Outcome.Success : Outcome.Failure);
	}

	[Test]
	public async Task IsMetBy_WhenReusedAfterAnExceptionWithoutInnerExceptions_ShouldContinueTheFurtherProcessing()
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new(
			"it", ExpectationGrammars.None);
		sut.IsMetBy(new AggregateException());

		ConstraintResult result = sut.IsMetBy(new AggregateException(new Exception("inner")));

		await That(result.FurtherProcessingStrategy).IsEqualTo(FurtherProcessingStrategy.Continue)
			.Because("the expectations on the inner exceptions explain the result again, when there are inner exceptions");
	}
}
