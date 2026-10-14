using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatString
{
	[GenerateExpectation("Is{Not}Empty", "is {not} empty",
		NegatedRemarks = "<see langword=\"null\" /> is neither treated as empty nor as not empty, so it fails.")]
	private static bool IsEmptyString(string value) => value.Length == 0;
}
