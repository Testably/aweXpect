using System;
using System.Collections.Generic;
using System.Net;

namespace aweXpect.Helpers;

internal static class UriExtensions
{
	private static readonly char[] EncodingCharacters = ['%', '+',];

	/// <summary>
	///     Returns the URL-decoded values of all query parameters of the absolute <paramref name="uri" /> with the
	///     <paramref name="name" /> in the order in which they appear, or <see langword="null" /> when there is none.
	/// </summary>
	/// <remarks>
	///     The query is read in place instead of being split, so that only the values of matching parameters are copied.
	///     A parameter with a blank name is never matched.
	/// </remarks>
	public static string[]? GetQueryParameterValues(this Uri uri, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return null;
		}

		string query = uri.Query;
		List<string>? values = null;
		int start = 0;
		while (start < query.Length && query[start] == '?')
		{
			start++;
		}

		while (start <= query.Length)
		{
			int end = query.IndexOf('&', start);
			if (end < 0)
			{
				end = query.Length;
			}

			int separator = query.IndexOf('=', start, end - start);
			if (HasName(query, start, (separator < 0 ? end : separator) - start, name))
			{
				values ??= [];
				values.Add(separator < 0
					? ""
					: WebUtility.UrlDecode(query.Substring(separator + 1, end - separator - 1)));
			}

			start = end + 1;
		}

		return values?.ToArray();
	}

	private static bool HasName(string query, int start, int length, string name)
	{
		if (query.IndexOfAny(EncodingCharacters, start, length) < 0)
		{
			return length == name.Length && string.CompareOrdinal(query, start, name, 0, length) == 0;
		}

		return string.Equals(WebUtility.UrlDecode(query.Substring(start, length)), name, StringComparison.Ordinal);
	}
}
