using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}Nested", "is {not} nested",
		Result = nameof(DescribeNestedType),
		Summary = "Verifies that the <see cref=\"Type\" /> is nested.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not nested.")]
	private static bool IsNestedType(Type type) => type.IsNested;

	private static string DescribeNestedType(Type type)
		=> ResultTexts.Describe(type.IsNested, "was nested", "was non-nested", type);
}
