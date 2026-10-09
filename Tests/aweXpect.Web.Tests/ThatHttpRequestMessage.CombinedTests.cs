using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class CombinedTests
	{
		[Fact]
		public async Task WhenHasContentAndHasRequestUriFail_ShouldShowRequestOnce()
		{
			HttpRequestMessage subject = RequestBuilder
				.WithMethod(HttpMethod.Get)
				.WithContent("foo");

			async Task Act()
				=> await That(subject).HasContent("bar").And.HasRequestUri("https://example.com");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a string content equal to "bar" and has a request URI equal to "https://example.com/",
				             but it was "foo", which differs at index 0:
				                ↓ (actual)
				               "foo"
				               "bar"
				                ↑ (expected)
				             and it was "https://awexpect.com/", which differs at index 8:
				                        ↓ (actual)
				               "https://awexpect.com/"
				               "https://example.com/"
				                        ↑ (expected)

				             HTTP-Request:
				               GET https://awexpect.com/ HTTP/1.1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 3
				               foo
				             """);
		}

		[Fact]
		public async Task WhenHasMethodAndHasHeaderFail_ShouldShowRequestOnce()
		{
			HttpRequestMessage subject = RequestBuilder
				.WithMethod(HttpMethod.Get)
				.WithContent("foo")
				.WithHeader("x-a", "1");

			async Task Act()
				=> await That(subject).HasMethod(HttpMethod.Post).And.HasHeader("y");

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has a POST method and has a `y` header,
				             but it was GET and did not contain the expected header

				             HTTP-Request:
				               GET https://awexpect.com/ HTTP/1.1
				                 x-a: 1
				                 Content-Type: text/plain; charset=utf-8
				                 Content-Length: 3
				               foo
				             """);
		}
	}
}
