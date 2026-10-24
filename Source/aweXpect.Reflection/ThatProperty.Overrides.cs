using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Overrides", "overrides a base property",
		NegatedName = "DoesNotOverride",
		NegatedExpectationText = "does not override a base property",
		Result = nameof(DescribeOverridingProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> overrides a base class property.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> does not override a base class property.")]
	private static bool OverridesProperty(PropertyInfo property) => property.IsOverride();

	private static string DescribeOverridingProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsOverride(),
			"did override a base property", "did not override a base property", property);
}
