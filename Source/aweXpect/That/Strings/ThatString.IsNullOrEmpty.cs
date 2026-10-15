using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatString
{
	[GenerateExpectation("Is{Not}NullOrEmpty", "is {not} null or empty",
		Summary = "Verifies that the subject is <see langword=\"null\" /> or <see cref=\"string.Empty\" />.",
		NegatedSummary = "Verifies that the subject is neither <see langword=\"null\" /> nor <see cref=\"string.Empty\" />.")]
	private static bool IsNullOrEmptyString([NotNullWhen(false)] string? value) => string.IsNullOrEmpty(value);
}
