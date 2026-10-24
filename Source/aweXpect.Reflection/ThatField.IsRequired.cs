using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatField
{
	[GenerateExpectation("Is{Not}Required", "is {not} required",
		Result = nameof(DescribeRequiredField),
		Summary = "Verifies that the <see cref=\"FieldInfo\" /> is required.",
		NegatedSummary = "Verifies that the <see cref=\"FieldInfo\" /> is not required.")]
	private static bool IsRequiredField(FieldInfo field) => field.IsRequired();

	private static string DescribeRequiredField(FieldInfo field)
		=> ResultTexts.Describe(field.IsRequired(), "was required", "was non-required", field);
}
