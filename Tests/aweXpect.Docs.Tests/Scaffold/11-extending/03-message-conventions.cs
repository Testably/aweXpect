using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace Snippets
{
	// The constraint that the sample for the code of the caller passes the source code to.
	internal sealed class HasFileNameMatchingConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<string, bool> predicate,
		string predicateExpression)
		: ConstraintResult.WithNotNullValue<string>(it, grammars),
			IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual) => this;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a file name matching ").Append(predicateExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _ = predicate;

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
	}

	// The "Your first expectation" page shows this helper.
	internal static class ExpectThatExtensions
	{
		public static IExpectThat<T> Get<T>(this IThat<T> subject) => (IExpectThat<T>)subject;
	}
}

// The page tells to declare this attribute on targets that miss it, and on the others it only causes a warning.
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
	{
		public string ParameterName { get; } = parameterName;
	}
}
