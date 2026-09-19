using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class QuantifierHelpers
{
	public static string ToNegatedString(this Quantifier quantifier)
	{
		quantifier.Negate();
		string result = quantifier.ToString();
		quantifier.Negate();
		return result;
	}

	/// <summary>
	///     The expectation of a "does not contain" constraint on the <paramref name="expected" /> text, which reads
	///     positively once the constraint is negated again.
	/// </summary>
	public static string ToDoesNotContainExpectation(this Quantifier quantifier, ExpectationGrammars grammars,
		string expected)
	{
		if (quantifier.IsNever)
		{
			return $"{grammars.Verb("does not contain", "do not contain")} {expected}";
		}

		return quantifier.IsNegated
			? $"{grammars.Verb("does not contain", "do not contain")} {expected} {quantifier.ToNegatedString()}"
			: $"{grammars.Verb("contains", "contain")} {expected} {quantifier}";
	}
}
