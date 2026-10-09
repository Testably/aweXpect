using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasStatusCode
	{
		public sealed class NegatedTests
		{
			[Fact]
			public async Task ClientError_WhenStatusCodeIsClientError_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.NotFound);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode().ClientError());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a client error status code (4xx),
					             but it had status code 404 NotFound

					             HTTP-Response:
					               404 NotFound HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task DifferentTo_WhenStatusCodeIsDifferent_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.OK);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasStatusCode().DifferentTo(HttpStatusCode.NotFound));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has status code 404 NotFound,
					             but it had status code 200 OK

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task DifferentTo_WhenStatusCodeIsEqual_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.NotFound);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasStatusCode().DifferentTo(HttpStatusCode.NotFound));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EqualTo_WhenStatusCodeIsDifferent_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.OK);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasStatusCode().EqualTo(HttpStatusCode.NotFound));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EqualTo_WhenStatusCodeIsEqual_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.NotFound);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasStatusCode().EqualTo(HttpStatusCode.NotFound));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have status code 404 NotFound,
					             but it had status code 404 NotFound

					             HTTP-Response:
					               404 NotFound HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task Error_WhenStatusCodeIsError_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.InternalServerError);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode().Error());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an error status code (4xx or 5xx),
					             but it had status code 500 InternalServerError

					             HTTP-Response:
					               500 InternalServerError HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task Error_WhenStatusCodeIsSuccess_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.OK);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode().Error());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task HasStatusCode_WhenStatusCodeIsEqual_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.NotFound);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode(HttpStatusCode.NotFound));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have status code 404 NotFound,
					             but it had status code 404 NotFound

					             HTTP-Response:
					               404 NotFound HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task Redirection_WhenStatusCodeIsRedirection_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.NotModified);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode().Redirection());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a redirection status code (3xx),
					             but it had status code 304 NotModified

					             HTTP-Response:
					               304 NotModified HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task ServerError_WhenStatusCodeIsServerError_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadGateway);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode().ServerError());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a server error status code (5xx),
					             but it had status code 502 BadGateway

					             HTTP-Response:
					               502 BadGateway HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task Success_WhenStatusCodeIsSuccess_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.OK);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode().Success());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a success status code (2xx),
					             but it had status code 200 OK

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasStatusCode().Success());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a success status code (2xx),
					             but it was <null>
					             """)
					.Because("a null subject has no status code, so the negated expectation fails as well");
			}
		}
	}
}
