using System.IO;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class HasContentType
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasContentType("application/json")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses have a `Content-Type` header equal to "application/json" ignoring case for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContentType(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenContentTypeDiffersFromExpected_ShouldFail()
			{
				string expected = "text/content-type";
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("text/other-content-type")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasContentType(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/content-type" ignoring case,
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
			public async Task WhenContentTypeDiffersOnlyInCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("Text/Plain")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasContentType("text/plain");

				await That(Act).DoesNotThrow()
					.Because("RFC 9110 defines the type and subtype of a media type as case-insensitive");
			}

			[Test]
			public async Task WhenContentTypeDiffersOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("Text/Plain")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasContentType("text/plain").IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("a repeated IgnoringCase replaces the default");
			}

			[Test]
			public async Task WhenContentTypeDiffersOnlyInCase_WithoutIgnoringCase_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("Text/Plain")
					.WithContent("some content");

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

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: Text/Plain
					                 Content-Length: 12
					               some content
					             """)
					.Because("IgnoringCase(false) compares the media type case-sensitively again");
			}

			[Test]
			public async Task WhenContentTypeEqualsExpected_ShouldSucceed()
			{
				string expected = "some/content";
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType(expected);

				async Task Act()
					=> await That(subject).HasContentType(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentTypeHeaderIsNotSet_ShouldFail()
			{
				string expected = "text/content-type";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(new StreamContent(new MemoryStream([0x0, 0x1,])));

				async Task Act()
					=> await That(subject).HasContentType(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "text/content-type" ignoring case,
					             but it had no `Content-Type` header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Length: 2
					               *Content with length 2*
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContentType("some content").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a `Content-Type` header equal to "some content" ignoring case,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUsingAComparerAfterIgnoringCaseFalse_ShouldUseTheComparer()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("Text/Plain")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasContentType("text/plain").IgnoringCase(false)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUsingAComparer_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("text/plain");

				async Task Act()
					=> await That(subject).HasContentType("text/plain").Using(StringComparer.Ordinal);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead.")
					.Because("the media type is compared ignoring case by default, which a comparer cannot honour");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenContentTypeDiffersFromExpected_ShouldFail()
			{
				string expected = "text/content-type";
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType("text/other-content-type")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContentType(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentTypeEqualsExpected_ShouldFail()
			{
				string expected = "some/content-type";
				HttpResponseMessage subject = ResponseBuilder
					.WithContentType(expected);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContentType(expected));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `Content-Type` header equal to "some/content-type" ignoring case,
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
				string expected = "text/content-type";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(new StreamContent(new MemoryStream([0x0, 0x1,])));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContentType(expected));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
