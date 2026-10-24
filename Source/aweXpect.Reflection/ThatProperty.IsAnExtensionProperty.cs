using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatProperty
{
	[GenerateExpectation("Is{Not}AnExtensionProperty", "is {not} an extension property",
		Result = nameof(DescribeExtensionProperty),
		Summary = """
		          Verifies that the <see cref="PropertyInfo" /> is an extension property (declared with the C# extension block
		          syntax).
		          """,
		NegatedSummary = """
		                 Verifies that the <see cref="PropertyInfo" /> is not an extension property (not declared with the C# extension
		                 block syntax).
		                 """)]
	private static bool IsAnExtensionPropertyInfo(PropertyInfo property) => property.IsExtensionProperty();

	private static string DescribeExtensionProperty(PropertyInfo property)
		=> ResultTexts.Describe(property.IsExtensionProperty(),
			"was an extension property", "was not an extension property", property);
}
