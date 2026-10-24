using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatField
{
	[GenerateExpectation("Is{Not}Constant", "is {not} constant",
		Result = nameof(DescribeConstantField),
		Summary = "Verifies that the <see cref=\"FieldInfo\" /> is constant.",
		NegatedSummary = "Verifies that the <see cref=\"FieldInfo\" /> is not constant.")]
	private static bool IsConstantField(FieldInfo field) => field.IsLiteral;

	private static string DescribeConstantField(FieldInfo field)
		=> ResultTexts.Describe(field.IsLiteral, "was constant", "was non-constant", field);
}
