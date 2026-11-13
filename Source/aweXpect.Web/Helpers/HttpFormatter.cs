using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Extending;
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

	/// <remarks>
	///     A custom processor might not check whether the content is disposed (e.g. request content on .NET Framework after
	///     sending) or might throw for other reasons. Disposed content therefore skips the processors, and an exception of a
	///     processor replaces only the content, which keeps the rest of the HTTP section.
	/// </remarks>
	private static async Task AppendContent(IReadOnlyList<IContentProcessor> contentProcessors,
		StringBuilder messageBuilder,
		HttpContent httpContent,
		string indentation,
		CancellationToken cancellationToken)
	{
		if (httpContent.IsNullOrDisposed())
		{
			AppendFallback(messageBuilder, httpContent, indentation, "");
			return;
		}

		int startLength = messageBuilder.Length;
		for (int i = 0; i < contentProcessors.Count; i++)
		{
			IContentProcessor contentProcessor = contentProcessors[i];
			try
			{
				if (await contentProcessor.AppendContentInfo(messageBuilder, httpContent, indentation,
					    cancellationToken))
				{
					return;
				}
			}
			catch (Exception exception) when (!IsCancellation(exception, cancellationToken))
			{
				messageBuilder.Length = startLength;
				AppendFallback(messageBuilder, httpContent, indentation,
					$", but the content processor {Formatter.Format(contentProcessor.GetType())} did throw " +
					$"{Formatter.Format(exception.GetType()).PrependAOrAn()}: {exception.Message}");
				return;
			}
		}

		AppendFallback(messageBuilder, httpContent, indentation, "");
	}

	private static void AppendFallback(StringBuilder messageBuilder,
		HttpContent httpContent,
		string indentation,
		string suffix)
	{
		httpContent.TryGetMediaType(out string? contentType);
		string length = httpContent.TryGetContentLength(out long contentLength)
			? $"with length {contentLength}"
			: "with unknown length";
		messageBuilder.Append(indentation)
			.AppendLine(contentType == null
				? $"*Content {length}{suffix}*"
				: $"*Content ({contentType}) {length}{suffix}*");
	}

	private static bool IsCancellation(Exception exception, CancellationToken cancellationToken)
		=> exception is OperationCanceledException && cancellationToken.IsCancellationRequested;

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
		IReadOnlyList<IContentProcessor> contentProcessors = Customize.aweXpect.Web().ContentProcessors.Get();

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
