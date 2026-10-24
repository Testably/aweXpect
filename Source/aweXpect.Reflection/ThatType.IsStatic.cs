using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}Static", "is {not} static",
		Result = nameof(DescribeStaticType),
		Summary = "Verifies that the <see cref=\"Type\" /> is static.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not static.")]
	private static bool IsStaticType(Type type) => type.IsReallyStatic();

	private static string DescribeStaticType(Type type)
		=> ResultTexts.Describe(type.IsReallyStatic(), "was static", "was non-static", type);
}
