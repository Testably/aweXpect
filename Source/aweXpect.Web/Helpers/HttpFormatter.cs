using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;

namespace aweXpect.Helpers;

internal static class HttpFormatter
{
	public static async Task<string> Format(
		HttpRequestMessage? request,
		string indentation,
		CancellationToken cancellationToken)
	{
		if (request == null)
		{
			return "<null>";
		}

		StringBuilder messageBuilder = new();

		messageBuilder.Append(indentation)
			.Append(request.Method.ToString().ToUpper()).Append(' ').Append(request.RequestUri)
			.Append(" HTTP/").Append(request.Version)
			.AppendLine();

		AppendHeaders(messageBuilder, request.Headers, indentation + indentation);
		if (request.Content != null)
		{
			await AppendContentWithHeaders(messageBuilder, request.Content, indentation, cancellationToken);
		}

		return messageBuilder.ToString().TrimEnd();
	}

	public static async Task<string> Format(
		HttpResponseMessage? response,
		string indentation,
		CancellationToken cancellationToken)
	{
		if (response == null)
		{
			return "<null>";
		}

		StringBuilder messageBuilder = new();

		messageBuilder.Append(indentation)
			.Append((int)response.StatusCode).Append(' ').Append(response.StatusCode)
			.Append(" HTTP/").Append(response.Version)
			.AppendLine();

		AppendHeaders(messageBuilder, response.Headers, indentation + indentation);
		if (response.Content != null)
		{
			await AppendContentWithHeaders(messageBuilder, response.Content, indentation, cancellationToken);
		}

		return messageBuilder.ToString().TrimEnd();
	}

	private static async Task AppendContent(IContentProcessor[] contentProcessors, StringBuilder messageBuilder,
		HttpContent httpContent,
		string indentation,
		CancellationToken cancellationToken)
	{
		foreach (IContentProcessor? contentProcessor in contentProcessors)
		{
			if (await contentProcessor.AppendContentInfo(messageBuilder, httpContent, indentation, cancellationToken))
			{
				return;
			}
		}

		httpContent.TryGetMediaType(out string? contentType);
		string length = httpContent.TryGetContentLength(out long contentLength)
			? $"with length {contentLength}"
			: "with unknown length";
		messageBuilder.Append(indentation)
			.AppendLine(contentType == null
				? $"*Content {length}*"
				: $"*Content ({contentType}) {length}*");
	}

	/// <remarks>
	///     Reading the content length adds the computed <c>Content-Length</c> header, and processing the content can buffer
	///     it, which makes a previously unknown length available. Both are therefore done before the headers are listed,
	///     so that formatting the same content again results in the same text.
	/// </remarks>
	private static async Task AppendContentWithHeaders(
		StringBuilder messageBuilder,
		HttpContent content,
		string indentation,
		CancellationToken cancellationToken)
	{
		IContentProcessor[] contentProcessors = Customize.aweXpect.Web().ContentProcessors.Get();

		StringBuilder contentBuilder = new();
		await AppendContent(contentProcessors, contentBuilder, content, indentation, cancellationToken);
		content.TryGetContentLength(out _);
		AppendHeaders(messageBuilder, content.Headers, indentation + indentation);
		messageBuilder.Append(contentBuilder);
	}

	private static void AppendHeaders(
		StringBuilder messageBuilder,
		HttpHeaders headers,
		string indentation)
	{
		foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
		{
			foreach (string headerValue in header.Value)
			{
				messageBuilder.Append(indentation)
					.Append(header.Key).Append(": ").AppendLine(headerValue);
			}
		}
	}
}
