using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasHeader
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder,];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasHeader("x-my-header")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses have an "x-my-header" header for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name").And
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("Accept-Ranges", "an")]
			[Arguments("ETag", "an")]
			[Arguments("Server", "a")]
			[Arguments("Upgrade", "an")]
			[Arguments("X-Request-Id", "an")]
			public async Task ShouldUseTheIndefiniteArticleOfTheHeaderName(string name, string article)
			{
				HttpResponseMessage? subject = null;

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
			public async Task WhenContentHeaderDoesNotExist_ShouldFail()
			{
				HttpResponseMessage subject = new(HttpStatusCode.OK)
				{
					Content = new StringContent("some content"),
				};

				async Task Act()
					=> await That(subject).HasHeader("Content-Disposition");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a "Content-Disposition" header,
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			[Arguments("Content-Type")]
			[Arguments("content-type")]
			[Arguments("Content-Length")]
			public async Task WhenContentHeaderExists_ShouldSucceed(string name)
			{
				HttpResponseMessage subject = new(HttpStatusCode.OK)
				{
					Content = new StringContent("some content"),
				};

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentLengthWasNotComputedYet_ShouldFindTheComputedValue()
			{
				StringContent content = new("some content");
				HttpResponseMessage subject = new(HttpStatusCode.OK)
				{
					Content = content,
				};
				await That(content.Headers.TryGetValues("Content-Length", out _)).IsFalse()
					.Because("the content length is only computed when it is read");

				async Task Act()
					=> await That(subject).HasHeader("Content-Length").WithValue("12");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header,
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenHeaderExists_ShouldSucceed()
			{
				string name = "x-my-header";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, "some header");

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderIsInTheMessageAndTheContentHeaders_ShouldCombineTheValues()
			{
				StringContent content = new("some content");
				content.Headers.Add("x-my-header", "content value");
				HttpResponseMessage subject = new(HttpStatusCode.OK)
				{
					Content = content,
				};
				subject.Headers.Add("x-my-header", "message value");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("message value", "content value");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

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
