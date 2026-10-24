using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatAssembly
{
	[GenerateExpectation("Is{Not}StrongNamed", "is {not} strong named",
		Result = nameof(DescribeStrongNamedAssembly),
		Summary = "Verifies that the <see cref=\"Assembly\" /> is strong named.",
		NegatedSummary = "Verifies that the <see cref=\"Assembly\" /> is not strong named.")]
	private static bool IsStrongNamedAssembly(Assembly assembly) => assembly.IsStrongNamed();

	private static string DescribeStrongNamedAssembly(Assembly assembly)
		=> ResultTexts.Describe(assembly.IsStrongNamed(), "was strong named", "was not strong named", assembly);
}
