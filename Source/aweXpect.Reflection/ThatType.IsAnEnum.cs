using System;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}AnEnum", "is {not} an enum",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is an enum.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not an enum.")]
	private static bool IsAnEnumType(Type type) => type.IsEnum;
}
