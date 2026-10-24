using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatConstructor
{
	[GenerateExpectation("Is{Not}Static", "is {not} static",
		Result = nameof(DescribeStaticConstructor),
		Summary = "Verifies that the <see cref=\"ConstructorInfo\" /> is static.",
		NegatedSummary = "Verifies that the <see cref=\"ConstructorInfo\" /> is not static.")]
	private static bool IsStaticConstructor(ConstructorInfo constructor) => constructor.IsStatic;

	private static string DescribeStaticConstructor(ConstructorInfo constructor)
		=> ResultTexts.Describe(constructor.IsStatic, "was static", "was non-static", constructor);
}
