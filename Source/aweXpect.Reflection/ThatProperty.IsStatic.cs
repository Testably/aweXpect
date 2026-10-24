using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}Static", "is {not} static",
		Result = nameof(DescribeStaticProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is static.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> is not static.")]
	private static bool IsStaticProperty(PropertyInfo property) => property.IsReallyStatic();

	private static string DescribeStaticProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsReallyStatic(), "was static", "was non-static", property);
}
