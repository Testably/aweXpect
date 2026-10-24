using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}AStruct", "is {not} a struct",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is a struct.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not a struct.")]
	private static bool IsAStructType(Type type) => type.IsReallyStruct();
}
