using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatString
{
	[GenerateExpectation("Is{Not}NullOrWhiteSpace", "is {not} null or whitespace",
		Summary = "Verifies that the subject is <see langword=\"null\" />, <see cref=\"string.Empty\" /> or consists only of whitespace characters.",
		NegatedSummary = "Verifies that the subject is neither <see langword=\"null\" /> nor <see cref=\"string.Empty\" /> and does not consist only of whitespace characters.")]
	private static bool IsNullOrWhiteSpaceString([NotNullWhen(false)] string? value) => string.IsNullOrWhiteSpace(value);
}
