using System.Text;
using aweXpect.Core;

namespace aweXpect.Internal.Tests.ThatTests.Exceptions;

public class HasRecursiveInnerExceptionsConstraintTests
{
	[Theory]
	[InlineData(ExpectationGrammars.None, "has recursive inner exceptions")]
	[InlineData(ExpectationGrammars.Negated, "does not have recursive inner exceptions")]
	[InlineData(ExpectationGrammars.Nested, "recursive inner exceptions are")]
	[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Negated, "recursive inner exceptions are not")]
	[InlineData(ExpectationGrammars.Active, "with recursive inner exceptions")]
	[InlineData(ExpectationGrammars.Active | ExpectationGrammars.Negated, "without recursive inner exceptions")]
	public async Task AppendExpectation_ShouldAppendExpectedText(ExpectationGrammars grammar, string expected)
	{
		ThatException.HasRecursiveInnerExceptionsConstraint sut = new("it", grammar);
		StringBuilder sb = new();

		sut.AppendExpectation(sb, "");

		await That(sb.ToString()).IsEqualTo(expected);
	}
}
