using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace Snippets;

// The constraint is written on the "Constraints and results" page.
internal sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<string>(it, grammars),
		IValueConstraint<string>
{
	public ConstraintResult IsMetBy(string actual) => this;

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
}
