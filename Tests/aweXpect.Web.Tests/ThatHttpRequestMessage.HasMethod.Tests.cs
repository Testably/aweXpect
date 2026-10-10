using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class HasMethod
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasMethod(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData("HEAD", "a")]
			[InlineData("OPTIONS", "an")]
			[InlineData("PATCH", "a")]
			public async Task ShouldUseTheIndefiniteArticleOfTheMethod(string method, string article)
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasMethod(new HttpMethod(method));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has {article} {method} method,
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenMethodDiffers_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).HasMethod(HttpMethod.Post);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a POST method,
					             but it was GET

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Fact]
			public async Task WhenMethodIsExpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get);

				async Task Act()
					=> await That(subject).HasMethod(HttpMethod.Get);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasMethod(HttpMethod.Delete);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a DELETE method,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("HEAD", "a")]
			[InlineData("OPTIONS", "an")]
			[InlineData("PATCH", "a")]
			public async Task ShouldUseTheIndefiniteArticleOfTheMethod(string method, string article)
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasMethod(new HttpMethod(method)));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have {article} {method} method,
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenMethodDiffers_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasMethod(HttpMethod.Post));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsExpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Put);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasMethod(HttpMethod.Put));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a PUT method,
					             but it had

					             HTTP-Request:
					               PUT https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasMethod(HttpMethod.Delete));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a DELETE method,
					             but it was <null>
					             """);
			}
		}
	}
}
