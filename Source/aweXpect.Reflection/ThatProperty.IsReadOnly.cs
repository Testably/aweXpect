using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("IsReadOnly", "is read-only",
		NegatedExpectationText = "is not read-only",
		Result = nameof(DescribeReadOnlyProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is read-only (can be read but not written).")]
	private static bool IsReadOnlyProperty(PropertyInfo property) => property.IsReadOnly();

	private static string DescribeReadOnlyProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsReadOnly(), "was read-only", "was not read-only", property);
}
