using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class HasRequestMessage
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestMessage(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenRequestHasDifferentContent_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("bar")
					.WithRequest(HttpMethod.Get, "https://www.awexpect.com")
					.WithRequestContent("foo");

				async Task Act()
					=> await That(subject).HasRequestMessage(request => request.HasContent("bar"));

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenRequestHasMatchingContent_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithRequest(HttpMethod.Get, "https://www.awexpect.com")
					.WithRequestContent("foo");

				async Task Act()
					=> await That(subject).HasRequestMessage(request => request.HasContent("foo"));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
