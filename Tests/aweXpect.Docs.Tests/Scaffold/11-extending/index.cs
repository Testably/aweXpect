// The page marks its test with the `[Test]` attribute of TUnit.
global using TUnit.Core;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace Snippets
{
	// The page declares the constraint privately in a sample of its own, and uses it from the expectation.
	internal sealed class IsRadioFriendlyConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Track>(it, grammars),
			IValueConstraint<Track?>
	{
		public ConstraintResult IsMetBy(Track? actual) => this;

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null) { }

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null) { }
	}
}

// The record on the page needs this type, which is missing in netstandard2.0 and net48, and on the other targets it only
// causes a warning.
namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit;
}
