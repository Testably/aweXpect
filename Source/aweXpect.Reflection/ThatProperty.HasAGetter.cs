using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("HasAGetter", "has a getter",
		NegatedExpectationText = "does not have a getter",
		Result = nameof(DescribeGetter),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> has a getter.")]
	private static bool HasAGetterProperty(PropertyInfo property) => property.HasGetter();

	private static string DescribeGetter(PropertyInfo property)
		=> ResultTexts.Describe(property.HasGetter(), "had a getter", "did not have a getter", property);
}
