using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetails
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldCombineMultipleChecks()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo",
					               "title": "bar",
					               "status": 404,
					               "instance": "could-be-some-guid"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetails("FOO").IgnoringCase().WithTitle("BAR").WithStatus(404)
						.WithInstance("could-be-SOME-guid ").IgnoringTrailingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "FOO" ignoring case, title "BAR", status 404 and instance "could-be-SOME-guid " ignoring trailing white-space,
					             but it had title "bar" which differs at index 0:
					                ↓ (actual)
					               "bar"
					               "BAR"
					                ↑ (expected)
					              and it had instance "could-be-some-guid" which differs at index 9:
					                         ↓ (actual)
					               "could-be-some-guid"
					               "could-be-SOME-guid "
					                         ↑ (expected)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "foo",
					                 "title": "bar",
					                 "status": 404,
					                 "instance": "could-be-some-guid"
					               }
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNoTypeIsSpecified_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "no-type": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetails("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "foo",
					             but it did not match the expected format because no 'type' property existed

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "no-type": "foo"
					               }
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetails("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "foo",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeDiffersInCase_WithIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetails("FOO").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar")]
			[InlineData("foo", "FOO")]
			public async Task WhenTypeDoesNotMatch_ShouldFail(string actualType, string expectedType)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "type": "{{actualType}}"
					               }
					               """);

				async Task Act()
					=> await That(subject).HasProblemDetails(expectedType);

				await That(Act).Throws<XunitException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with type "{{expectedType}}",
					               but it was type "{{actualType}}" which differs at index 0:
					                  ↓ (actual)
					                 "{{actualType}}"
					                 "{{expectedType}}"
					                  ↑ (expected)

					               HTTP-Response:
					                 200 OK HTTP/1.1
					                   Content-Type: text/plain; charset=utf-8
					                   Content-Length: *
					                 {
					                   "type": "{{actualType}}"
					                 }
					               """).AsWildcard();
			}

			[Fact]
			public async Task WhenTypeMatches_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetails("foo");

				await That(Act).DoesNotThrow();
			}
		}
	}
}
