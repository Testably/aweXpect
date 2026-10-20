using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace aweXpect.Web.Tests.TestHelpers;

internal sealed class HttpResponseBuilder
{
	private readonly Dictionary<string, string> _headers = new();
	private readonly Dictionary<string, string[]> _multiHeaders = new();
	private HttpContent? _content;
	private string? _contentType;
	private HttpRequestBuilder? _requestBuilder;
	private HttpStatusCode _statusCode = HttpStatusCode.OK;

	/// <summary>
	///     Implicitly converts the <paramref name="builder" /> to a <see cref="HttpResponseMessage" />.
	/// </summary>
	public static implicit operator HttpResponseMessage(HttpResponseBuilder builder)
	{
		return builder.Build();
	}

	public HttpResponseBuilder WithContent(string content)
	{
		_content = new StringContent(content);
		return this;
	}

	public HttpResponseBuilder WithContent(HttpContent content)
	{
		_content = content;
		return this;
	}

	public HttpResponseBuilder WithContent(byte[] content)
	{
		_content = new StreamContent(new MemoryStream(content));
		return this;
	}

	public HttpResponseBuilder WithHeader(string name, string value)
	{
		_headers.Add(name, value);
		return this;
	}

	public HttpResponseBuilder WithHeaders(string name, params string[] values)
	{
		_multiHeaders.Add(name, values);
		return this;
	}

	public HttpResponseBuilder WithContentType(string mediaType)
	{
		_contentType = mediaType;
		return this;
	}

	public HttpRequestBuilder WithRequest(HttpMethod method, string uri)
	{
		_requestBuilder = new HttpRequestBuilder(this, method, uri);
		return _requestBuilder;
	}

	public HttpResponseBuilder WithStatusCode(HttpStatusCode statusCode)
	{
		_statusCode = statusCode;
		return this;
	}

	private HttpResponseMessage Build()
	{
		HttpResponseMessage httpResponseMessage = new();
		httpResponseMessage.StatusCode = _statusCode;
		httpResponseMessage.Content = _content ?? new StringContent("");
		if (!string.IsNullOrEmpty(_contentType))
		{
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
		}

		foreach (KeyValuePair<string, string> header in _headers)
		{
			httpResponseMessage.Headers.Add(header.Key, header.Value);
		}

		foreach (KeyValuePair<string, string[]> header in _multiHeaders)
		{
			httpResponseMessage.Headers.Add(header.Key, header.Value);
		}

		if (_requestBuilder != null)
		{
			httpResponseMessage.RequestMessage = _requestBuilder;
		}

		return httpResponseMessage;
	}

	public class HttpRequestBuilder
	{
		private readonly HttpMethod _method;
		private readonly HttpResponseBuilder _responseBuilder;
		private readonly string _uri;
		private HttpContent? _content;

		public HttpRequestBuilder(HttpResponseBuilder responseBuilder, HttpMethod method,
			string uri)
		{
			_responseBuilder = responseBuilder;
			_method = method;
			_uri = uri;
		}

		/// <summary>
		///     Implicitly converts the <paramref name="builder" /> to a <see cref="HttpResponseMessage" />.
		/// </summary>
		public static implicit operator HttpRequestMessage(HttpRequestBuilder builder)
		{
			return builder.Build();
		}

		/// <summary>
		///     Implicitly converts the <paramref name="builder" /> to a <see cref="HttpResponseMessage" />.
		/// </summary>
		public static implicit operator HttpResponseMessage(HttpRequestBuilder builder)
		{
			return builder._responseBuilder.Build();
		}

		public HttpRequestBuilder WithRequestContent(string content)
		{
			_content = new StringContent(content);
			return this;
		}

		private HttpRequestMessage Build()
		{
			HttpRequestMessage httpResponseMessage = new();
			httpResponseMessage.Method = _method;
			httpResponseMessage.RequestUri = new Uri(_uri);
			httpResponseMessage.Content = _content;
			return httpResponseMessage;
		}
	}
}
