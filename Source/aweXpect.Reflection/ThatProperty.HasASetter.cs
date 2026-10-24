using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("HasASetter", "has a setter",
		NegatedName = "DoesNotHaveASetter",
		NegatedExpectationText = "does not have a setter",
		Result = nameof(DescribeSetter),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> has a (regular, non-init) setter.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> does not have a (regular, non-init) setter.")]
	private static bool HasASetterProperty(PropertyInfo property) => property.HasSetter();

	private static string DescribeSetter(PropertyInfo property)
		=> ResultTexts.Describe(property.HasSetter(), "had a setter", "did not have a setter", property);
}
