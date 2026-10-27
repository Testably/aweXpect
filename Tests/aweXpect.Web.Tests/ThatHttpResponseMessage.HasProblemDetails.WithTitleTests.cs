using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class WithTitleTests
		{
			[Test]
			public async Task AsRegex_WhenExpectedIsAnEmptyPattern_ShouldThrowArgumentException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("""{"title": "Not Found"}""");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenCombiningMatchTypes_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Not*").AsWildcard().AsSuffix();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AsSuffix cannot be combined with AsWildcard.");
			}

			[Test]
			public async Task WhenCombiningWildcardWithUsing_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Not*").AsWildcard()
						.Using(StringComparer.Ordinal);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Using cannot be combined with AsWildcard.");
			}

			[Test]
			public async Task WhenTitleIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenTitleIsNullAfterStatus_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(400).WithTitle(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
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

			[Test]
			public async Task WhenTitleDoesNotEndWithSuffix_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Allowed").AsSuffix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and title ending with "Allowed",
					             but it had title "Not Found", which differs at index 7:
					                       ↓ (actual)
					               "Not Found"
					                 "Allowed"
					                       ↑ (expected suffix)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "title": "Not Found"
					               }
					             """).AsWildcard();
			}

			[Test]
			[Arguments("foo", "bar")]
			[Arguments("foo", "FOO")]
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

				await That(Act).Throws<FailException>()
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenTitleDoesNotMatchRegex_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("^Not [0-9]+$").AsRegex();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and title matching regex "^Not [0-9]+$",
					             but it had title "Not Found", which did not match:
					               ↓ (actual)
					               "Not Found"
					               "^Not [0-9]+$"
					               ↑ (regex pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "title": "Not Found"
					               }
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTitleDoesNotMatchWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Bad*").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and title matching "Bad*",
					             but it had title "Not Found", which did not match:
					               ↓ (actual)
					               "Not Found"
					               "Bad*"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "title": "Not Found"
					               }
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTitleDoesNotMatchWildcardIgnoringCase_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("BAD*").AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and title matching "BAD*" ignoring case,
					             but it had title "Not Found", which did not match:
					               ↓ (actual)
					               "Not Found"
					               "BAD*"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "title": "Not Found"
					               }
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTitleDoesNotStartWithPrefix_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Bad").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and title starting with "Bad",
					             but it had title "Not Found", which differs at index 0:
					                ↓ (actual)
					               "Not Found"
					               "Bad"
					                ↑ (expected prefix)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "title": "Not Found"
					               }
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTitleEndsWithSuffix_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Found").AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTitleIsAbsentAndMatchedAsWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("*").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and title matching "*",
					             but it had title <null>, which did not match:
					               ↓ (actual)
					               <null>
					               "*"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type"
					               }
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTitleIsSpecifiedTwice_ShouldUseTheLastTitle()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "baz"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("foo").WithTitle("bar");

				await That(Act).Throws<FailException>()
					.WithMessage("*has a ProblemDetails content with any type and title \"bar\",*but it had title \"baz\"*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenTitleIsSpecifiedTwiceAroundStatus_ShouldUseTheLastTitleInItsPlace()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "baz",
					               "status": 500
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("foo").WithStatus(500).WithTitle("bar");

				await That(Act).Throws<FailException>()
					.WithMessage(
						"*has a ProblemDetails content with any type, title \"bar\" and status 500,*but it had title \"baz\"*")
					.AsWildcard();
			}

			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenTitleMatchesRegex_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("^Not [A-Z][a-z]+$").AsRegex();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTitleMatchesWildcard_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Not*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTitleMatchesWildcardIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("NOT*").AsWildcard().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTitleStartsWithPrefix_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithTitle("Not").AsPrefix();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedWithTitleTests
		{
			[Test]
			public async Task WhenTitleDoesNotMatchWildcard_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasProblemDetailsContent().WithTitle("Bad*").AsWildcard());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTitleIsAbsentAndMatchedAsWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type"
					             }
					             """);

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasProblemDetailsContent().WithTitle("*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content and title matching "*",
					             but it had title <null>, which did not match:
					               ↓ (actual)
					               <null>
					               "*"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type"
					               }
					             """).AsWildcard()
					.Because("an absent title has no content to inspect, so it fails the expectation and its negation alike");
			}

			[Test]
			public async Task WhenTitleMatchesWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasProblemDetailsContent().WithTitle("Not*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content and title matching "Not*",
					             but it had

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "title": "Not Found"
					               }
					             """).AsWildcard();
			}
		}
	}
}
