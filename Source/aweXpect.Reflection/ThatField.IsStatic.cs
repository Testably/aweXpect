using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatField
{
	[GenerateExpectation("Is{Not}Static", "is {not} static",
		Result = nameof(DescribeStaticField),
		Summary = "Verifies that the <see cref=\"FieldInfo\" /> is static.",
		NegatedSummary = "Verifies that the <see cref=\"FieldInfo\" /> is not static.")]
	private static bool IsStaticField(FieldInfo field) => field.IsStatic;

	private static string DescribeStaticField(FieldInfo field)
		=> ResultTexts.Describe(field.IsStatic, "was static", "was non-static", field);
}
