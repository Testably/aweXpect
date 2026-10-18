using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasHeader
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder,];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.HasHeader("x-my-header")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests have an "x-my-header" header for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
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
					=> await That(subject).HasHeader(name);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              has {article} "{name}" header,
					              but it was <null>
					              """);
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header,
					             but it did not contain the expected header

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenHeaderExists_ShouldSucceed()
			{
				string name = "x-my-header";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, "some header");

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-header");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header,
					             but it was <null>
					             """);
			}
		}
	}
}
