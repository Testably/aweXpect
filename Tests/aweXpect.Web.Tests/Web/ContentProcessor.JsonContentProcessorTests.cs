using System.Net;
using System.Net.Http;
using aweXpect.Web.Tests.TestHelpers;

namespace aweXpect.Tests.Web.ContentProcessors;

public sealed partial class ContentProcessor
{
	public sealed class JsonContentProcessorTests
	{
		[Theory]
		[InlineData("application/json")]
		[InlineData("application/problem+json")]
		public async Task JsonContent_WithInvalidJson_ShouldIncludeRawTextInFailureMessage(
			string contentType)
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent("{\"my-content\":1")
				.WithContentType(contentType);

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that httpResponse
				               has status code 202 Accepted,
				               but it had status code 200 OK

				               HTTP-Request:
				                 HTTP/1.1 200 OK
				                   Content-Type: {{contentType}}
				                 {"my-content":1
				                 *** JSON parse error: '1' is an invalid end of a number. Expected a delimiter. LineNumber: 0 | BytePositionInLine: 15. ***
				                 The originating request was <null>
				               """);
		}

		[Theory]
		[InlineData("application/json")]
		[InlineData("application/problem+json")]
		public async Task JsonContent_WithValidJson_ShouldIncludePrettifiedJsonInFailureMessage(
			string contentType)
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent("{\"my-content\":1}")
				.WithContentType(contentType);

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<XunitException>()
				.WithMessage($$"""
				               Expected that httpResponse
				               has status code 202 Accepted,
				               but it had status code 200 OK

				               HTTP-Request:
				                 HTTP/1.1 200 OK
				                   Content-Type: {{contentType}}
				                 {
				                   "my-content": 1
				                 }
				                 The originating request was <null>
				               """);
		}
	}
}
