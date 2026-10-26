using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("IsReadable", "is readable",
		NegatedExpectationText = "is not readable",
		Result = nameof(DescribeReadableProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is readable (can be read).",
		Remarks = "Every property has at least one accessor, so <see cref=\"IsWriteOnly\" /> is the negation.")]
	private static bool IsReadableProperty(PropertyInfo property) => property.IsReadable();

	private static string DescribeReadableProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsReadable(), "was readable", "was not readable", property);
}
