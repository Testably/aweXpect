using System;
using aweXpect.Core.Extending;

namespace aweXpect;

#nullable enable
public static partial class ThatUri
{
	[GenerateExpectation("HasDefaultPort", "has the default port for the used scheme",
		NegatedName = "DoesNotHaveDefaultPort",
		NegatedExpectationText = "does not have the default port for the used scheme",
		Remarks = "<seealso cref=\"Uri.IsDefaultPort\" />",
		NegatedRemarks = "<seealso cref=\"Uri.IsDefaultPort\" />")]
	private static PredicateResult HasDefaultPortOfScheme(Uri uri)
		=> uri.IsAbsoluteUri ? uri.IsDefaultPort : WasRelative;
}
