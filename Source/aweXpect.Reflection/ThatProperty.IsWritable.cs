using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("IsWritable", "is writable",
		NegatedExpectationText = "is not writable",
		Result = nameof(DescribeWritableProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is writable (can be written).")]
	private static bool IsWritableProperty(PropertyInfo property) => property.IsWritable();

	private static string DescribeWritableProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsWritable(), "was writable", "was not writable", property);
}
