using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class CombinedTests
	{
		[Fact]
		public async Task WhenAllItemsFailSeveralExpectations_ShouldShowResponseOfItemOnce()
		{
			HttpResponseMessage[] subject =
			[
				ResponseBuilder.WithStatusCode(HttpStatusCode.NotFound).WithHeader("x-a", "1"),
				ResponseBuilder.WithStatusCode(HttpStatusCode.BadRequest).WithHeader("x-a", "2"),
			];

			async Task Act()
				=> await That(subject).All().ComplyWith(response
					=> response.HasHeader("x-b").And.HasStatusCode().Success());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has an `x-b` header and has a success status code (2xx) for all items,
				             but none of 2 did
				             *
				             HTTP-Response (item [0]):
				               404 NotFound HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """).AsWildcard();
		}

		[Fact]
		public async Task WhenDoesNotHaveHeaderAndHasStatusCodeFail_ShouldShowResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).DoesNotHaveHeader("x-a").And.HasStatusCode().Success();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not have an `x-a` header and has a success status code (2xx),
				             but it did contain the `x-a` header: ["1"] and had status code 404 NotFound

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}

		[Fact]
		public async Task WhenHasContentAndHasStatusCodeFail_ShouldShowResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithContent("foo");

			async Task Act()
				=> await That(subject).HasContent(content => content.IsEqualTo("bar"))
					.And.HasContent("baz")
					.And.HasStatusCode().Success();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a string content which is equal to "bar" and has a string content equal to "baz" and has a success status code (2xx),
				             but the string content was "foo", which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "bar"
				                ↑ (expected)
				             and it was "foo", which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "baz"
				                ↑ (expected)
				             and it had status code 404 NotFound

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 3
				               foo
				             """);
		}

		[Fact]
		public async Task WhenHasHeaderAndHasStatusCodeFail_ShouldShowResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).HasHeader("x-a").WhoseValue(value => value.IsEqualTo("2"))
					.And.HasStatusCode().EqualTo(HttpStatusCode.Created);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has an `x-a` header whose value is equal to "2" and has status code 201 Created,
				             but the value was "1", which differs at index 0:
				                ↓ (actual)
				               "1"
				               "2"
				                ↑ (expected)
				             and it had status code 200 OK

				             HTTP-Response:
				               200 OK HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}

		[Fact]
		public async Task WhenHasRequestMessageAndHasHeaderFail_ShouldShowRequestAndResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithRequest(HttpMethod.Get, "https://www.awexpect.com")
				.WithRequestContent("foo");

			async Task Act()
				=> await That(subject).HasRequestMessage(request => request.HasHeader("y")).And.HasHeader("x");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a request message which has a `y` header and has an `x` header,
				             but it did not contain the expected header

				             HTTP-Request:
				               GET https://www.awexpect.com/ HTTP/1.1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 3
				               foo

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}

		[Fact]
		public async Task WhenHasRequestMessageAndHasStatusCodeFail_ShouldShowRequestAndResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithRequest(HttpMethod.Get, "https://www.awexpect.com")
				.WithRequestContent("foo");

			async Task Act()
				=> await That(subject).HasRequestMessage(request => request.HasMethod(HttpMethod.Post))
					.And.HasStatusCode().Success();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a request message which has a POST method and has a success status code (2xx),
				             but it was GET and had status code 404 NotFound

				             HTTP-Request:
				               GET https://www.awexpect.com/ HTTP/1.1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 3
				               foo

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}

		[Fact]
		public async Task WhenHasStatusCodeAndHasContentTypeFail_ShouldShowResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).HasStatusCode().Success().And.HasContentType("application/json");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a success status code (2xx) and has a `Content-Type` header equal to "application/json",
				             but it had status code 404 NotFound and was "text/plain", which differs at index 0:
				                ↓ (actual)
				               "text/plain"
				               "application/json"
				                ↑ (expected)

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}

		[Fact]
		public async Task WhenHasStatusCodeAndHasHeaderFail_ShouldShowResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).HasStatusCode().Success().And.HasHeader("x-missing");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a success status code (2xx) and has an `x-missing` header,
				             but it had status code 404 NotFound and did not contain the expected header

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}

		[Fact]
		public async Task WhenHasStatusCodeOrHasHeaderFail_ShouldShowResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).HasStatusCode().Success().Or.HasHeader("x-missing");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a success status code (2xx) or has an `x-missing` header,
				             but it had status code 404 NotFound and did not contain the expected header

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}

		[Fact]
		public async Task WhenOnlyOtherExpectationOnResponseFails_ShouldShowResponse()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).HasStatusCode().Success()
					.And.Satisfies(response => response?.Headers.Contains("x-b") == true);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a success status code (2xx) and satisfies response => response?.Headers.Contains("x-b") == true,
				             but it was *

				             HTTP-Response:
				               200 OK HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """).AsWildcard();
		}

		[Fact]
		public async Task WhenTwoStatusCodeExpectationsFail_ShouldShowResponseOnce()
		{
			HttpResponseMessage subject = ResponseBuilder
				.WithStatusCode(HttpStatusCode.NotFound)
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.OK).And.HasStatusCode().Success();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has status code 200 OK and has a success status code (2xx),
				             but it had status code 404 NotFound

				             HTTP-Response:
				               404 NotFound HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 0
				             """);
		}
	}
}
