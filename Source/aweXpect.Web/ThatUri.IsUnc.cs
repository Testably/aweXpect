using System;
using aweXpect.Core.Extending;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	[GenerateExpectation("Is{Not}Unc", "is {not} a UNC path",
		Remarks = "<seealso cref=\"Uri.IsUnc\" />",
		NegatedRemarks = "<seealso cref=\"Uri.IsUnc\" />")]
	private static PredicateResult IsUncPath(Uri uri)
		=> uri.IsAbsoluteUri ? uri.IsUnc : WasRelative;
}
