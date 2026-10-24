using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}ARecord", "is {not} a record",
		Result = nameof(DescribeTypeKind),
		Summary = "Verifies that the <see cref=\"Type\" /> is a record.",
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not a record.")]
	private static bool IsARecordType(Type type) => type.IsRecordClass();
}
