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

	/// <summary>
	///     Appends the <paramref name="separator" /> followed by the expectation of the <paramref name="right" /> result.
	/// </summary>
	/// <remarks>
	///     A trailing <c>which</c> in the <paramref name="separator" /> is dropped, when the expectation of the
	///     <paramref name="right" /> result starts with its own <c>whose</c>, to avoid rendering <c>which whose</c>.
	/// </remarks>
	public static void AppendSeparatedExpectation(this StringBuilder stringBuilder, string separator,
		ConstraintResult right)
	{
		const string which = "which ";
		if (!separator.EndsWith(which, StringComparison.Ordinal))
		{
			stringBuilder.Append(separator);
			right.AppendExpectation(stringBuilder);
			return;
		}

		StringBuilder rightExpectation = new();
		right.AppendExpectation(rightExpectation);
		string rightText = rightExpectation.ToString();
		if (rightText.StartsWith("whose ", StringComparison.Ordinal))
		{
			stringBuilder.Append(separator, 0, separator.Length - which.Length);
		}
		else
		{
			stringBuilder.Append(separator);
		}

		stringBuilder.Append(rightText);
	}
}
