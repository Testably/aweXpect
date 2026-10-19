using System.IO;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class DoesNotHaveContentType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenContentTypeDiffersFromUnexpected_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("text/other-content-type")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/content-type");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentTypeEqualsUnexpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("some/content-type");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("some/content-type");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header equal to "some/content-type",
					             but it had

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: some/content-type
					                 Content-Length: 0
					               *Content (some/content-type) with length 0*
					             """);
			}

			[Test]
			public async Task WhenContentTypeHeaderIsNotSet_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(new StreamContent(new MemoryStream([0x0, 0x1,])));

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/content-type");

				await That(Act).DoesNotThrow()
					.Because("a response without a `Content-Type` header does not have the unexpected media type");
			}

			[Test]
			public async Task WhenContentTypeMatchesUnexpectedWithOptions_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("some/content-type");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("SOME/*").AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header matching "SOME/*" ignoring case,
					             but it had

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: some/content-type
					                 Content-Length: 0
					               *Content (some/content-type) with length 0*
					             """)
					.Because("the string options apply to the unexpected media type");
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithContentType("application/json"),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.DoesNotHaveContentType("application/json")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses do not have a `Content-Type` header equal to "application/json" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("some/content-type");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header equal to "some/content-type",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveContentType(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenContentTypeDiffersFromUnexpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("text/other-content-type")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContentType("text/content-type"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/content-type",
					             but it had content type "text/other-content-type", which differs at index 5:
					                     ↓ (actual)
					               "text/other-content-type"
					               "text/content-type"
					                     ↑ (expected)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/other-content-type
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentTypeEqualsUnexpected_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("some/content-type");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContentType("some/content-type"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentTypeHeaderIsNotSet_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(new StreamContent(new MemoryStream([0x0, 0x1,])));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContentType("text/content-type"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/content-type",
					             but it had no `Content-Type` header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Length: 2
					               *Content with length 2*
					             """);
			}
		}
	}
}
