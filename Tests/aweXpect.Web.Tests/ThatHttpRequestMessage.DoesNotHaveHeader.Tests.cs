using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class DoesNotHaveHeader
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder.WithHeader("x-my-header", "foo"),];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.DoesNotHaveHeader("x-my-header")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests do not have an "x-my-header" header for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveHeader(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("Accept", "an")]
			[Arguments("ETag", "an")]
			[Arguments("Host", "a")]
			[Arguments("User-Agent", "a")]
			[Arguments("X-Request-Id", "an")]
			public async Task ShouldUseTheIndefiniteArticleOfTheHeaderName(string name, string article)
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveHeader(name);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have {article} "{name}" header,
					              but it was <null>
					              """);
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldSucceed()
			{
				string name = "x-my-header";
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveHeader(name);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderExists_ShouldFail()
			{
				string name = "x-my-header";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, "some header")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveHeader(name);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-my-header" header,
					             but it did contain the "x-my-header" header: ["some header"]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveHeader("x-my-header");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-my-header" header,
					             but it was <null>
					             """);
			}
		}
	}
}
