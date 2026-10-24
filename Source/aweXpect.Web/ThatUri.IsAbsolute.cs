using System;
using aweXpect.Core.Extending;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	[GenerateExpectation("Is{Not}Absolute", "is {not} an absolute URI",
		Remarks = "<seealso cref=\"Uri.IsAbsoluteUri\" />",
		NegatedRemarks = "<seealso cref=\"Uri.IsAbsoluteUri\" />")]
	private static bool IsAbsoluteUri(Uri uri)
		=> uri.IsAbsoluteUri;
}
