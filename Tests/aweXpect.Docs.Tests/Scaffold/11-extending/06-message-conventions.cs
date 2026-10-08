// The "Extending aweXpect" page shows this import.
global using aweXpect.Core.Extending;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace Snippets
{
	// The type that the "Extending aweXpect" page declares.
	public record Track(string Title, TimeSpan Duration);

	// The constraint that the sample for the code of the caller passes the source code to.
	internal sealed class HasTitleMatchingConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<string, bool> predicate,
		string predicateExpression)
		: ConstraintResult.WithNotNullValue<Track>(it, grammars),
			IValueConstraint<Track?>
	{
		public ConstraintResult IsMetBy(Track? actual) => this;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a title matching ").Append(predicateExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> _ = predicate;

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
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

	// The record needs this type, which is missing in netstandard2.0 and net48, and on the other targets it only causes
	// a warning.
	internal static class IsExternalInit;
}
