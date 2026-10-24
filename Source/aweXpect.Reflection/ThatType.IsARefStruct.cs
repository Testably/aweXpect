using System;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;

namespace aweXpect;

public static partial class ThatType
{
	[GenerateExpectation("Is{Not}ARefStruct", "is {not} a ref struct",
		Result = nameof(DescribeTypeKind),
		Summary = """
		          Verifies that the <see cref="Type" /> is a ref struct
		          (a stack-only, <c>IsByRefLike</c> value type).
		          """,
		NegatedSummary = "Verifies that the <see cref=\"Type\" /> is not a ref struct.")]
	private static bool IsARefStructType(Type type) => type.IsRefStruct();
}
