using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class DoesNotHaveStatusCode
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				HttpResponseMessage? subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.OK);

				HttpResponseMessage result = await That(subject).DoesNotHaveStatusCode(HttpStatusCode.NotFound);

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithStatusCode(HttpStatusCode.NotFound),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.DoesNotHaveStatusCode(HttpStatusCode.NotFound)));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses do not have status code 404 NotFound for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenStatusCodeDiffersFromUnexpected_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest);

				async Task Act()
					=> await That(subject).DoesNotHaveStatusCode(HttpStatusCode.OK);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStatusCodeEqualsUnexpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.NotFound)
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveStatusCode(HttpStatusCode.NotFound);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have status code 404 NotFound,
					             but it had status code 404 NotFound

					             HTTP-Response:
					               404 NotFound HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveStatusCode(HttpStatusCode.Accepted);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have status code 202 Accepted,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenStatusCodeDiffersFromUnexpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.OK);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveStatusCode(HttpStatusCode.NotFound));

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenStatusCodeEqualsUnexpected_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.NotFound);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveStatusCode(HttpStatusCode.NotFound));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveStatusCode(HttpStatusCode.Accepted));

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
