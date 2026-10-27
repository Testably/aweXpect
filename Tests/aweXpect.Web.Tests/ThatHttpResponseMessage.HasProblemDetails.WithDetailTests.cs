using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class WithDetailTests
		{
			[Test]
			public async Task WhenDetailIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenDetailIsNullAfterStatus_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(400).WithDetail(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenDetailIsSpecifiedTwice_ShouldUseTheLastDetail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "detail": "baz"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail("foo").WithDetail("bar");

				await That(Act).Throws<FailException>()
					.WithMessage("*has a ProblemDetails content with any type and detail \"bar\",*but it had detail \"baz\"*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenDetailIsSpecifiedTwiceAroundStatus_ShouldUseTheLastDetailInItsPlace()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "detail": "baz",
					               "status": 500
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail("foo").WithStatus(500).WithDetail("bar");

				await That(Act).Throws<FailException>()
					.WithMessage(
						"*has a ProblemDetails content with any type, detail \"bar\" and status 500,*but it had detail \"baz\"*")
					.AsWildcard();
			}

			[Test]
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

			[Test]
			[Arguments("foo", "bar")]
			[Arguments("foo", "FOO")]
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

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenDetailDoesNotMatchWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "detail": "The album was not found."
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail("*track*").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and detail matching "*track*",
					             but it had detail "The album was not found.", which did not match:
					               ↓ (actual)
					               "The album was not found."
					               "*track*"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "detail": "The album was not found."
					               }
					             """).AsWildcard();
			}

			[Test]
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

			[Test]
			public async Task WhenDetailMatchesWildcard_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "detail": "The album was not found."
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithDetail("*album*").AsWildcard();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
