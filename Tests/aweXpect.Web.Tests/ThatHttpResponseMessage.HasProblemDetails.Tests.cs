using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class Tests
		{
			[Test]
			public async Task AsRegex_WhenExpectedIsAnEmptyPattern_ShouldThrowArgumentException()
			{
				HttpResponseMessage subject = ResponseBuilder.WithContent("{}");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' regex pattern cannot be empty.").AsPrefix().And
					.WithParamName("expected");
			}

			[Test]
			public async Task WhenMemberIsACollection_Negated_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.DoesNotComplyWith(response => response.HasProblemDetailsContent())));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses do not have a ProblemDetails content for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasProblemDetailsContent()));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses have a ProblemDetails content with any type for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
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
					=> await That(subject).HasProblemDetailsContent("FOO").IgnoringCase().WithTitle("BAR")
						.WithStatus(404)
						.WithInstance("could-be-SOME-guid ").IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "FOO" ignoring case, title "BAR", status 404 and instance "could-be-SOME-guid " ignoring trailing whitespace,
					             but it had title "bar", which differs at index 0:
					                ↓ (actual)
					               "bar"
					               "BAR"
					                ↑ (expected)
					              and it had instance "could-be-some-guid", which differs at index 9:
					                         ↓ (actual)
					               "could-be-some-guid"
					               "could-be-SOME-guid"
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

			[Test]
			public async Task WhenContentHasMembersThatAreNotExpected_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo",
					               "title": "bar",
					               "status": 404,
					               "detail": "baz",
					               "instance": "could-be-some-guid"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).DoesNotThrow()
					.Because("only the expected members of the problem details are compared");
			}

			[Test]
			[Arguments("")]
			[Arguments("  ")]
			public async Task WhenContentIsEmpty_ShouldFail(string content)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(content);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "foo",
					             but it could not be parsed as problem details: The input does not contain any JSON tokens.*

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					             """).AsWildcard()
					.Because("an empty body cannot be inspected and must not let the JSON exception escape");
			}

			[Test]
			[Arguments("[]")]
			[Arguments("\"foo\"")]
			[Arguments("42")]
			[Arguments("null")]
			public async Task WhenContentIsNotAJsonObject_ShouldFail(string content)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(content);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo").WithTitle("bar");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has a ProblemDetails content with type "foo" and title "bar",
					              but it could not be parsed as problem details: the JSON value was not an object

					              HTTP-Response:
					                200 OK HTTP/1.1
					                  Content-Type: text/plain; charset=utf-8
					                  Content-Length: *
					                {content}
					              """).AsWildcard()
					.Because("problem details must be a JSON object");
			}

			[Test]
			public async Task WhenContentIsNotJson_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("<html>error</html>");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "foo",
					             but it could not be parsed as problem details: '<' is an invalid start of a value.*

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               <html>error</html>
					             """).AsWildcard()
					.Because("a non-JSON body must fail the expectation instead of letting the JSON exception escape");
			}

			[Test]
			public async Task WhenContentIsNotSet_ShouldFail()
			{
				HttpResponseMessage subject = new();

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "foo",
					             but it could not be parsed as problem details: The input does not contain any JSON tokens.*

					             HTTP-Response:
					               200 OK HTTP/1.1*
					             """).AsWildcard()
					.Because("a response without content is read as an empty string on every target framework");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("type", "42", "was a Number, not a String")]
			[Arguments("title", "{}", "was an Object, not a String")]
			[Arguments("detail", "[]", "was an Array, not a String")]
			[Arguments("instance", "true", "was a Boolean, not a String")]
			[Arguments("status", "\"400\"", "was a String, not a Number")]
			[Arguments("status", "400.5", "was 400.5, which is not a 32-bit integer")]
			[Arguments("status", "2147483648", "was 2147483648, which is not a 32-bit integer")]
			public async Task WhenMemberHasWrongType_ShouldFail(string member, string value, string reason)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "{{member}}": {{value}}
					               }
					               """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent();

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with any type,
					               but it could not be parsed as problem details: the member "{{member}}" {{reason}}

					               HTTP-Response:
					                 200 OK HTTP/1.1
					                   Content-Type: text/plain; charset=utf-8
					                   Content-Length: *
					                 {
					                   "{{member}}": {{value}}
					                 }
					               """).AsWildcard()
					.Because("a wrongly typed standard member must fail the expectation instead of letting the JSON exception escape");
			}

			[Test]
			public async Task WhenMembersAreNull_ShouldTreatThemAsAbsent()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo",
					               "title": null,
					               "status": null,
					               "detail": null,
					               "instance": null
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).DoesNotThrow()
					.Because("a null optional member is equivalent to an absent one");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeDiffersInCase_WithIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("FOO").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("foo", "bar")]
			[Arguments("foo", "FOO")]
			public async Task WhenTypeDoesNotMatch_ShouldFail(string actualType, string expectedType)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "type": "{{actualType}}"
					               }
					               """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent(expectedType);

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with type "{{expectedType}}",
					               but it had type "{{actualType}}", which differs at index 0:
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

			[Test]
			public async Task WhenTypeDoesNotMatchWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "https://awexpect.com/problems/not-found",
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("*/conflict").AsWildcard()
						.WithTitle("Not*").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type matching "*/conflict" and title matching "Not*",
					             but it had type "https://awexpect.com/problems/…", which did not match:
					               ↓ (actual)
					               "https://awexpect.com/problems/not-found"
					               "*/conflict"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "https://awexpect.com/problems/not-found",
					                 "title": "Not Found"
					               }
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypeIsAbsent_ShouldFailWithAboutBlank()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "no-type": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type "foo",
					             but it had type "about:blank", which differs at index 0:
					                ↓ (actual)
					               "about:blank"
					               "foo"
					                ↑ (expected)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "no-type": "foo"
					               }
					             """).AsWildcard()
					.Because("an absent type is assumed to be \"about:blank\" (RFC 9457)");
			}

			[Test]
			public async Task WhenTypeIsAbsent_ShouldMatchAboutBlank()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("about:blank");

				await That(Act).DoesNotThrow()
					.Because("an absent type is assumed to be \"about:blank\" (RFC 9457)");
			}

			[Test]
			public async Task WhenTypeIsAbsent_WithoutExpectedType_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent();

				await That(Act).DoesNotThrow()
					.Because("the type is optional in a problem details object (RFC 9457)");
			}

			[Test]
			public async Task WhenTypeIsAbsentAndDoesNotMatchWildcard_ShouldFailWithAboutBlank()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("*/not-found").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with type matching "*/not-found",
					             but it had type "about:blank", which did not match:
					               ↓ (actual)
					               "about:blank"
					               "*/not-found"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "title": "Not Found"
					               }
					             """).AsWildcard()
					.Because("the wildcard is matched against the assumed type \"about:blank\"");
			}

			[Test]
			public async Task WhenTypeIsAbsentAndMatchesWildcard_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("about:*").AsWildcard();

				await That(Act).DoesNotThrow()
					.Because("the wildcard is matched against the assumed type \"about:blank\"");
			}

			[Test]
			public async Task WhenTypeMatches_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeMatchesWildcard_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "https://awexpect.com/problems/not-found"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("*/not-found").AsWildcard();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenContentIsEmpty_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content with type "foo",
					             but it could not be parsed as problem details: The input does not contain any JSON tokens.*

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					             """).AsWildcard()
					.Because("a subject that cannot be inspected fails both ways");
			}

			[Test]
			public async Task WhenContentIsNotAJsonObject_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("[]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content,
					             but it could not be parsed as problem details: the JSON value was not an object

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               []
					             """).AsWildcard()
					.Because("a subject that cannot be inspected fails both ways");
			}

			[Test]
			public async Task WhenContentIsNotJson_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("<html>error</html>");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content with type "foo",
					             but it could not be parsed as problem details: '<' is an invalid start of a value.*

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               <html>error</html>
					             """).AsWildcard()
					.Because("a subject that cannot be inspected fails both ways");
			}

			[Test]
			public async Task WhenContentIsNotSet_ShouldFail()
			{
				HttpResponseMessage subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content with type "foo",
					             but it could not be parsed as problem details: The input does not contain any JSON tokens.*

					             HTTP-Response:
					               200 OK HTTP/1.1*
					             """).AsWildcard()
					.Because("a response without content is read as an empty string, which cannot be inspected and fails both ways");
			}

			[Test]
			[Arguments("type", "42", "was a Number, not a String")]
			[Arguments("title", "{}", "was an Object, not a String")]
			[Arguments("detail", "[]", "was an Array, not a String")]
			[Arguments("instance", "true", "was a Boolean, not a String")]
			[Arguments("status", "\"400\"", "was a String, not a Number")]
			[Arguments("status", "400.5", "was 400.5, which is not a 32-bit integer")]
			[Arguments("status", "2147483648", "was 2147483648, which is not a 32-bit integer")]
			public async Task WhenMemberHasWrongType_ShouldFail(string member, string value, string reason)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "{{member}}": {{value}}
					               }
					               """);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent());

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               does not have a ProblemDetails content,
					               but it could not be parsed as problem details: the member "{{member}}" {{reason}}

					               HTTP-Response:
					                 200 OK HTTP/1.1
					                   Content-Type: text/plain; charset=utf-8
					                   Content-Length: *
					                 {
					                   "{{member}}": {{value}}
					                 }
					               """).AsWildcard()
					.Because("a subject that cannot be inspected fails both ways");
			}

			[Test]
			[Arguments("foo", "bar")]
			[Arguments("foo", "FOO")]
			public async Task WhenTypeDoesNotMatch_ShouldSucceed(string actualType, string expectedType)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "type": "{{actualType}}"
					               }
					               """);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent(expectedType));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsAbsent_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("foo"));

				await That(Act).DoesNotThrow()
					.Because("an absent type is assumed to be \"about:blank\", which differs from the type");
			}

			[Test]
			public async Task WhenTypeIsAbsentAndMatchesWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "title": "Not Found"
					             }
					             """);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content with type matching "*",
					             but it had

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "title": "Not Found"
					               }
					             """).AsWildcard()
					.Because("an absent type is assumed to be \"about:blank\", which matches the wildcard");
			}

			[Test]
			public async Task WhenTypeMatches_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "foo"
					             }
					             """);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a ProblemDetails content with type "foo",
					             but it had

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "foo"
					               }
					             """).AsWildcard();
			}
		}
	}
}
