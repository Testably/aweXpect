using System;
using System.Diagnostics.CodeAnalysis;
using aweXpect.Core.Extending;

namespace aweXpect.Helpers;

internal static class StringExtensions
{
	[return: NotNullIfNotNull(nameof(value))]
	public static string? Indent(this string? value, string? indentation = "  ",
		bool indentFirstLine = true)
	{
		if (value == null || string.IsNullOrEmpty(indentation))
		{
			return value;
		}

		return (indentFirstLine ? indentation : "")
		       + value.Replace("\n", $"\n{indentation}");
	}

	/// <summary>
	///     Returns the indefinite article ("a" or "an") for the <paramref name="headerName" />.
	/// </summary>
	/// <remarks>
	///     A single leading letter, e.g. in "X-Request-Id", is read by its name ("an X-Request-Id").
	/// </remarks>
	public static string IndefiniteArticleForHeader(this string headerName)
	{
		string word = headerName.Length > 0 && char.IsLetter(headerName[0]) &&
		              (headerName.Length == 1 || headerName[1] == '-')
			? char.ToUpperInvariant(headerName[0]).ToString()
			: headerName;
		return word.PrependAOrAn().StartsWith("an ", StringComparison.Ordinal) ? "an" : "a";
	}
}
