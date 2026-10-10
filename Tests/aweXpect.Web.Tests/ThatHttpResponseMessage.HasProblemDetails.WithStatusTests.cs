using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class WithStatusTests
		{
			[Fact]
			public async Task WhenStatusIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(500).WithStatus(501);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("WithStatus cannot be specified more than once.")
					.Because("the second status would silently replace the first one");
			}

			[Fact]
			public async Task WhenStatusIsSpecifiedTwiceAroundTitle_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(500).WithTitle("foo").WithStatus(501);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("WithStatus cannot be specified more than once.")
					.Because("the second status would silently replace the first one");
			}

			[Theory]
			[InlineData(200, 400)]
			[InlineData(404, 201)]
			public async Task WhenStatusDoesNotMatch_ShouldFail(int actualStatus, int expectedStatus)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "type": "my-type",
					                 "status": {{actualStatus}}
					               }
					               """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(expectedStatus);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with any type and status {{expectedStatus}},
					               but it had status {{actualStatus}}

					               HTTP-Response:
					                 200 OK HTTP/1.1
					                   Content-Type: text/plain; charset=utf-8
					                   Content-Length: *
					                 {
					                   "type": "my-type",
					                   "status": {{actualStatus}}
					                 }
					               """).AsWildcard();
			}

			[Fact]
			public async Task WhenStatusMatches_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "status": 500
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(500);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
