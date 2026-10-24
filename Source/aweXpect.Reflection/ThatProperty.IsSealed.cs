using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}Sealed", "is {not} sealed",
		Result = nameof(DescribeSealedProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is sealed.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> is not sealed.")]
	private static bool IsSealedProperty(PropertyInfo property) => property.IsReallySealed();

	private static string DescribeSealedProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsReallySealed(), "was sealed", "was non-sealed", property);
}
