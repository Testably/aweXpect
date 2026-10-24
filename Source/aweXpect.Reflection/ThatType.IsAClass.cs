using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}AClass", "is {not} a class",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is a class.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not a class.")]
	private static bool IsAClassType(Type type) => type.IsReallyClass();
}
