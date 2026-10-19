using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class HasRequestUri
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_Negated_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder,];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.DoesNotComplyWith(request => request.HasRequestUri("https://awexpect.com/"))));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests do not have a request URI equal to "https://awexpect.com/" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder,];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.HasRequestUri("https://example.com/")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Requests have a request URI equal to "https://example.com/" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenExpectedStringIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedUriIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri((Uri)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("")]
			[Arguments("not a uri")]
			[Arguments("https://")]
			public async Task WhenExpectedStringIsNoValidAbsoluteUri_ShouldThrowArgumentException(string expected)
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri(expected);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' must be a valid absolute URI.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedUriIsRelative_ShouldThrowArgumentException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri(new Uri("/tracks/1", UriKind.Relative));

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' must be a valid absolute URI.").AsPrefix()
					.Because("the Uri overload validates the expected URI like the string overload");
			}

			[Test]
			public async Task WhenPathDiffersOnlyInCase_IgnoringCase_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://music.example.com/Tracks/1?genre=JAZZ");

				async Task Act()
					=> await That(subject).HasRequestUri("https://music.example.com/tracks/1?genre=jazz").IgnoringCase()
						.And.HasRequestUri(new Uri("https://music.example.com/TRACKS/1?genre=Jazz")).IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("IgnoringCase also ignores the case of the path and the query");
			}

			[Test]
			public async Task WhenPathDiffersOnlyInCase_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://music.example.com/Tracks/1");

				async Task Act()
					=> await That(subject).HasRequestUri("https://music.example.com/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://music.example.com/trac…",
					             but it had request URI "https://music.example.com/Trac…", which differs at index 26:
					                           ↓ (actual)
					               "…ample.com/Tracks/1"
					               "…ample.com/tracks/1"
					                           ↑ (expected)

					             HTTP-Request:
					               GET https://music.example.com/Tracks/1 HTTP/1.1
					             """)
					.Because("the path of a URI is case-sensitive");
			}

			[Test]
			public async Task WhenQueryDiffersOnlyInCase_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://music.example.com/tracks?genre=JAZZ");

				async Task Act()
					=> await That(subject).HasRequestUri("https://music.example.com/tracks?genre=jazz");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://music.example.com/trac…",
					             but it had request URI "https://music.example.com/trac…", which differs at index 39:
					                           ↓ (actual)
					               "…cks?genre=JAZZ"
					               "…cks?genre=jazz"
					                           ↑ (expected)

					             HTTP-Request:
					               GET https://music.example.com/tracks?genre=JAZZ HTTP/1.1
					             """)
					.Because("the query of a URI is case-sensitive");
			}

			[Test]
			[Arguments("https://music.example.com/Tracks/1?genre=JAZZ", "HTTPS://Music.Example.COM/Tracks/1?genre=JAZZ")]
			[Arguments("HTTPS://MUSIC.EXAMPLE.COM/Tracks/1?genre=JAZZ", "https://music.example.com/Tracks/1?genre=JAZZ")]
			public async Task WhenSchemeAndHostDifferOnlyInCase_ShouldSucceed(string actual, string expected)
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri(actual);

				async Task Act()
					=> await That(subject).HasRequestUri(expected).And.HasRequestUri(new Uri(expected));

				await That(Act).DoesNotThrow()
					.Because("the scheme and the host of a URI are case-insensitive");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri(new Uri("https://awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUriDiffers_IgnoringCase_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).HasRequestUri("https://awexpect.com/abd").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/abd" ignoring case,
					             but it had request URI "https://awexpect.com/ABC", which differs at index 23:
					                           ↓ (actual)
					               "…ect.com/ABC"
					               "…ect.com/abd"
					                           ↑ (expected)

					             HTTP-Request:
					               GET https://awexpect.com/ABC HTTP/1.1
					             """)
				.Because("IgnoringCase skips the case-only difference at index 21");
			}

			[Test]
			public async Task WhenUriDiffers_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).HasRequestUri("https://awexpect.com/awexpect.Web");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI equal to "https://awexpect.com/awexpect.…",
					             but it had request URI "https://awexpect.com/" with a length of 21, which is shorter than the expected length of 33 and misses:
					               "awexpect.Web"

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenUriHasDifferentCaseBeforeTheDifference_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithMethod(HttpMethod.Get)
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).HasRequestUri("https://awexpect.com/abd");

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
					             """)
					.Because("the path is compared case-sensitively, so the first difference is the case of the first character");
			}

			[Test]
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
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasRequestUri(new Uri("https://awexpect.com")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/",
					             but it was <null>
					             """);
			}

			[Test]
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

			[Test]
			public async Task WhenUriDiffersOnlyInCase_IgnoringCase_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com/ABC");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasRequestUri("https://awexpect.com/abc").IgnoringCase());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a request URI equal to "https://awexpect.com/abc" ignoring case,
					             but it had

					             HTTP-Request:
					               HEAD https://awexpect.com/ABC HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenUriIsExpected_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithUri("https://awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasRequestUri("https://awexpect.com"));

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
	}
}
