using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Is{Not}Static", "is {not} static",
		Result = nameof(DescribeStaticMethod),
		Summary = "Verifies that the <see cref=\"MethodInfo\" /> is static.",
		NegatedSummary = "Verifies that the <see cref=\"MethodInfo\" /> is not static.")]
	private static bool IsStaticMethod(MethodInfo method) => method.IsStatic;

	private static string DescribeStaticMethod(MethodInfo method)
		=> ResultTexts.Describe(method.IsStatic, "was static", "was non-static", method);
}
