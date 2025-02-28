using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class HasMethod
	{
		public sealed class Tests
		{
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
	}
}
