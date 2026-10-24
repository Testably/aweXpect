using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Is{Not}Async", "is {not} async",
		Result = nameof(DescribeAsyncMethod),
		Summary = "Verifies that the <see cref=\"MethodInfo\" /> is asynchronous (declared with the <see langword=\"async\" /> keyword).",
		NegatedSummary = """
		                 Verifies that the <see cref="MethodInfo" /> is not asynchronous (not declared with the <see langword="async" />
		                 keyword).
		                 """)]
	private static bool IsAsyncMethod(MethodInfo method) => method.IsAsync();

	private static string DescribeAsyncMethod(MethodInfo method)
		=> ResultTexts.Describe(method.IsAsync(), "was async", "was non-async", method);
}
