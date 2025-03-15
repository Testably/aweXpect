using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class HasRequestUri
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri(new Uri("https://awexpect.com"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUriDiffers_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).HasRequestUri("https://awexpect.com/awexpect.Web");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/awexpect.Web",
					             but it was "https://awexpect.com/" which differs at index 21:
					                           ↓ (actual)
					               "…xpect.com/"
					               "…xpect.com/awexpect.Web"
					                           ↑ (expected)

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Fact]
			public async Task WhenUriIsExpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).HasRequestUri("https://awexpect.com");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasRequestUri(new Uri("https://awexpect.com")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUriDiffers_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasRequestUri("https://awexpect.com/awexpect.Web"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUriIsExpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasRequestUri("https://awexpect.com"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/",
					             but it had
					             """);
			}
		}
	}
}
