using System.Net.Http;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasRequestUri
	{
		public sealed class ExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri((Action<IThatSubject<Uri?>>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenPathMatchesWildcard_ShouldSucceed()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://music.example.com/tracks/42");

				async Task Act()
					=> await That(subject).HasRequestUri(uri => uri.HasPath("/tracks/*").AsWildcard());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRequestUriDoesNotSatisfyExpectations_Negated_ShouldSucceed()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://music.example.com/tracks/42");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasRequestUri(uri => uri.HasPath().StartingWith("/admin/")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenRequestUriDoesNotSatisfyExpectations_ShouldFail()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://music.example.com/albums/42");

				async Task Act()
					=> await That(subject).HasRequestUri(uri => uri
						.HasHost().EqualTo("music.example.com").And
						.HasPath().StartingWith("/tracks/"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI which has host equal to "music.example.com" and has path starting with "/tracks/",
					             but the request URI had path "/albums/42", which differs at index 1:
					                 ↓ (actual)
					               "/albums/42"
					               "/tracks/"
					                 ↑ (expected prefix)

					             HTTP-Request:
					               GET https://music.example.com/albums/42 HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenRequestUriIsNull_ShouldFail()
			{
				HttpRequestMessage subject = new()
				{
					Method = HttpMethod.Get,
				};

				async Task Act()
					=> await That(subject).HasRequestUri(uri => uri.HasHost().EqualTo("music.example.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI which has host equal to "music.example.com",
					             but the request URI was <null>

					             HTTP-Request:
					               GET  HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenRequestUriSatisfiesExpectations_Negated_ShouldFail()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://music.example.com/tracks/42");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasRequestUri(uri => uri.HasPath().StartingWith("/tracks/")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI which does not have path starting with "/tracks/",
					             but the request URI had path "/tracks/42"

					             HTTP-Request:
					               GET https://music.example.com/tracks/42 HTTP/1.1
					             """);
			}

			[Test]
			public async Task WhenRequestUriSatisfiesExpectations_ShouldSucceed()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://music.example.com/tracks/42");

				async Task Act()
					=> await That(subject).HasRequestUri(uri => uri
						.HasHost().EqualTo("music.example.com").And
						.HasPath().StartingWith("/tracks/"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_Negated_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasRequestUri(uri => uri.HasHost().EqualTo("music.example.com")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI which does not have host equal to "music.example.com",
					             but it was <null>
					             """)
					.Because("a null subject has no request URI, so the negated expectation fails as well");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasRequestUri(uri => uri.HasHost().EqualTo("music.example.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a request URI which has host equal to "music.example.com",
					             but it was <null>
					             """);
			}
		}
	}
}
