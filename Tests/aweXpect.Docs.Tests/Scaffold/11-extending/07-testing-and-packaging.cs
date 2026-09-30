using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace Snippets
{
	// The expectation that the other pages of this section write.
	internal static class IsAbsolutePathExtensions
	{
		public static AndOrResult<string, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
			=> new(((IExpectThat<string?>)subject).ExpectationBuilder.AddConstraint((it, grammars)
					=> new IsAbsolutePathConstraint(it, grammars)),
				subject);
	}

	internal sealed class IsAbsolutePathConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual) => this;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
	}
}
