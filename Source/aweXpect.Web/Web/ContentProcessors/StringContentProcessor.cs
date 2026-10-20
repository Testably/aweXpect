using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Helpers;

namespace aweXpect.Web.ContentProcessors;

/// <summary>
///     Handles string contents.
/// </summary>
public class StringContentProcessor : IContentProcessor
{
	/// <inheritdoc cref="IContentProcessor.AppendContentInfo(StringBuilder, HttpContent, string, CancellationToken)" />
	public async Task<bool> AppendContentInfo(
		StringBuilder messageBuilder,
		HttpContent httpContent,
		string indentation,
		CancellationToken cancellationToken = default)
	{
		if (httpContent.IsNullOrDisposed())
		{
			return false;
		}

		httpContent.TryGetMediaType(out string? mediaType);
		if (mediaType == null || !IsSupportedMediaType(mediaType))
		{
			return false;
		}

#if NETSTANDARD2_0
		string stringContent = await httpContent.ReadAsStringAsync();
#else
		string stringContent = await httpContent.ReadAsStringAsync(cancellationToken);
#endif
		messageBuilder.AppendLine(stringContent.Indent(indentation));

		return true;
	}

	private static bool IsSupportedMediaType(string mediaType)
		=> mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) ||
		   mediaType.Contains("xml") ||
		   mediaType.Contains("json") ||
		   mediaType.Contains("html");
}
