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

		string? parseError;
		try
		{
			using JsonDocument jsonDocument =
				await JsonDocument.ParseAsync(
#if NETSTANDARD2_0
					await httpContent.ReadAsStreamAsync(),
#else
					await httpContent.ReadAsStreamAsync(cancellationToken),
#endif
					new JsonDocumentOptions
					{
						AllowTrailingCommas = true,
					},
					cancellationToken);
			string? prettifiedJson = JsonSerializer.Serialize(jsonDocument, SerializerOptions);

			messageBuilder.AppendLine(prettifiedJson.Indent(indentation));
			return true;
		}
		catch (JsonException e)
		{
			parseError = e.Message;
		}

#if NETSTANDARD2_0
		string stringContent = await httpContent.ReadAsStringAsync();
#else
		string stringContent = await httpContent.ReadAsStringAsync(cancellationToken);
#endif
		messageBuilder.AppendLine(stringContent.Indent(indentation));
		if (parseError != null)
		{
			messageBuilder.Append(indentation).AppendLine($"*** JSON parse error: {parseError} ***");
		}

		return true;
	}

	private static bool IsSupportedMediaType(string mediaType)
		=> mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
		   mediaType.Equals("application/problem+json", StringComparison.OrdinalIgnoreCase);
}
