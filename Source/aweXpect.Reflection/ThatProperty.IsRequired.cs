using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}Required", "is {not} required",
		Result = nameof(DescribeRequiredProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is required.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> is not required.")]
	private static bool IsRequiredProperty(PropertyInfo property) => property.IsRequired();

	private static string DescribeRequiredProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsRequired(), "was required", "was non-required", property);
}
