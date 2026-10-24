using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("IsWriteOnly", "is write-only",
		NegatedExpectationText = "is not write-only",
		Result = nameof(DescribeWriteOnlyProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is write-only (can be written but not read).")]
	private static bool IsWriteOnlyProperty(PropertyInfo property) => property.IsWriteOnly();

	private static string DescribeWriteOnlyProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsWriteOnly(), "was write-only", "was not write-only", property);
}
