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
			IContentProcessor[] contentProcessors = Customize.aweXpect.Web().ContentProcessors.Get();

			AppendHeaders(messageBuilder, request.Content.Headers, indentation + indentation);
			await AppendContent(contentProcessors, messageBuilder, request.Content, indentation,
				cancellationToken);
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

		IContentProcessor[] contentProcessors = Customize.aweXpect.Web().ContentProcessors.Get();

		AppendHeaders(messageBuilder, response.Headers, indentation + indentation);
		AppendHeaders(messageBuilder, response.Content.Headers, indentation + indentation);
		await AppendContent(contentProcessors, messageBuilder, response.Content, indentation, cancellationToken);
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
