using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableGuid
{
	[GenerateExpectation("Is{Not}Empty", "is {not} empty",
		NegatedRemarks = "<see langword=\"null\" /> is neither treated as empty nor as not empty, so it fails.")]
	private static bool IsEmptyGuid([DisallowNull] Guid? value) => value.Value == Guid.Empty;
}
