using System.IO;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class HasContentType
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				HttpRequestMessage result = await That(subject).HasContentType("text/plain");

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenContentTypeDiffersFromExpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/other-content-type");

				async Task Act()
					=> await That(subject).HasContentType("text/content-type");

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
			[Arguments("text/plain", "")]
			[Arguments("text/", nameof(StringEqualityOptions.AsPrefix))]
			[Arguments("^text/p.*$", nameof(StringEqualityOptions.AsRegex))]
			[Arguments("/plain", nameof(StringEqualityOptions.AsSuffix))]
			[Arguments("text/*", nameof(StringEqualityOptions.AsWildcard))]
			public async Task WhenContentTypeDiffersOnlyInCase_ShouldSucceed(string expected, string matchType)
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("Text/Plain");

				async Task Act()
				{
					StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>> result =
						That(subject).HasContentType(expected);
					await (matchType switch
					{
						nameof(StringEqualityOptions.AsPrefix) => result.AsPrefix(),
						nameof(StringEqualityOptions.AsRegex) => result.AsRegex(),
						nameof(StringEqualityOptions.AsSuffix) => result.AsSuffix(),
						nameof(StringEqualityOptions.AsWildcard) => result.AsWildcard(),
						_ => result,
					});
				}

				await That(Act).DoesNotThrow()
					.Because("RFC 9110 defines the type and subtype of a media type as case-insensitive");
			}

			[Test]
			public async Task WhenContentTypeDiffersOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("Text/Plain");

				async Task Act()
					=> await That(subject).HasContentType("text/plain").IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("a repeated IgnoringCase replaces the default");
			}

			[Test]
			public async Task WhenContentTypeDiffersOnlyInCase_WithoutIgnoringCase_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("Text/Plain");

				async Task Act()
					=> await That(subject).HasContentType("text/plain").IgnoringCase(false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/plain",
					             but it had content type "Text/Plain", which differs at index 0:
					                ↓ (actual)
					               "Text/Plain"
					               "text/plain"
					                ↑ (expected)

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: Text/Plain
					                 Content-Length: 12
					               some content
					             """)
					.Because("IgnoringCase(false) compares the media type case-sensitively again");
			}

			[Test]
			public async Task WhenContentTypeDoesNotMatchExpectedWithOptions_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).HasContentType("APPLICATION/*").AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header matching "APPLICATION/*" ignoring case,
					             but it had content type "text/plain", which did not match:
					               ↓ (actual)
					               "text/plain"
					               "APPLICATION/*"
					               ↑ (wildcard pattern)

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentTypeEqualsExpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).HasContentType("text/plain");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentTypeHeaderIsNotSet_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent(new StreamContent(new MemoryStream([0x0, 0x1,])));

				async Task Act()
					=> await That(subject).HasContentType("text/content-type");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/content-type" ignoring case,
					             but it had no `Content-Type` header

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Length: 2
					               *Content with length 2*
					             """);
			}

			[Test]
			public async Task WhenContentTypeMatchesExpectedWithOptions_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).HasContentType("TEXT/*").AsWildcard().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContentType(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.HasContentType("application/json")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests have a `Content-Type` header equal to "application/json" ignoring case for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenRequestHasNoContent_ShouldFail()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://awexpect.com");

				async Task Act()
					=> await That(subject).HasContentType("text/plain");

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
					=> await That(subject).HasContentType("text/plain").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/plain" ignoring case,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenContentTypeDiffersFromExpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/other-content-type");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContentType("text/content-type"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentTypeEqualsExpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content")
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContentType("text/plain"));

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
			public async Task WhenRequestHasNoContent_ShouldSucceed()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContentType("text/plain"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContentType("text/plain"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header equal to "text/plain" ignoring case,
					             but it was <null>
					             """);
			}
		}
	}
}
