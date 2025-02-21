using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;

namespace aweXpect.Helpers;

internal static class HttpResponseMessageFormatter
{
	public static async Task<string> Format(
		HttpResponseMessage response,
		string indentation,
		CancellationToken cancellationToken)
	{
		StringBuilder messageBuilder = new();

		messageBuilder.Append(indentation)
			.Append("HTTP/").Append(response.Version)
			.Append(' ').Append((int)response.StatusCode).Append(' ')
			.Append(response.StatusCode)
			.AppendLine();

		IContentProcessor[] contentProcessors = Customize.aweXpect.Web().ContentProcessors.Get();

		AppendHeaders(messageBuilder, response.Headers, indentation + indentation);
		AppendHeaders(messageBuilder, response.Content.Headers, indentation + indentation);
		await AppendContent(contentProcessors, messageBuilder, response.Content, indentation, cancellationToken);

		HttpRequestMessage? request = response.RequestMessage;
		if (request == null)
		{
			messageBuilder.Append(indentation).AppendLine("The originating request was <null>");
		}
		else
		{
			messageBuilder.Append(indentation).AppendLine("The originating request was:");
			messageBuilder.Append(indentation).Append(indentation)
				.Append(request.Method.ToString().ToUpper()).Append(' ')
				.Append(request.RequestUri).Append(" HTTP ").Append(request.Version)
				.AppendLine();

			AppendHeaders(messageBuilder, request.Headers, indentation + indentation + indentation);
			if (request.Content != null)
			{
				AppendHeaders(messageBuilder, request.Content.Headers, indentation + indentation + indentation);
				await AppendContent(contentProcessors, messageBuilder, request.Content, indentation + indentation,
					cancellationToken);
			}
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
		httpContent.TryGetContentLength(out long contentLength);
		messageBuilder.Append(indentation)
			.AppendLine(contentType == null
				? $"*Content with length {contentLength}*"
				: $"*Content ({contentType}) with length {contentLength}*");
	}

	private static void AppendHeaders(
		StringBuilder messageBuilder,
		HttpHeaders headers,
		string indentation)
	{
		foreach (KeyValuePair<string, IEnumerable<string>> header in headers
			         .OrderBy(x => x.Key == "Content-Length"))
		{
			foreach (string headerValue in header.Value)
			{
				messageBuilder.Append(indentation)
					.Append(header.Key).Append(": ").AppendLine(headerValue);
			}
		}
	}
}
