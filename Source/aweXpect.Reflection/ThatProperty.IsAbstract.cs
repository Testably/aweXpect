using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}Abstract", "is {not} abstract",
		Result = nameof(DescribeAbstractProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is abstract.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> is not abstract.")]
	private static bool IsAbstractProperty(PropertyInfo property) => property.IsReallyAbstract();

	private static string DescribeAbstractProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsReallyAbstract(), "was abstract", "was non-abstract", property);
}
