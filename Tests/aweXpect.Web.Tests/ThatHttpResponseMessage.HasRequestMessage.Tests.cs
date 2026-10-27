using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class HasRequestMessage
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestMessage(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenRequestHasDifferentContent_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("bar")
					.WithRequest(HttpMethod.Get, "https://www.awexpect.com")
					.WithRequestContent("foo");

				async Task Act()
					=> await That(subject).HasRequestMessage(request => request.HasContent("bar"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request message which has a string content equal to "bar",
					             but it had string content "foo", which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "bar"
					                ↑ (expected)

					             HTTP-Request:
					               GET https://www.awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 3
					               foo

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 3
					               bar
					             """);
			}

			[Test]
			public async Task WhenRequestHasDifferentContentLength_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("bar")
					.WithRequest(HttpMethod.Get, "https://www.awexpect.com")
					.WithRequestContent("foo");

				async Task Act()
					=> await That(subject).HasRequestMessage(request
						=> request.HasContent(content => content.HasLength().EqualTo(5)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request message which has a string content which has length equal to 5,
					             but the string content had length 3

					             HTTP-Request:
					               GET https://www.awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 3
					               foo

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 3
					               bar
					             """);
			}

			[Test]
			public async Task WhenRequestHasDifferentHost_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("bar")
					.WithRequest(HttpMethod.Get, "https://music.example.com");

				async Task Act()
					=> await That(subject).HasRequestMessage(request
						=> request.HasRequestUri(uri => uri.HasHost().EqualTo("other.example.com")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request message which has a request URI which has host equal to "other.example.com",
					             but the request URI had host "music.example.com", which differs at index 0:
					                ↓ (actual)
					               "music.example.com"
					               "other.example.com"
					                ↑ (expected)

					             HTTP-Request:
					               GET https://music.example.com/ HTTP/1.1

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 3
					               bar
					             """);
			}

			[Test]
			public async Task WhenRequestHasMatchingContent_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithRequest(HttpMethod.Get, "https://www.awexpect.com")
					.WithRequestContent("foo");

				async Task Act()
					=> await That(subject).HasRequestMessage(request => request.HasContent("foo"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenResponseHasNoRequestMessage_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("bar");

				async Task Act()
					=> await That(subject).HasRequestMessage(request => request.IsNotNull());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request message which is not null,
					             but it was <null>

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 3
					               bar
					             """)
					.Because("a response does not need a request message, so the nested subject can be null");
			}

			[Test]
			public async Task WhenSubjectIsNull_Negated_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasRequestMessage(request => request.HasMethod(HttpMethod.Get)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request message which does not have a GET method,
					             but it was <null>
					             """)
					.Because("a null subject has no request message, so the negated expectation fails as well");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestMessage(request => request.HasMethod(HttpMethod.Get));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request message which has a GET method,
					             but it was <null>
					             """);
			}
		}
	}
}
