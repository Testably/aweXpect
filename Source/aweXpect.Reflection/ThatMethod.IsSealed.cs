using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Is{Not}Sealed", "is {not} sealed",
		Result = nameof(DescribeSealedMethod),
		Summary = "Verifies that the <see cref=\"MethodInfo\" /> is sealed.",
		NegatedSummary = "Verifies that the <see cref=\"MethodInfo\" /> is not sealed.")]
	private static bool IsSealedMethod(MethodInfo method) => method.IsReallySealed();

	private static string DescribeSealedMethod(MethodInfo method)
		=> ResultTexts.Describe(method.IsReallySealed(), "was sealed", "was non-sealed", method);
}
