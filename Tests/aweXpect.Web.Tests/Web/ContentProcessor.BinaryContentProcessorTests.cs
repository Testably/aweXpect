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
			HttpResponseMessage httpResponse = new HttpResponseBuilder().WithContent("foo").WithMediaType("text/css");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Request:
				               HTTP/1.1 200 OK
				               foo
				               The originating request was <null>
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
				.WithMediaType(contentType);

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that httpResponse
				               has status code 202 Accepted,
				               but it had status code 200 OK

				               HTTP-Request:
				                 HTTP/1.1 200 OK
				                 *Content is binary ({{contentType}}) with length {{bytes.Length}}*
				                 The originating request was <null>
				               """);
		}
	}
}
