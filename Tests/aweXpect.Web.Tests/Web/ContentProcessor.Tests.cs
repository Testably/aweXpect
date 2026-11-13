using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using aweXpect.Customization;
using aweXpect.Web;
using aweXpect.Web.ContentProcessors;
using aweXpect.Web.Tests.TestHelpers;

namespace aweXpect.Tests.Web.ContentProcessors;

public sealed partial class ContentProcessor
{
	public sealed class Tests
	{
		[Test]
		public async Task WhenProcessorThrows_ShouldKeepTheHttpContextAndShowTheException()
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent("foo")
				.WithContentType("text/csv")
				.WithHeader("x-my-header", "bar");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			using (Customize.aweXpect.Web().ContentProcessors.Set([
				       new ThrowingContentProcessor(new InvalidOperationException("processor bug")),
				       new StringContentProcessor(),
			       ]))
			{
				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that httpResponse
					             has status code equal to 202 Accepted,
					             but it had status code 200 OK

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: bar
					                 Content-Type: text/csv
					                 Content-Length: 3
					               *Content (text/csv) with length 3, but the content processor ContentProcessor.Tests.ThrowingContentProcessor did throw an InvalidOperationException: processor bug*
					             """)
					.Because("a bug in a processor must neither hide the rest of the HTTP context nor leave its partial output, nor fall back to later processors");
			}
		}

		[Test]
		public async Task WhenProcessorThrowsOperationCanceledExceptionWithoutCancellation_ShouldShowTheException()
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent("foo")
				.WithContentType("text/csv");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			using (Customize.aweXpect.Web().ContentProcessors.Set([
				       new ThrowingContentProcessor(new OperationCanceledException("not canceled")),
			       ]))
			{
				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that httpResponse
					             has status code equal to 202 Accepted,
					             but it had status code 200 OK

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/csv
					                 Content-Length: 3
					               *Content (text/csv) with length 3, but the content processor ContentProcessor.Tests.ThrowingContentProcessor did throw an OperationCanceledException: not canceled*
					             """)
					.Because("only a cancellation of the evaluation drops the context, not an exception of the processor itself");
			}
		}

		[Test]
		public async Task WithDisposedContent_ShouldNotCallCustomProcessors()
		{
			StringContent content = new("foo");
			content.Dispose();
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent(content)
				.WithContentType("text/csv");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			using (Customize.aweXpect.Web().ContentProcessors.Set([new UncheckedReadingContentProcessor(),]))
			{
				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that httpResponse
					             has status code equal to 202 Accepted,
					             but it had status code 200 OK

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/csv
					               *Content (text/csv) with unknown length*
					             """)
					.Because("a custom processor that reads disposed content without checking it must not be called");
			}
		}

		[Test]
		public async Task WithDisposedContent_ShouldReturnDefaultText()
		{
			StringContent content = new("foo");
			content.Dispose();
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContent(content)
				.WithContentType("application/my-type");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code equal to 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Response:
				               200 OK HTTP/1.1
				                 Content-Type: application/my-type
				               *Content (application/my-type) with unknown length*
				             """);
		}

		[Test]
		public async Task WithoutContent_ShouldReturnDefaultText()
		{
			HttpResponseMessage httpResponse = new HttpResponseBuilder()
				.WithContentType("application/my-type");

			async Task Act()
				=> await That(httpResponse).HasStatusCode().EqualTo(HttpStatusCode.Accepted);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that httpResponse
				             has status code equal to 202 Accepted,
				             but it had status code 200 OK

				             HTTP-Response:
				               200 OK HTTP/1.1
				                 Content-Type: application/my-type
				                 Content-Length: 0
				               *Content (application/my-type) with length 0*
				             """);
		}

		private sealed class ThrowingContentProcessor(Exception exception) : IContentProcessor
		{
			public Task<bool> AppendContentInfo(StringBuilder messageBuilder, HttpContent httpContent,
				string indentation, CancellationToken cancellationToken = default)
			{
				messageBuilder.Append("partial output");
				throw exception;
			}
		}

		private sealed class UncheckedReadingContentProcessor : IContentProcessor
		{
			public async Task<bool> AppendContentInfo(StringBuilder messageBuilder, HttpContent httpContent,
				string indentation, CancellationToken cancellationToken = default)
			{
				messageBuilder.AppendLine(await httpContent.ReadAsStringAsync());
				return true;
			}
		}
	}
}
