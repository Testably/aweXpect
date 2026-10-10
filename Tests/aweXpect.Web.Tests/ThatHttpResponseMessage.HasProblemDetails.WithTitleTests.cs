using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class WithTitleTests
		{
			[Fact]
			public async Task WhenTitleIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("foo").WithTitle("bar");

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("WithTitle cannot be specified more than once.")
					.Because("the second title would silently replace the first one");
			}

			[Fact]
			public async Task WhenTitleIsSpecifiedTwiceAroundStatus_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("foo").WithStatus(500).WithTitle("bar");

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("WithTitle cannot be specified more than once.")
					.Because("the second title would silently replace the first one");
			}

			[Fact]
			public async Task WhenTitleIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("title").And
					.WithMessage("The 'title' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenTitleIsNullAfterStatus_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(400).WithTitle(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("title").And
					.WithMessage("The 'title' cannot be null.").AsPrefix();
			}

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
					=> await That(subject).HasProblemDetailsContent().WithTitle("BAR").IgnoringCase();

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
					=> await That(subject).HasProblemDetailsContent().WithTitle(expectedTitle);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with any type and title "{{expectedTitle}}",
					               but it had title "{{actualTitle}}", which differs at index 0:
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
			public async Task WhenTitleDoesNotMatch_WithIgnoringCase_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("FOX").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and title "FOX" ignoring case,
					             but it had title "foo", which differs at index 2:
					                  ↓ (actual)
					               "foo"
					               "FOX"
					                  ↑ (expected)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "title": "foo"
					               }
					             """).AsWildcard()
					.Because("the title is compared ignoring case, so the difference in case is not reported");
			}

			[Fact]
			public async Task WhenTitleMatches_AndContentHasDetailAndInstance_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "foo",
					               "detail": "bar",
					               "instance": "baz"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("foo");

				await That(Act).DoesNotThrow()
					.Because("the detail and instance are not compared, when they are not expected");
			}

			[Fact]
			public async Task WhenTitleMatches_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "foo",
					               "status": 200,
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(200).WithTitle("foo");

				await That(Act).DoesNotThrow();
			}
		}
	}
}
