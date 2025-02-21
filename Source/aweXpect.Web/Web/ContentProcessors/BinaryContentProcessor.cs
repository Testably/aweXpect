using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Helpers;

namespace aweXpect.Web.ContentProcessors;

/// <summary>
///     Handles audio, image, video and pdf files.
/// </summary>
public class BinaryContentProcessor : IContentProcessor
{
	/// <inheritdoc cref="IContentProcessor.AppendContentInfo(StringBuilder, HttpContent, string, CancellationToken)" />
	public Task<bool> AppendContentInfo(
		StringBuilder messageBuilder,
		HttpContent httpContent,
		string indentation,
		CancellationToken cancellationToken = default)
	{
		if (httpContent.IsNullOrDisposed() || httpContent is StringContent or FormUrlEncodedContent)
		{
			return Task.FromResult(false);
		}

		string? fileName = httpContent?.Headers?.ContentDisposition?.FileName;
		httpContent.TryGetMediaType(out string? mediaType);
		if (string.IsNullOrEmpty(fileName) &&
		    (mediaType == null || !IsSupportedMediaType(mediaType)) &&
		    httpContent is not ByteArrayContent)
		{
			return Task.FromResult(false);
		}

		httpContent.TryGetContentLength(out long contentLength);
		messageBuilder.Append(indentation).AppendLine($"*Content is binary ({mediaType}) with length {contentLength}*");
		return Task.FromResult(true);
	}

	private static bool IsSupportedMediaType(string mediaType)
		=> mediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
		   mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
		   mediaType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ||
		   mediaType.Contains("pdf");
}
