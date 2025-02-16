using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	private static HttpResponseBuilder ResponseBuilder => new();

	/// <summary>
	///     Status codes indicating a client error (4xx)
	/// </summary>
	public static TheoryData<HttpStatusCode> ClientErrorStatusCodes()
		=>
		[
			HttpStatusCode.BadRequest,
			HttpStatusCode.Unauthorized,
			HttpStatusCode.PaymentRequired,
			HttpStatusCode.Forbidden,
			HttpStatusCode.NotFound,
			HttpStatusCode.MethodNotAllowed,
			HttpStatusCode.NotAcceptable,
			HttpStatusCode.ProxyAuthenticationRequired,
			HttpStatusCode.RequestTimeout,
			HttpStatusCode.Conflict,
			HttpStatusCode.Gone,
			HttpStatusCode.LengthRequired,
			HttpStatusCode.PreconditionFailed,
			HttpStatusCode.RequestEntityTooLarge,
			HttpStatusCode.RequestUriTooLong,
			HttpStatusCode.UnsupportedMediaType,
			HttpStatusCode.RequestedRangeNotSatisfiable,
			HttpStatusCode.ExpectationFailed,
		];

	/// <summary>
	///     Status codes indicating a redirect (3xx)
	/// </summary>
	public static TheoryData<HttpStatusCode> RedirectStatusCodes()
		=>
		[
			HttpStatusCode.MultipleChoices,
			HttpStatusCode.MovedPermanently,
			HttpStatusCode.Redirect,
			HttpStatusCode.SeeOther,
			HttpStatusCode.NotModified,
			HttpStatusCode.UseProxy,
			HttpStatusCode.Unused,
			HttpStatusCode.TemporaryRedirect,
		];

	/// <summary>
	///     Status codes indicating a server error (5xx)
	/// </summary>
	public static TheoryData<HttpStatusCode> ServerErrorStatusCodes()
		=>
		[
			HttpStatusCode.InternalServerError,
			HttpStatusCode.NotImplemented,
			HttpStatusCode.BadGateway,
			HttpStatusCode.ServiceUnavailable,
			HttpStatusCode.GatewayTimeout,
			HttpStatusCode.HttpVersionNotSupported,
		];

	/// <summary>
	///     Status codes indicating success (2xx)
	/// </summary>
	public static TheoryData<HttpStatusCode> SuccessStatusCodes()
		=>
		[
			HttpStatusCode.OK,
			HttpStatusCode.Created,
			HttpStatusCode.Accepted,
			HttpStatusCode.NonAuthoritativeInformation,
			HttpStatusCode.NoContent,
			HttpStatusCode.ResetContent,
			HttpStatusCode.PartialContent,
		];

	private sealed class HttpResponseBuilder
	{
		private HttpContent? _content;
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
}
