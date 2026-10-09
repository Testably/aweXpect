using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Helpers;

namespace aweXpect.Web.ContentProcessors;

/// <summary>
///     Handles JSON contents by pretty-printing them.
/// </summary>
public class JsonContentProcessor : IContentProcessor
{
	private static readonly JsonSerializerOptions SerializerOptions = new()
	{
		WriteIndented = true,
	};

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

		// Reading the content as string buffers it, so that it can be read again (the content stream can only be read once).
#if NETSTANDARD2_0
		string stringContent = await httpContent.ReadAsStringAsync();
#else
		string stringContent = await httpContent.ReadAsStringAsync(cancellationToken);
#endif
		try
		{
			using JsonDocument jsonDocument = JsonDocument.Parse(stringContent,
				new JsonDocumentOptions
				{
					AllowTrailingCommas = true,
				});
			string? prettifiedJson = JsonSerializer.Serialize(jsonDocument, SerializerOptions);

			messageBuilder.AppendLine(prettifiedJson.Indent(indentation));
		}
		catch (JsonException e)
		{
			messageBuilder.AppendLine(stringContent.Indent(indentation));
			messageBuilder.Append(indentation).AppendLine($"*** JSON parse error: {e.Message} ***");
		}

		return true;
	}

	private static bool IsSupportedMediaType(string mediaType)
		=> mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
		   mediaType.Equals("application/problem+json", StringComparison.OrdinalIgnoreCase);
}
