using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class MemberGrammars
{
	/// <summary>
	///     The grammars of the expectations on a single member that a prefix ending in "which" introduces, e.g.
	///     "has a string content which has length equal to 5".
	/// </summary>
	/// <remarks>
	///     The nested and plural grammars of a caller do not apply to the member, as they turn <c>HasLength()</c> into
	///     "whose length" or "have length".
	/// </remarks>
	public static ExpectationGrammars AfterWhich(ExpectationGrammars grammars)
		=> grammars & ~(ExpectationGrammars.Nested | ExpectationGrammars.Plural);
}
