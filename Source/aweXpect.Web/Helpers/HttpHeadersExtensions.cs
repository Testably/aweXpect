using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace aweXpect.Helpers;

internal static class HttpHeadersExtensions
{
	/// <summary>
	///     Returns the values of the header with the <paramref name="name" /> in the <paramref name="headers" /> of a
	///     message followed by those in the headers of its <paramref name="content" />, or <see langword="null" /> when
	///     neither has the header.
	/// </summary>
	/// <remarks>
	///     The content length is read first, as the content headers only contain a computed <c>Content-Length</c> after
	///     it was read. <see cref="HttpClient" /> never sets a header in both collections; should it happen, the values
	///     of both are returned, as they would both be sent.
	/// </remarks>
	public static string[]? GetHeaderValues(this HttpHeaders headers, HttpContent? content, string name)
	{
		List<string>? values = null;
		if (headers.TryGetValues(name, out IEnumerable<string>? messageValues))
		{
			values = [..messageValues,];
		}

		if (content != null)
		{
			content.TryGetContentLength(out _);
			if (content.Headers.TryGetValues(name, out IEnumerable<string>? contentValues))
			{
				values ??= [];
				values.AddRange(contentValues);
			}
		}

		return values?.ToArray();
	}
}
