using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Helpers;

internal static class HttpContentExtensions
{
	public static bool IsNullOrDisposed(this HttpContent? content)
	{
		if (content == null)
		{
			return true;
		}

		try
		{
			_ = content.Headers.ContentLength;
			return false;
		}
		catch (ObjectDisposedException)
		{
			return true;
		}
	}

	/// <remarks>
	///     The content of a <see cref="HttpResponseMessage" /> is <see langword="null" /> on .NET Framework until it is set,
	///     whereas .NET 5+ uses an empty content, so a missing content is read as the empty string on every framework.
	///     A missing content of a <see cref="HttpRequestMessage" /> is read the same way, as HTTP does not distinguish
	///     an absent body from an empty one.
	/// </remarks>
	public static Task<string> ReadAsStringOrEmptyAsync(this HttpContent? content,
		CancellationToken cancellationToken)
	{
		if (content == null)
		{
			return Task.FromResult("");
		}

#if NETSTANDARD2_0
		return content.ReadAsStringAsync();
#else
		return content.ReadAsStringAsync(cancellationToken);
#endif
	}

	public static bool TryGetContentLength(this HttpContent? content, out long length)
	{
		try
		{
			long? contentLength = content?.Headers.ContentLength;
			length = contentLength ?? 0;
			return contentLength != null;
		}
		catch (Exception)
		{
			length = 0;
			return false;
		}
	}

	public static bool TryGetMediaType(this HttpContent? content, [NotNullWhen(true)] out string? contentType)
	{
		contentType = content?.Headers.ContentType?.MediaType;
		return contentType != null;
	}
}
