using System;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatGuid
{
	[GenerateExpectation("Is{Not}Empty", "is {not} empty",
		NullableOverload = false)]
	private static bool IsEmptyGuid(Guid value) => value == Guid.Empty;
}
