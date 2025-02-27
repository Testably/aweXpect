using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace aweXpect.Web.Tests.TestHelpers;

internal class HttpRequestBuilder
{
	private readonly Dictionary<string, string> _headers = new();
	private readonly Dictionary<string, string[]> _multiHeaders = new();
	private HttpContent? _content;
	private string? _contentType;
	private HttpMethod _method = HttpMethod.Head;
	private string _uri = "https://aweXpect.com";

	/// <summary>
	///     Implicitly converts the <paramref name="builder" /> to a <see cref="HttpResponseMessage" />.
	/// </summary>
	public static implicit operator HttpRequestMessage(HttpRequestBuilder builder)
	{
		return builder.Build();
	}

	public HttpRequestBuilder WithMethod(HttpMethod method)
	{
		_method = method;
		return this;
	}

	public HttpRequestBuilder WithUri(string uri)
	{
		_uri = uri;
		return this;
	}

	public HttpRequestBuilder WithContent(string content)
	{
		_content = new StringContent(content);
		return this;
	}

	public HttpRequestBuilder WithHeader(string name, string value)
	{
		_headers.Add(name, value);
		return this;
	}

	public HttpRequestBuilder WithHeaders(string name, params string[] values)
	{
		_multiHeaders.Add(name, values);
		return this;
	}

	public HttpRequestBuilder WithContentType(string mediaType)
	{
		_contentType = mediaType;
		return this;
	}

	private HttpRequestMessage Build()
	{
		HttpRequestMessage httpRequestMessage = new();
		httpRequestMessage.Method = _method;
		httpRequestMessage.RequestUri = new Uri(_uri);
		httpRequestMessage.Content = _content;
		if (!string.IsNullOrEmpty(_contentType) && httpRequestMessage.Content != null)
		{
			httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
		}

		foreach (KeyValuePair<string, string> header in _headers)
		{
			httpRequestMessage.Headers.Add(header.Key, header.Value);
		}

		foreach (KeyValuePair<string, string[]> header in _multiHeaders)
		{
			httpRequestMessage.Headers.Add(header.Key, header.Value);
		}

		return httpRequestMessage;
	}
}
