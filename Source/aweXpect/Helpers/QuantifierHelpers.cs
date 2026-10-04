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
		string expected, bool isNegated)
	{
		const string not = "not ";
		if (quantifier.IsNever(isNegated))
		{
			return $"{grammars.Verb("does not contain", "do not contain")} {expected}";
		}

		string quantifierText = quantifier.ToString(isNegated);
		if (isNegated && quantifierText.StartsWith(not, StringComparison.Ordinal))
		{
			return
				$"{grammars.Verb("does not contain", "do not contain")} {expected} {quantifierText.Substring(not.Length)}";
		}

		return $"{grammars.Verb("contains", "contain")} {expected} {quantifierText}";
	}

	/// <summary>
	///     Appends a number of occurrences, e.g. <c>once</c>, <c>twice</c> or <c>3 times</c>.
	/// </summary>
	public static StringBuilder AppendOccurrences(this StringBuilder stringBuilder, int count)
		=> count switch
		{
			1 => stringBuilder.Append("once"),
			2 => stringBuilder.Append("twice"),
			_ => stringBuilder.Append(count).Append(" times"),
		};
}
