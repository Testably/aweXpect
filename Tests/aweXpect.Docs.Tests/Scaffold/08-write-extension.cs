using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Formatting;

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

	internal sealed class MyValueFormatter : IValueFormatter
	{
		public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options) => false;
	}
}

// The page tells to declare these attributes on targets that miss them, and on the others they only cause a warning.
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class ModuleInitializerAttribute : Attribute;
}

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute
	{
		public bool ReturnValue { get; } = returnValue;
	}
}
