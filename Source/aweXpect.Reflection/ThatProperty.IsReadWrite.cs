using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("IsReadWrite", "is read-write",
		NegatedExpectationText = "is not read-write",
		Result = nameof(DescribeReadWriteProperty),
		Summary = "Verifies that the <see cref=\"PropertyInfo\" /> is read-write (can be both read and written).")]
	private static bool IsReadWriteProperty(PropertyInfo property) => property.IsReadWrite();

	private static string DescribeReadWriteProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsReadWrite(), "was read-write", "was not read-write", property);
}
