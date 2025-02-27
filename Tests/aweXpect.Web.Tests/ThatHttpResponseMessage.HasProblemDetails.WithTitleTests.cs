using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetails
	{
		public sealed class WithTitleTests
		{
			[Fact]
			public async Task WhenTitleDiffersInCase_WithIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "bar"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetails().WithTitle("BAR").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar")]
			[InlineData("foo", "FOO")]
			public async Task WhenTitleDoesNotMatch_ShouldFail(string actualTitle, string expectedTitle)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "type": "my-type",
					                 "title": "{{actualTitle}}"
					               }
					               """);

				async Task Act()
					=> await That(subject).HasProblemDetails().WithTitle(expectedTitle);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with any type and title "{{expectedTitle}}",
					               but it had title "{{actualTitle}}" which differs at index 0:
					                  ↓ (actual)
					                 "{{actualTitle}}"
					                 "{{expectedTitle}}"
					                  ↑ (expected)

					               HTTP-Response:
					                 200 OK HTTP/1.1
					                   Content-Type: text/plain; charset=utf-8
					                   Content-Length: *
					                 {
					                   "type": "my-type",
					                   "title": "{{actualTitle}}"
					                 }
					               """).AsWildcard();
			}

			[Fact]
			public async Task WhenTitleMatches_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetails().WithTitle("foo");

				await That(Act).DoesNotThrow();
			}
		}
	}
}
