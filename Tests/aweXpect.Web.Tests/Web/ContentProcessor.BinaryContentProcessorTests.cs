using System.Net;
using System.Net.Http;
using aweXpect.Web.Tests.TestHelpers;

namespace aweXpect.Tests.Web.ContentProcessors;

public sealed partial class ContentProcessor
{
	public sealed class BinaryContentProcessorTests
	{
		[Fact]
		public async Task BinaryContentProcessor_Test()
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder().WithContent("foo").WithContentType("text/css");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Response:
				               200 OK HTTP/1.1
				                 Content-Type: text/css
				               foo
				             """);
		}

		[Theory]
		[InlineData("audio/mp3")]
		[InlineData("audio/mpeg")]
		[InlineData("video/mp4")]
		[InlineData("video/mpeg")]
		[InlineData("image/gif")]
		[InlineData("image/webp")]
		[InlineData("application/pdf")]
		public async Task WhenContentTypeIsInterpretedAsBinaryContent_ShouldIncludeContentTypeAndLengthInFailureMessage(
			string contentType)
		{
			byte[] bytes = "foo-bar"u8.ToArray();
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent(bytes)
				.WithContentType(contentType);

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that httpResponse
				               has status code 202 Accepted,
				               but it had status code 200 OK

				               HTTP-Response:
				                 200 OK HTTP/1.1
				                   Content-Type: {{contentType}}
				                 *Content is binary ({{contentType}}) with length {{bytes.Length}}*
				               """);
		}
	}
}
