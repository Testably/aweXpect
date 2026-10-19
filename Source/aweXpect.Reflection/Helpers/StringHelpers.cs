using System;
using aweXpect.Core.Extending;

namespace aweXpect.Reflection.Helpers;

internal static class StringHelpers
{
	public static string PrefixIn(this string description)
	{
		if (description.StartsWith("in "))
		{
			return description;
		}

		return "in " + description;
	}

	/// <summary>
	///     Trims the common whitespace of the caller argument <paramref name="expression" /> and keeps an expression
	///     that spans two lines on a single line.
	/// </summary>
	public static string TrimExpression(this string expression)
	{
		string trimmed = expression.TrimCommonWhiteSpace();
		if (trimmed.IndexOf('\n') == trimmed.LastIndexOf('\n'))
		{
			return trimmed.Replace(Environment.NewLine, " ");
		}

		return trimmed;
	}
}
