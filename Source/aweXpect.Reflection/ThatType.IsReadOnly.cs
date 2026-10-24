using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}ReadOnly", "is {not} read-only",
		Result = nameof(DescribeReadOnlyType),
		Summary = """
		          Verifies that the <see cref="Type" /> is a read-only struct
		          (declared with the <c>readonly</c> modifier).
		          """,
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not a read-only struct.")]
	private static bool IsReadOnlyType(Type type) => type.IsReadOnlyStruct();

	private static string DescribeReadOnlyType(Type type)
		=> ResultTexts.Describe(type.IsReadOnlyStruct(), "was read-only", "was not read-only", type);
}
