using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}Virtual", "is {not} virtual",
		Result = nameof(DescribeVirtualProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is virtual.",
		NegatedSummary = "Verifies that the <see cref=\"PropertyInfo\" /> is not virtual.")]
	private static bool IsVirtualProperty(PropertyInfo property) => property.IsReallyVirtual();

	private static string DescribeVirtualProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsReallyVirtual(), "was virtual", "was non-virtual", property);
}
