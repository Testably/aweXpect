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
		string query = uri.Query;
		List<string>? values = null;
		int start = GetStart(query, name);
		while (TryFindNext(query, name, ref start, out int valueStart, out int valueEnd))
		{
			values ??= [];
			values.Add(Decode(query, valueStart, valueEnd));
		}

		return values?.ToArray();
	}

	/// <summary>
	///     Checks if the absolute <paramref name="uri" /> has a query parameter with the <paramref name="name" />, without
	///     copying its value.
	/// </summary>
	public static bool HasQueryParameter(this Uri uri, string name)
	{
		string query = uri.Query;
		int start = GetStart(query, name);
		return TryFindNext(query, name, ref start, out _, out _);
	}

	/// <summary>
	///     Returns <see langword="true" /> when the absolute <paramref name="uri" /> has exactly one query parameter with
	///     the <paramref name="name" />, with its URL-decoded <paramref name="value" />.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="GetQueryParameterValues" />, it copies no value of a parameter that occurs more than once.
	/// </remarks>
	public static bool TryGetSingleQueryParameterValue(this Uri uri, string name, out string? value)
	{
		string query = uri.Query;
		int start = GetStart(query, name);
		if (!TryFindNext(query, name, ref start, out int valueStart, out int valueEnd) ||
		    TryFindNext(query, name, ref start, out _, out _))
		{
			value = null;
			return false;
		}

		value = Decode(query, valueStart, valueEnd);
		return true;
	}

	/// <summary>
	///     Returns the index of the first parameter in the <paramref name="query" />, or an index after its end when the
	///     <paramref name="name" /> is blank, so that it never matches.
	/// </summary>
	private static int GetStart(string query, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return query.Length + 1;
		}

		int start = 0;
		while (start < query.Length && query[start] == '?')
		{
			start++;
		}

		return start;
	}

	/// <summary>
	///     Finds the next parameter with the <paramref name="name" /> from the <paramref name="start" /> on and returns the
	///     range of its value, which is empty for a parameter without <c>=</c>.
	/// </summary>
	private static bool TryFindNext(string query, string name, ref int start, out int valueStart, out int valueEnd)
	{
		while (start <= query.Length)
		{
			int parameterStart = start;
			int end = query.IndexOf('&', parameterStart);
			if (end < 0)
			{
				end = query.Length;
			}

			start = end + 1;
			int separator = query.IndexOf('=', parameterStart, end - parameterStart);
			if (HasName(query, parameterStart, (separator < 0 ? end : separator) - parameterStart, name))
			{
				valueStart = separator < 0 ? end : separator + 1;
				valueEnd = end;
				return true;
			}
		}

		valueStart = 0;
		valueEnd = 0;
		return false;
	}

	private static string Decode(string query, int valueStart, int valueEnd)
		=> valueStart == valueEnd ? "" : WebUtility.UrlDecode(query.Substring(valueStart, valueEnd - valueStart));

	private static bool HasName(string query, int start, int length, string name)
	{
		if (query.IndexOfAny(EncodingCharacters, start, length) < 0)
		{
			return length == name.Length && string.CompareOrdinal(query, start, name, 0, length) == 0;
		}

		return string.Equals(WebUtility.UrlDecode(query.Substring(start, length)), name, StringComparison.Ordinal);
	}
}
