using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class DoesNotHaveRequestUri
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
							.ComplyWith(item => item.DoesNotHaveRequestUri("https://awexpect.com/")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests do not have a request URI equal to "https://awexpect.com/" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenPathDiffersOnlyInCase_IgnoringCase_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri(new Uri("https://awexpect.com/abc")).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/abc" ignoring case,
					             but it had

					             HTTP-Request:
					               GET https://awexpect.com/ABC HTTP/1.1
					             """)
					.Because("IgnoringCase also ignores the case of the path");
			}

			[Test]
			public async Task WhenPathDiffersOnlyInCase_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri("https://awexpect.com/abc");

				await That(Act).DoesNotThrow()
					.Because("the path of a URI is case-sensitive");
			}

			[Test]
			public async Task WhenRequestUriIsNull_ShouldSucceed()
			{
				HttpRequestMessage subject = new();

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri("https://awexpect.com");

				await That(Act).DoesNotThrow()
					.Because("a request without a request URI does not have the unexpected one");
			}

			[Test]
			[Arguments("HTTPS://AWEXPECT.COM/ABC")]
			[Arguments("https://AWExpect.com/ABC")]
			public async Task WhenSchemeAndHostDifferOnlyInCase_ShouldFail(string unexpected)
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri(new Uri(unexpected));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/ABC",
					             but it had

					             HTTP-Request:
					               GET https://awexpect.com/ABC HTTP/1.1
					             """)
					.Because("the scheme and the host of a URI are case-insensitive");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri(new Uri("https://awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedStringIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("")]
			[Arguments("not a uri")]
			[Arguments("https://")]
			public async Task WhenUnexpectedStringIsNoValidAbsoluteUri_ShouldThrowArgumentException(string unexpected)
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri(unexpected);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' must be a valid absolute URI.").AsPrefix();
			}

			[Test]
			public async Task WhenUnexpectedUriIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri((Uri)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenUnexpectedUriIsRelative_ShouldThrowArgumentException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri(new Uri("/tracks/1", UriKind.Relative));

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' must be a valid absolute URI.").AsPrefix()
					.Because("the Uri overload validates the unexpected URI like the string overload");
			}

			[Test]
			public async Task WhenUriDiffers_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri("https://awexpect.com/awexpect.Web");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUriIsUnexpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotHaveRequestUri("https://awexpect.com");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/",
					             but it had

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveRequestUri("https://awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUriDiffers_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveRequestUri("https://awexpect.com/abd"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/abd",
					             but it had request URI "https://awexpect.com/ABC", which differs at index 21:
					                           ↓ (actual)
					               "…xpect.com/ABC"
					               "…xpect.com/abd"
					                           ↑ (expected)

					             HTTP-Request:
					               GET https://awexpect.com/ABC HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenUriDiffersOnlyInCase_IgnoringCase_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveRequestUri("https://awexpect.com/abc").IgnoringCase());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUriIsUnexpected_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveRequestUri(new Uri("https://awexpect.com")));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
