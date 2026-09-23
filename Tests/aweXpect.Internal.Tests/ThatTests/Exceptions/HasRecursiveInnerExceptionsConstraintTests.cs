using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Internal.Tests.ThatTests.Exceptions;

public class HasRecursiveInnerExceptionsConstraintTests
{
	[Theory]
	[InlineData(ExpectationGrammars.None, "has recursive inner exceptions")]
	[InlineData(ExpectationGrammars.Negated, "does not have recursive inner exceptions")]
	[InlineData(ExpectationGrammars.Nested, "whose recursive inner exceptions are")]
	[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Negated, "whose recursive inner exceptions are not")]
	[InlineData(ExpectationGrammars.Active, "with recursive inner exceptions")]
	[InlineData(ExpectationGrammars.Active | ExpectationGrammars.Negated, "without recursive inner exceptions")]
	public async Task AppendExpectation_ShouldAppendExpectedText(ExpectationGrammars grammar, string expected)
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new("it", grammar);
		StringBuilder sb = new();

		sut.AppendExpectation(sb, "");

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(ExpectationGrammars.None)]
	[InlineData(ExpectationGrammars.Nested)]
	[InlineData(ExpectationGrammars.Active | ExpectationGrammars.Nested)]
	public async Task AppendResult_WhenExceptionHasNoInnerExceptions_ShouldAppendResultText(
		ExpectationGrammars grammar)
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new("it", grammar);
		StringBuilder sb = new();

		sut.IsMetBy(new AggregateException());
		sut.AppendResult(sb, "");

		await That(sb.ToString()).IsEqualTo("it had no inner exceptions");
	}

	[Theory]
	[InlineData(false)]
	[InlineData(true)]
	public async Task IsMetBy_ShouldRequireAtLeastOneRecursiveInnerException(bool hasInnerException)
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new("it", ExpectationGrammars.None);
		Exception subject = hasInnerException
			? new AggregateException(new AggregateException(new Exception("inner")))
			: new AggregateException();

		ConstraintResult result = sut.IsMetBy(subject);

		await That(result.Outcome).IsEqualTo(hasInnerException ? Outcome.Success : Outcome.Failure);
	}
}
