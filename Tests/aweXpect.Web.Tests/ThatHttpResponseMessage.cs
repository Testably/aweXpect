using System.Net;
using aweXpect.Web.Tests.TestHelpers;

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
}
