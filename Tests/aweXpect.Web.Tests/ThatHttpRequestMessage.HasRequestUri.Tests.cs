using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class HasRequestUri
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedStringIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedUriIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri((Uri)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData("")]
			[InlineData("not a uri")]
			[InlineData("https://")]
			public async Task WhenExpectedStringIsNoValidAbsoluteUri_ShouldThrowArgumentException(string expected)
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri(expected);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' must be a valid absolute URI.").AsPrefix();
			}

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
					             but it was "https://awexpect.com/" with a length of 21, which is shorter than the expected length of 33 and misses:
					               "awexpect.Web"

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Fact]
			public async Task WhenUriHasDifferentCaseBeforeTheDifference_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).HasRequestUri("https://awexpect.com/abd");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/abd",
					             but it was "https://awexpect.com/ABC", which differs at index 23:
					                           ↓ (actual)
					               "…ect.com/ABC"
					               "…ect.com/abd"
					                           ↑ (expected)

					             HTTP-Request:
					               GET https://awexpect.com/ABC HTTP/1.1
					             """)
					.Because("the request URI is compared ignoring case, so the difference in case is not reported");
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
					             
					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					             """);
			}
		}
	}
}
