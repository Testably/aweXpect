using System;
using aweXpect.Core.Extending;

namespace aweXpect.Helpers;

internal static class StringExtensions
{
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
