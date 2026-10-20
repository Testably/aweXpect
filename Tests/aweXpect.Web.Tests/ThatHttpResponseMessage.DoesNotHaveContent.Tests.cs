using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class DoesNotHaveContent
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenContentDiffersFromUnexpected_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveContent("other content");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentEqualsUnexpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveContent("some content");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a string content equal to "some content",
					             but it had

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentMatchesUnexpectedWithOptions_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveContent("SOME*").AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a string content matching "SOME*" ignoring case,
					             but it had

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """)
					.Because("the string options apply to the unexpected content");
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.DoesNotHaveContent("foo")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses do not have a string content equal to "foo" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveContent("some content");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a string content equal to "some content",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveContent(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenContentDiffersFromUnexpected_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContent("other content"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a string content equal to "other content",
					             but it had string content "some content", which differs at index 0:
					                ↓ (actual)
					               "some content"
					               "other content"
					                ↑ (expected)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentEqualsUnexpected_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveContent("some content"));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
