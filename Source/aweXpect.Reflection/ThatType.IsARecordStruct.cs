using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}ARecordStruct", "is {not} a record struct",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is a record struct.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not a record struct.")]
	private static bool IsARecordStructType(Type type) => type.IsRecordStruct();
}
