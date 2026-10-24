using System.Reflection;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatField
{
	[GenerateExpectation("Is{Not}ReadOnly", "is {not} read-only",
		Result = nameof(DescribeReadOnlyField),
		Summary = "Verifies that the <see cref=\"FieldInfo\" /> is read-only.",
		NegatedSummary = "Verifies that the <see cref=\"FieldInfo\" /> is not read-only.")]
	private static bool IsReadOnlyField(FieldInfo field) => field.IsInitOnly;

	private static string DescribeReadOnlyField(FieldInfo field)
		=> ResultTexts.Describe(field.IsInitOnly, "was read-only", "was non-read-only", field);
}
