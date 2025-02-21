using System.Net;
using System.Net.Http;
using aweXpect.Web.Tests.TestHelpers;

namespace aweXpect.Tests.Web.ContentProcessors;

public sealed partial class ContentProcessor
{
	public sealed class Tests
	{
		[Fact]
		public async Task WithDisposedContent_ShouldReturnDefaultText()
		{
			StringContent content = new("foo");
			content.Dispose();
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent(content)
				.WithContentType("application/my-type");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Request:
				               HTTP/1.1 200 OK
				                 Content-Type: application/my-type
				               *Content (application/my-type) with length 0*
				               The originating request was <null>
				             """);
		}

		[Fact]
		public async Task WithoutContent_ShouldReturnDefaultText()
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContentType("application/my-type");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Request:
				               HTTP/1.1 200 OK
				                 Content-Type: application/my-type
				               *Content (application/my-type) with length 0*
				               The originating request was <null>
				             """);
		}
	}
}
