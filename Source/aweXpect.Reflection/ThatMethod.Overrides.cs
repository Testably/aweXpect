using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatMethod
{
	[GenerateExpectation("Overrides", "overrides a base method",
		NegatedName = "DoesNotOverride",
		NegatedExpectationText = "does not override a base method",
		Result = nameof(DescribeOverridingMethod),
		Summary = "Verifies that the <see cref=\"MethodInfo\" /> overrides a base class method.",
		NegatedSummary = "Verifies that the <see cref=\"MethodInfo\" /> does not override a base class method.")]
	private static bool OverridesMethod(MethodInfo method) => method.IsOverride();

	private static string DescribeOverridingMethod(MethodInfo method)
		=> ResultTexts.Describe(method.IsOverride(),
			"did override a base method", "did not override a base method", method);
}
