using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Is{Not}AnExtensionMethod", "is {not} an extension method",
		Result = nameof(DescribeExtensionMethod),
		Summary = """
		          Verifies that the <see cref="MethodInfo" /> is an extension method (whose first parameter is declared with the
		          <see langword="this" /> modifier).
		          """,
		NegatedSummary = """
		                 Verifies that the <see cref="MethodInfo" /> is not an extension method (whose first parameter is not declared with
		                 the <see langword="this" /> modifier).
		                 """)]
	private static bool IsAnExtensionMethodInfo(MethodInfo method) => method.IsExtensionMethod();

	private static string DescribeExtensionMethod(MethodInfo method)
		=> ResultTexts.Describe(method.IsExtensionMethod(),
			"was an extension method", "was not an extension method", method);
}
