using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}ADelegate", "is {not} a delegate",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is a delegate.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not a delegate.")]
	private static bool IsADelegateType(Type type) => type.IsDelegate();
}
