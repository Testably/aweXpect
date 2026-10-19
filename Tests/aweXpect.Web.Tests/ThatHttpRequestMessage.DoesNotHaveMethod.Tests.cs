using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class DoesNotHaveMethod
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("HEAD", "a")]
			[Arguments("OPTIONS", "an")]
			[Arguments("PATCH", "a")]
			public async Task ShouldUseTheIndefiniteArticleOfTheMethod(string method, string article)
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveMethod(new HttpMethod(method));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have {article} {method} method,
					              but it was <null>
					              """);
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder,];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.DoesNotHaveMethod(HttpMethod.Head)));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests do not have a HEAD method for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenMethodDiffers_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get);

				async Task Act()
					=> await That(subject).DoesNotHaveMethod(HttpMethod.Delete);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsUnexpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Delete);

				async Task Act()
					=> await That(subject).DoesNotHaveMethod(HttpMethod.Delete);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a DELETE method,
					             but it had

					             HTTP-Request:
					               DELETE https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveMethod(HttpMethod.Delete);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a DELETE method,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveMethod(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodDiffers_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveMethod(HttpMethod.Post));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a POST method,
					             but it had method GET

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenMethodIsUnexpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Put);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveMethod(HttpMethod.Put));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveMethod(HttpMethod.Delete));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a DELETE method,
					             but it was <null>
					             """);
			}
		}
	}
}
