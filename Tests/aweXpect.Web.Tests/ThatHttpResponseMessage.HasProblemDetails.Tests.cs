using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_Negated_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.DoesNotComplyWith(response => response.HasProblemDetailsContent())));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Responses do not have a ProblemDetails content for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasProblemDetailsContent()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Responses have a ProblemDetails content with any type for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

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
					=> await That(subject).HasProblemDetailsContent("FOO").IgnoringCase().WithTitle("BAR")
						.WithStatus(404)
						.WithInstance("could-be-SOME-guid ").IgnoringTrailingWhiteSpace();

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Theory]
			[InlineData("")]
			[InlineData("  ")]
			public async Task WhenContentIsEmpty_ShouldFail(string content)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(content);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).Throws<XunitException>()
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

			[Theory]
			[InlineData("[]")]
			[InlineData("\"foo\"")]
			[InlineData("42")]
			[InlineData("null")]
			public async Task WhenContentIsNotAJsonObject_ShouldFail(string content)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(content);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo").WithTitle("bar");

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenContentIsNotJson_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("<html>error</html>");

				async Task Act()
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).Throws<XunitException>()
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

			[Theory]
			[InlineData("type", "42", "was a Number, not a String")]
			[InlineData("title", "{}", "was an Object, not a String")]
			[InlineData("detail", "[]", "was an Array, not a String")]
			[InlineData("instance", "true", "was a Boolean, not a String")]
			[InlineData("status", "\"400\"", "was a String, not a Number")]
			[InlineData("status", "400.5", "was 400.5, which is not a 32-bit integer")]
			[InlineData("status", "2147483648", "was 2147483648, which is not a 32-bit integer")]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
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
					=> await That(subject).HasProblemDetailsContent("foo");

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
					=> await That(subject).HasProblemDetailsContent("foo");

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
					=> await That(subject).HasProblemDetailsContent("FOO").IgnoringCase();

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
					=> await That(subject).HasProblemDetailsContent(expectedType);

				await That(Act).Throws<XunitException>()
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
					=> await That(subject).HasProblemDetailsContent("foo");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenContentIsEmpty_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("foo"));

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenContentIsNotAJsonObject_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("[]");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent());

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenContentIsNotJson_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("<html>error</html>");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasProblemDetailsContent("foo"));

				await That(Act).Throws<XunitException>()
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

			[Theory]
			[InlineData("type", "42", "was a Number, not a String")]
			[InlineData("title", "{}", "was an Object, not a String")]
			[InlineData("detail", "[]", "was an Array, not a String")]
			[InlineData("instance", "true", "was a Boolean, not a String")]
			[InlineData("status", "\"400\"", "was a String, not a Number")]
			[InlineData("status", "400.5", "was 400.5, which is not a 32-bit integer")]
			[InlineData("status", "2147483648", "was 2147483648, which is not a 32-bit integer")]
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

				await That(Act).Throws<XunitException>()
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

			[Theory]
			[InlineData("foo", "bar")]
			[InlineData("foo", "FOO")]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
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
