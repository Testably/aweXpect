using System;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Options;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Counts and describes how often a <see langword="string" /> occurs in a <see langword="string" /> subject, for the
///     expectations on a <see langword="string" /> and on a <see langword="string" /> property.
/// </summary>
internal static class StringOccurrences
{
	/// <summary>
	///     Counts the occurrences of the <paramref name="expected" /> value in the <paramref name="actual" /> value until
	///     no further occurrence could change the outcome of the <paramref name="quantifier" />.
	/// </summary>
	/// <remarks>
	///     The count is not <c>IsFinished</c> when the search stopped early, so that more occurrences may follow.
	/// </remarks>
	public static async ValueTask<(int Count, bool IsFinished)> Count(StringEqualityOptions options, string actual,
		string expected, Quantifier quantifier)
	{
		int decisiveAmount = GetDecisiveAmount(quantifier);
		int count = await options.CountOccurrences(actual, expected, decisiveAmount);
		return (count, count < decisiveAmount);
	}

	/// <summary>
	///     Appends the expectation, e.g. <c>contains "a" at least twice</c>, with the <paramref name="verb" /> or, when
	///     the negation of the <paramref name="quantifier" /> reads better on the verb, the <paramref name="negatedVerb" />.
	/// </summary>
	/// <remarks>
	///     A negated quantifier without a complement is rendered as <c>not …</c> (e.g. <c>not exactly twice</c>), so the
	///     negation is moved to the verb: <c>does not contain "a" exactly twice</c>.
	/// </remarks>
	public static void AppendExpectation(StringBuilder stringBuilder, Quantifier quantifier, bool isNegated,
		string verb, string negatedVerb, string expected, StringEqualityOptions options)
	{
		const string not = "not ";
		if (quantifier.IsNever(isNegated))
		{
			stringBuilder.Append(negatedVerb).Append(' ');
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
			return;
		}

		string quantifierText = quantifier.ToString(isNegated);
		bool negatesVerb = isNegated && quantifierText.StartsWith(not, StringComparison.Ordinal);
		stringBuilder.Append(negatesVerb ? negatedVerb : verb).Append(' ');
		Formatter.Format(stringBuilder, expected);
		stringBuilder.Append(options).Append(' ')
			.Append(negatesVerb ? quantifierText.Substring(not.Length) : quantifierText);
	}

	/// <summary>
	///     Appends how often the <paramref name="subject" /> contained the <paramref name="expected" /> value, e.g.
	///     <c>it contained "a" once in "abc"</c>.
	/// </summary>
	public static void AppendResult(StringBuilder stringBuilder, string subject, string expected, string actual,
		int count, bool isFinished, StringEqualityOptions options, string? indentation)
	{
		if (count == 0)
		{
			stringBuilder.Append(subject).Append(" did not contain ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(" in ");
		}
		else
		{
			stringBuilder.Append(subject).Append(" contained ");
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(isFinished ? " " : " at least ");
			CountBounds.AppendOccurrences(stringBuilder, count);
			stringBuilder.Append(" in ");
		}

		Formatter.Format(stringBuilder, actual);
		stringBuilder.Append(options.GetOccurrenceDetails(actual, expected, count).Indent(indentation, false));
	}

	/// <summary>
	///     The number of occurrences from which on no further occurrence can change the outcome, so that the rest of
	///     the subject does not have to be searched.
	/// </summary>
	/// <remarks>
	///     At an upper bound that includes the <see cref="Quantifier.DeterminableAmount" />, only one more occurrence
	///     decides the outcome. The options validate the expected value while they count, so at least one occurrence
	///     is searched for.
	/// </remarks>
	private static int GetDecisiveAmount(Quantifier quantifier)
	{
		int amount = quantifier.DeterminableAmount;
		if (quantifier.Check(amount, false) is null && amount < int.MaxValue)
		{
			return amount + 1;
		}

		return Math.Max(amount, 1);
	}
}
