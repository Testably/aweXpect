using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Is{Not}Abstract", "is {not} abstract",
		Result = nameof(DescribeAbstractMethod),
		Summary = "Verifies that the <see cref=\"MethodInfo\" /> is abstract.",
		NegatedSummary = "Verifies that the <see cref=\"MethodInfo\" /> is not abstract.")]
	private static bool IsAbstractMethod(MethodInfo method) => method.IsAbstract;

	private static string DescribeAbstractMethod(MethodInfo method)
		=> ResultTexts.Describe(method.IsAbstract, "was abstract", "was non-abstract", method);
}
