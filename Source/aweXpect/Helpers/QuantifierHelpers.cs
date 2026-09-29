using System;
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class QuantifierHelpers
{
	/// <summary>
	///     The expectation of a "contains" constraint on the <paramref name="expected" /> text.
	/// </summary>
	/// <remarks>
	///     A negated quantifier without a complement is rendered as <c>not …</c> (e.g. <c>not exactly twice</c>), so the
	///     negation is moved to the verb: <c>does not contain "a" exactly twice</c>.
	/// </remarks>
	public static string ToContainsExpectation(this Quantifier quantifier, ExpectationGrammars grammars,
		string expected)
	{
		const string not = "not ";
		if (quantifier.IsNever)
		{
			return $"{grammars.Verb("does not contain", "do not contain")} {expected}";
		}

		string quantifierText = quantifier.ToString();
		if (quantifier.IsNegated && quantifierText.StartsWith(not, StringComparison.Ordinal))
		{
			return
				$"{grammars.Verb("does not contain", "do not contain")} {expected} {quantifierText.Substring(not.Length)}";
		}

		return $"{grammars.Verb("contains", "contain")} {expected} {quantifierText}";
	}
}
