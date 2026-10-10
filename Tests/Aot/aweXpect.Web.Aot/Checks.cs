using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static aweXpect.Expect;

namespace aweXpect.Web.Aot;

internal static class Checks
{
	private const string Json = """{"name":"aweXpect","tags":["web"]}""";

	private const string PrettyJson = """
	                                    {
	                                      "name": "aweXpect",
	                                      "tags": [
	                                        "web"
	                                      ]
	                                    }
	                                  """;

	private const string ProblemDetails =
		"""{"type":"https://awexpect.com/not-found","title":"Not Found","status":404,"detail":"The track was not found."}""";

	public static readonly Check[] All =
	[
		new("a matching status code passes",
			() => ShouldPass(async () => await That(Response(HttpStatusCode.OK)).HasStatusCode(HttpStatusCode.OK))),
		new("a success status code passes",
			() => ShouldPass(async () => await That(Response(HttpStatusCode.Created)).HasStatusCode().Success())),
		new("a differing status code fails and names both",
			() => ShouldFail(async () => await That(Response(HttpStatusCode.NotFound)).HasStatusCode().Success(),
				"has a success status code (2xx)", "but it had status code 404 NotFound")),
		new("a failing expectation on a response with JSON content writes the pretty-printed content",
			() => ShouldFail(async () => await That(Response(HttpStatusCode.OK, Json)).HasStatusCode(HttpStatusCode.Accepted),
				"HTTP-Response:", "200 OK HTTP/1.1", "Content-Type: application/json", PrettyJson)),
		new("a failing expectation on a request with JSON content writes the pretty-printed content",
			() => ShouldFail(async () => await That(Request(Json)).HasMethod(HttpMethod.Get),
				"has a GET method", "HTTP-Request:", "POST https://awexpect.com/tracks HTTP/1.1", PrettyJson)),
		new("a failing expectation on a response with invalid JSON content writes the raw content and the parse error",
			() => ShouldFail(async () => await That(Response(HttpStatusCode.OK, "{\"name\":1")).HasStatusCode(HttpStatusCode.Accepted),
				"{\"name\":1", "*** JSON parse error:")),
		new("a failing expectation on a response with binary content describes the content",
			() => ShouldFail(async () =>
			{
				HttpResponseMessage response = new(HttpStatusCode.OK)
				{
					Content = new ByteArrayContent([1, 2, 3,]),
				};
				await That(response).HasStatusCode(HttpStatusCode.Accepted);
			}, "*Content is binary () with length 3*")),
		new("a present header with the expected value passes",
			() => ShouldPass(async () => await That(Response(HttpStatusCode.OK, header: "x-track"))
				.HasHeader("x-track").WithValue("42"))),
		new("a missing header fails and is named",
			() => ShouldFail(async () => await That(Response(HttpStatusCode.OK)).HasHeader("x-track"),
				"has an `x-track` header")),
		new("an unexpected header fails and is named",
			() => ShouldFail(async () => await That(Response(HttpStatusCode.OK, header: "x-track")).DoesNotHaveHeader("x-track"),
				"does not have an `x-track` header","[\"42\"]")),
		new("an equal content passes",
			() => ShouldPass(async () => await That(Response(HttpStatusCode.OK, Json)).HasContent(Json))),
		new("a differing content fails and shows both",
			() => ShouldFail(async () => await That(Response(HttpStatusCode.OK, Json)).HasContent("{}"),
				"has a string content equal to \"{}\"", "differs at index 1")),
		new("a matching content type passes",
			() => ShouldPass(async () => await That(Response(HttpStatusCode.OK, Json)).HasContentType("application/json"))),
		new("a request with the expected content and request URI passes",
			() => ShouldPass(async () => await That(Request(Json)).HasContent(Json)
				.And.HasRequestUri("https://awexpect.com/tracks")
				.And.HasMethod(HttpMethod.Post))),
		new("matching problem details pass",
			() => ShouldPass(async () => await That(ProblemDetailsResponse())
				.HasProblemDetailsContent("https://awexpect.com/not-found")
				.WithTitle("Not Found").WithStatus(404).WithDetail("The track was not found."))),
		new("differing problem details fail and name the member",
			() => ShouldFail(async () => await That(ProblemDetailsResponse())
					.HasProblemDetailsContent().WithTitle("Bad Request"),
				"has a ProblemDetails content with any type and title \"Bad Request\"",
				"but it had title \"Not Found\"",
				"\"title\": \"Not Found\"")),
		new("problem details on a non-JSON content fail",
			() => ShouldFail(async () => await That(Response(HttpStatusCode.OK, "foo")).HasProblemDetailsContent(),
				"has a ProblemDetails content")),
		new("a differing request message fails",
			() => ShouldFail(async () =>
			{
				HttpResponseMessage response = Response(HttpStatusCode.OK);
				response.RequestMessage = Request(Json);
				await That(response).HasRequestMessage(request => request.HasMethod(HttpMethod.Get));
			}, "has a request message which has a GET method")),
		new("a null URI fails instead of throwing",
			() => ShouldFail(async () => await That((Uri?)null).IsAbsolute(),
				"is an absolute URI", "but it was <null>")),
	];

	private static HttpResponseMessage Response(HttpStatusCode statusCode, string? json = null, string? header = null)
	{
		HttpResponseMessage response = new(statusCode)
		{
			Content = new StringContent(json ?? "", Encoding.UTF8, json is null ? "text/plain" : "application/json"),
		};
		if (header is not null)
		{
			response.Headers.Add(header, "42");
		}

		return response;
	}

	private static HttpResponseMessage ProblemDetailsResponse()
		=> new(HttpStatusCode.NotFound)
		{
			Content = new StringContent(ProblemDetails, Encoding.UTF8, "application/problem+json"),
		};

	private static HttpRequestMessage Request(string json)
		=> new(HttpMethod.Post, "https://awexpect.com/tracks")
		{
			Content = new StringContent(json, Encoding.UTF8, "application/json"),
		};

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}

	/// <summary>
	///     Expects a <see cref="FailException" /> whose message contains every part, ignoring line endings.
	/// </summary>
	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (FailException exception)
		{
			string message = exception.Message.ReplaceLineEndings("\n");
			string? missing = Array.Find(parts,
				part => !message.Contains(part.ReplaceLineEndings("\n"), StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} instead of {typeof(FailException).FullName}: {exception.Message}";
		}

		return "did not throw";
	}
}
