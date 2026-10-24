using System;
using aweXpect.Core.Extending;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	[GenerateExpectation("Is{Not}File", "is {not} a file URI",
		Remarks = "<seealso cref=\"Uri.IsFile\" />",
		NegatedRemarks = "<seealso cref=\"Uri.IsFile\" />")]
	private static PredicateResult IsFileUri(Uri uri)
		=> uri.IsAbsoluteUri ? uri.IsFile : WasRelative;
}
