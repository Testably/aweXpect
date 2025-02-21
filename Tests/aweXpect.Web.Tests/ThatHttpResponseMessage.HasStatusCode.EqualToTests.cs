using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasStatusCode
	{
		public sealed class EqualToTests
		{
			[Fact]
			public async Task WhenFailing_ShouldIncludeRequestInMessage()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest)
					.WithContent("some content")
					.WithRequest(HttpMethod.Get, "https://example.com")
					.WithRequestContent("request content");

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.OK);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has status code 200 OK,
					             but it had status code 400 BadRequest

					             HTTP-Request:
					               HTTP/1.1 400 BadRequest
					                 Content-Type: text/plain; charset=utf-8
					               some content
					               The originating request was:
					                 GET https://example.com/ HTTP 1.1
					                   Content-Type: text/plain; charset=utf-8
					                 request content
					             """);
			}

			[Fact]
			public async Task WhenFailing_ShouldIncludeResponseContentAndStatusCodeInMessage()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest)
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.OK);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has status code 200 OK,
					             but it had status code 400 BadRequest

					             HTTP-Request:
					               HTTP/1.1 400 BadRequest
					                 Content-Type: text/plain; charset=utf-8
					               some content
					               The originating request was <null>
					             """);
			}

			[Fact]
			public async Task WhenStatusCodeDiffersFromExpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest);

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.OK);

				await That(Act).Throws<XunitException>();
			}

			[Theory]
			[MemberData(nameof(SuccessStatusCodes), MemberType = typeof(ThatHttpResponseMessage))]
			[MemberData(nameof(RedirectStatusCodes), MemberType = typeof(ThatHttpResponseMessage))]
			[MemberData(nameof(ClientErrorStatusCodes), MemberType = typeof(ThatHttpResponseMessage))]
			[MemberData(nameof(ServerErrorStatusCodes), MemberType = typeof(ThatHttpResponseMessage))]
			public async Task WhenStatusCodeIsExpected_ShouldSucceed(HttpStatusCode statusCode)
			{
				HttpStatusCode expected = statusCode;
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has status code 202 Accepted,
					             but it was <null>
					             """);
			}
		}
	}
}
