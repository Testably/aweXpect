using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace Snippets
{
	// The page declares the constraint privately next to each of its versions, and uses it from another sample.
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

	// The "Your first expectation" page shows this helper.
	internal static class ExpectThatExtensions
	{
		public static IExpectThat<T> Get<T>(this IThat<T> subject) => (IExpectThat<T>)subject;
	}
}

// The page tells to declare this attribute on targets that miss it, and on the others it only causes a warning.
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute
	{
		public bool ReturnValue { get; } = returnValue;
	}
}
