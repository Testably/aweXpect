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
				.WithMediaType("application/my-type");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Request:
				               HTTP/1.1 200 OK
				               *Content (application/my-type) with length 0 could not be handled by any processor!*
				               The originating request was <null>
				             """);
		}

		[Fact]
		public async Task WithoutContent_ShouldReturnDefaultText()
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithMediaType("application/my-type");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Request:
				               HTTP/1.1 200 OK
				               *Content (application/my-type) with length 0 could not be handled by any processor!*
				               The originating request was <null>
				             """);
		}
	}
}
