using System.IO;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class DoesNotHaveContentType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				HttpRequestMessage result = await That(subject).DoesNotHaveContentType("application/json");

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenContentTypeDiffersFromUnexpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/other-content-type");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/content-type");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentTypeDiffersOnlyInCaseFromUnexpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("Text/Plain");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/plain");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header equal to "text/plain" ignoring case,
					             but it had

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: Text/Plain
					                 Content-Length: 12
					               some content
					             """)
					.Because("RFC 9110 defines the type and subtype of a media type as case-insensitive");
			}

			[Test]
			public async Task WhenContentTypeDiffersOnlyInCaseFromUnexpected_WithoutIgnoringCase_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("Text/Plain");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/plain").IgnoringCase(false);

				await That(Act).DoesNotThrow()
					.Because("IgnoringCase(false) compares the media type case-sensitively again");
			}

			[Test]
			public async Task WhenContentTypeEqualsUnexpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/plain");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header equal to "text/plain" ignoring case,
					             but it had

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentTypeHeaderIsNotSet_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent(new StreamContent(new MemoryStream([0x0, 0x1,])));

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/content-type");

				await That(Act).DoesNotThrow()
					.Because("a request without a `Content-Type` header does not have the unexpected media type");
			}

			[Test]
			public async Task WhenContentTypeMatchesUnexpectedWithOptions_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("TEXT/*").AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header matching "TEXT/*" ignoring case,
					             but it had

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain
					                 Content-Length: 12
					               some content
					             """)
					.Because("the string options apply to the unexpected media type");
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder.WithContent("foo").WithContentType("application/json"),];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.DoesNotHaveContentType("application/json")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests do not have a `Content-Type` header equal to "application/json" ignoring case for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenRequestHasNoContent_ShouldSucceed()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/plain");

				await That(Act).DoesNotThrow()
					.Because("a request without content has no media type at all");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveContentType("text/plain");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header equal to "text/plain" ignoring case,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

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
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/other-content-type");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContentType("text/content-type"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/content-type" ignoring case,
					             but it had content type "text/other-content-type", which differs at index 5:
					                     ↓ (actual)
					               "text/other-content-type"
					               "text/content-type"
					                     ↑ (expected)

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/other-content-type
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentTypeEqualsUnexpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContentType("text/plain"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRequestHasNoContent_ShouldFail()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContentType("text/plain"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/plain" ignoring case,
					             but it had no `Content-Type` header

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContentType("text/plain"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/plain" ignoring case,
					             but it was <null>
					             """);
			}
		}
	}
}
