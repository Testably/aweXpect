using System;
using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

internal static class StringBuilderExtensions
{
	public static void ItWasNull(this StringBuilder stringBuilder, string it)
		=> stringBuilder.Append(it).Append(" was <null>");

	public static void ItWasNull(this StringBuilder stringBuilder, string it, ExpectationGrammars grammars)
		=> stringBuilder.Append(it).Append(grammars.SubjectVerb(it, " was", " were")).Append(" <null>");

	public static void ItDidNotFinishWithin(this StringBuilder stringBuilder, string it, TimeSpan timeout)
	{
		stringBuilder.Append(it).Append(" did not finish within ");
		Formatter.Format(stringBuilder, timeout);
	}

	/// <summary>
	///     Appends the <paramref name="separator" /> followed by the expectation of the <paramref name="right" /> result.
	/// </summary>
	/// <remarks>
	///     A trailing <c>which</c> in the <paramref name="separator" /> is dropped, when the expectation of the
	///     <paramref name="right" /> result starts with its own <c>whose</c>, to avoid rendering <c>which whose</c>.
	/// </remarks>
	public static void AppendSeparatedExpectation(this StringBuilder stringBuilder, string separator,
		ConstraintResult right)
		=> stringBuilder.AppendSeparatedExpectation(separator, sb => right.AppendExpectation(sb));

	/// <summary>
	///     Appends the <paramref name="separator" /> followed by the expectation written by
	///     <paramref name="appendRight" />.
	/// </summary>
	/// <remarks>
	///     A trailing <c>which</c> in the <paramref name="separator" /> is dropped, when the appended expectation starts
	///     with its own <c>whose</c>, to avoid rendering <c>which whose</c>.
	/// </remarks>
	public static void AppendSeparatedExpectation(this StringBuilder stringBuilder, string separator,
		Action<StringBuilder> appendRight)
	{
		const string which = "which ";
		const string whose = "whose ";
		if (!separator.EndsWith(which, StringComparison.Ordinal))
		{
			stringBuilder.Append(separator);
			appendRight(stringBuilder);
			return;
		}

		// The right expectation is rendered after the separator, because nested quantifiers inspect the preceding text.
		StringBuilder rightExpectation = new(separator);
		appendRight(rightExpectation);
		string text = rightExpectation.ToString();
		if (text.StartsWith(separator, StringComparison.Ordinal) &&
		    string.CompareOrdinal(text, separator.Length, whose, 0, whose.Length) == 0)
		{
			stringBuilder.Append(separator, 0, separator.Length - which.Length);
			stringBuilder.Append(text, separator.Length, text.Length - separator.Length);
		}
		else
		{
			stringBuilder.Append(text);
		}
	}
}
