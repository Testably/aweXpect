#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Helpers;

namespace aweXpect;

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

		AppendHeaders(messageBuilder, response.Headers, indentation);
		await AppendContent(messageBuilder, response.Content, indentation, cancellationToken);

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

			AppendHeaders(messageBuilder, request.Headers, indentation);
			if (request.Content != null)
			{
				await AppendContent(messageBuilder, request.Content, indentation + indentation,
					cancellationToken);
			}
		}

		return messageBuilder.ToString().TrimEnd();
	}

	private static async Task AppendContent(StringBuilder messageBuilder,
		HttpContent content,
		string indentation,
		CancellationToken cancellationToken)
	{
		if (content is StringContent or FormUrlEncodedContent)
		{
			string stringContent = await content.ReadAsStringAsync(cancellationToken);
			messageBuilder.AppendLine(stringContent.Indent(indentation));
		}
		else
		{
			messageBuilder.Append(indentation).AppendLine("Content is binary");
		}
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
				messageBuilder.Append(indentation).Append(indentation)
					.Append(header.Key).Append(": ").AppendLine(headerValue);
			}
		}
	}
}
#endif
