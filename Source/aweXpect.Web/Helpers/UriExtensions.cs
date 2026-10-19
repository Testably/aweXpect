using System;
using System.Collections.Generic;
using System.Net;

namespace aweXpect.Helpers;

internal static class UriExtensions
{
	/// <summary>
	///     Returns the URL-decoded values of all query parameters of the absolute <paramref name="uri" /> with the
	///     <paramref name="name" /> in the order in which they appear, or <see langword="null" /> when there is none.
	/// </summary>
	public static string[]? GetQueryParameterValues(this Uri uri, string name)
	{
		List<string>? values = null;
		foreach (string pair in uri.Query.TrimStart('?').Split('&'))
		{
			string[] parts = pair.Split(new[] { '=', }, 2);
			string key = WebUtility.UrlDecode(parts[0]);
			if (string.IsNullOrWhiteSpace(key) || !string.Equals(key, name, StringComparison.Ordinal))
			{
				continue;
			}

			values ??= [];
			values.Add(parts.Length == 2 ? WebUtility.UrlDecode(parts[1]) : "");
		}

		return values?.ToArray();
	}
}
