using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}AnException", "is {not} an exception",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is an exception.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not an exception.")]
	private static bool IsAnExceptionType(Type type) => type.IsException();
}
