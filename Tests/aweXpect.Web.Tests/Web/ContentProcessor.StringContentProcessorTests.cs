using System.Net;
using System.Net.Http;
using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;
using aweXpect.Web.Tests.TestHelpers;

namespace aweXpect.Tests.Web.ContentProcessors;

public sealed partial class ContentProcessor
{
	public sealed class StringContentProcessorTests
	{
		[Theory]
		[InlineData("application/json")]
		[InlineData("application/problem+json")]
		public async Task JsonString_WithoutJsonProcessor_ShouldIncludeTextContentInFailureMessage(
			string contentType)
		{
			using (Customize.aweXpect.Web().ContentProcessors.Set([new StringContentProcessor(),]))
			{
				HttpResponseMessage httpResponse = new HttpResponseBuilder()
					.WithContent("{\"my-content\":1}")
					.WithContentType(contentType);

				async Task Act()
					=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that httpResponse
					             has status code 202 Accepted,
					             but it had status code 200 OK

					             HTTP-Request:
					               HTTP/1.1 200 OK
					               {"my-content":1}
					               The originating request was <null>
					             """);
			}
		}

		[Fact]
		public async Task TextFile_ShouldIncludeTextContentInFailureMessage()
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent("""
				             body {
				                 background-color: powderblue;
				             }
				             """)
				.WithContentType("text/css");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Request:
				               HTTP/1.1 200 OK
				               body {
				                   background-color: powderblue;
				               }
				               The originating request was <null>
				             """);
		}

		[Theory]
		[InlineData("application/xml")]
		[InlineData("application/html")]
		[InlineData("text/plain")]
		[InlineData("text/csv")]
		[InlineData("text/html")]
		public async Task WhenContentTypeIsInterpretedAsStringContent_ShouldIncludeTextContentInFailureMessage(
			string contentType)
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent("{\"my-content\":1}")
				.WithContentType(contentType);

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Request:
				               HTTP/1.1 200 OK
				               {"my-content":1}
				               The originating request was <null>
				             """);
		}
	}
}
