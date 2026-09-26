using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class QuantifierHelpers
{
	/// <summary>
	///     The expectation of a "contains" constraint on the <paramref name="expected" /> text.
	/// </summary>
	public static string ToContainsExpectation(this Quantifier quantifier, ExpectationGrammars grammars,
		string expected)
	{
		if (quantifier.IsNever)
		{
			return $"{grammars.Verb("does not contain", "do not contain")} {expected}";
		}

		return $"{grammars.Verb("contains", "contain")} {expected} {quantifier}";
	}
}
