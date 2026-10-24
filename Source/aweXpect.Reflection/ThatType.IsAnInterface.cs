using System;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}AnInterface", "is {not} an interface",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is an interface.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not an interface.")]
	private static bool IsAnInterfaceType(Type type) => type.IsInterface;
}
