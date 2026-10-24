using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}AnAttribute", "is {not} an attribute",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is an attribute.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not an attribute.")]
	private static bool IsAnAttributeType(Type type) => type.IsAttribute();
}
