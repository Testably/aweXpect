using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class WithDetailTests
		{
			[Fact]
			public async Task WhenDetailIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail("foo").WithDetail("bar");

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("WithDetail cannot be specified more than once.")
					.Because("the second detail would silently replace the first one");
			}

			[Fact]
			public async Task WhenDetailIsSpecifiedTwiceAroundStatus_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail("foo").WithStatus(500).WithDetail("bar");

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("WithDetail cannot be specified more than once.")
					.Because("the second detail would silently replace the first one");
			}

			[Fact]
			public async Task WhenDetailIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("detail").And
					.WithMessage("The 'detail' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenDetailIsNullAfterStatus_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(400).WithDetail(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("detail").And
					.WithMessage("The 'detail' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenDetailDiffersInCase_WithIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "detail": "bar"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail("BAR").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar")]
			[InlineData("foo", "FOO")]
			public async Task WhenDetailDoesNotMatch_ShouldFail(string actualDetail, string expectedDetail)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "type": "my-type",
					                 "detail": "{{actualDetail}}"
					               }
					               """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail(expectedDetail);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with any type and detail "{{expectedDetail}}",
					               but it had detail "{{actualDetail}}", which differs at index 0:
					                  ↓ (actual)
					                 "{{actualDetail}}"
					                 "{{expectedDetail}}"
					                  ↑ (expected)

					               HTTP-Response:
					                 200 OK HTTP/1.1
					                   Content-Type: text/plain; charset=utf-8
					                   Content-Length: *
					                 {
					                   "type": "my-type",
					                   "detail": "{{actualDetail}}"
					                 }
					               """).AsWildcard();
			}

			[Fact]
			public async Task WhenDetailMatches_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "detail": "foo",
					               "status": 200,
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(200).WithDetail("foo");

				await That(Act).DoesNotThrow();
			}
		}
	}
}
