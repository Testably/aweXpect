using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasStatusCode
	{
		public sealed class EqualToTests
		{
			[Test]
			public async Task WhenMemberIsACollection_Negated_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder,];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.DoesNotComplyWith(response => response.HasStatusCode().EqualTo(HttpStatusCode.OK))));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses do not have status code 200 OK for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder,];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasStatusCode().EqualTo(HttpStatusCode.NotFound)));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses have status code 404 NotFound for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenFailing_ShouldIncludeRequestInMessage()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest)
					.WithContent("some content")
					.WithRequest(HttpMethod.Get, "https://awexpect.com")
					.WithRequestContent("request content");

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.OK);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has status code 200 OK,
					             but it had status code 400 BadRequest

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 15
					               request content

					             HTTP-Response:
					               400 BadRequest HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenFailing_ShouldIncludeResponseContentAndStatusCodeInMessage()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest)
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.OK);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has status code 200 OK,
					             but it had status code 400 BadRequest

					             HTTP-Response:
					               400 BadRequest HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenStatusCodeDiffersFromExpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest);

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.OK);

				await That(Act).Throws<FailException>();
			}

			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(SuccessStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(RedirectStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ClientErrorStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ServerErrorStatusCodes))]
			public async Task WhenStatusCodeIsExpected_ShouldSucceed(HttpStatusCode statusCode)
			{
				HttpStatusCode expected = statusCode;
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has status code 202 Accepted,
					             but it was <null>
					             """);
			}
		}
	}
}
