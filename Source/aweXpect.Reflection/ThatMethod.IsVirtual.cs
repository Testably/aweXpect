using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Is{Not}Virtual", "is {not} virtual",
		Result = nameof(DescribeVirtualMethod),
		Summary = "Verifies that the <see cref=\"MethodInfo\" /> is virtual.",
		NegatedSummary = "Verifies that the <see cref=\"MethodInfo\" /> is not virtual.")]
	private static bool IsVirtualMethod(MethodInfo method) => method.IsVirtual;

	private static string DescribeVirtualMethod(MethodInfo method)
		=> ResultTexts.Describe(method.IsVirtual, "was virtual", "was non-virtual", method);
}
