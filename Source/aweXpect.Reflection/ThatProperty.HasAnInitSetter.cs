using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("HasAnInitSetter", "has an init setter",
		NegatedName = "DoesNotHaveAnInitSetter",
		NegatedExpectationText = "does not have an init setter",
		Result = nameof(DescribeInitSetter),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> has an init-only setter.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> does not have an init-only setter.")]
	private static bool HasAnInitSetterProperty(PropertyInfo property) => property.HasInitSetter();

	private static string DescribeInitSetter(PropertyInfo property)
		=> ResultTexts.Describe(property.HasInitSetter(), "had an init setter", "did not have an init setter",
			property);
}
