// The "Extending aweXpect" page shows this import.
global using aweXpect.Core.Extending;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace Snippets
{
	// The type that the "Extending aweXpect" page declares.
	public record Track(string Title, TimeSpan Duration);

	// The page declares the constraint privately next to each of its versions, and uses it from another sample.
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

// The record needs this type, which is missing in netstandard2.0 and net48, and on the other targets it only causes a
// warning.
namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit;
}
