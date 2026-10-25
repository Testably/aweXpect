namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasPath
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasPath().EqualTo("/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has path equal to "/tracks/1",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasPath().EqualTo("tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has path equal to "tracks/1",
					             but it was relative
					             """);
			}

			[Test]
			public async Task WhenPathIsEscaped_ShouldCompareTheDecodedPath()
			{
				Uri subject = new("https://music.example.com/my%20song/1");

				async Task Act()
					=> await That(subject).HasPath().EqualTo("/my song/1");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPathContainsPlus_ShouldKeepThePlus()
			{
				Uri subject = new("https://music.example.com/rock+roll");

				async Task Act()
					=> await That(subject).HasPath().EqualTo("/rock+roll");

				await That(Act).DoesNotThrow()
					.Because("a '+' is only a space in form-encoded query strings, not in paths");
			}

			[Test]
			public async Task WhenSubjectHasNoPath_ShouldBeSlash()
			{
				Uri subject = new("https://music.example.com");

				async Task Act()
					=> await That(subject).HasPath().EqualTo("/");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task StartingWith_WhenSubjectPathStartsWithTheExpectedValue_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1?page=2#lyrics");

				async Task Act()
					=> await That(subject).HasPath().StartingWith("/tracks/");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedPath_ShouldFail()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).HasPath().EqualTo("/tracks/2");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has path equal to "/tracks/2",
					             but it had path "/tracks/1", which differs at index 8:
					                        ↓ (actual)
					               "/tracks/1"
					               "/tracks/2"
					                        ↑ (expected)
					             """);
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedPath_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1?page=2#lyrics");

				async Task Act()
					=> await That(subject).HasPath().EqualTo("/tracks/1");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectEqualsTheExpectedPath_ShouldFail()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).HasPath().NotEqualTo("/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have path equal to "/tracks/1",
					             but it had path "/tracks/1"
					             """);
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedPath_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).HasPath().NotEqualTo("/tracks/2");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_AsWildcard_WhenSubjectMatchesThePattern_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).HasPath("/tracks/*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_IgnoringCase_WhenSubjectDiffersOnlyInCase_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/Tracks/1");

				async Task Act()
					=> await That(subject).HasPath("/tracks/1").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).HasPath(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task Shorthand_WhenSubjectDiffersOnlyInCase_ShouldFail()
			{
				Uri subject = new("https://music.example.com/Tracks/1");

				async Task Act()
					=> await That(subject).HasPath("/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has path equal to "/tracks/1",
					             but it had path "/Tracks/1", which differs at index 1:
					                 ↓ (actual)
					               "/Tracks/1"
					               "/tracks/1"
					                 ↑ (expected)
					             """)
					.Because("URI paths are case-sensitive");
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedPath_ShouldFail()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).HasPath("/tracks/2");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has path equal to "/tracks/2",
					             but it had path "/tracks/1", which differs at index 8:
					                        ↓ (actual)
					               "/tracks/1"
					               "/tracks/2"
					                        ↑ (expected)
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedPath_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1?page=2#lyrics");

				async Task Act()
					=> await That(subject).HasPath("/tracks/1");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasPath("/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has path equal to "/tracks/1",
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPath().EqualTo("/tracks/1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have path equal to "/tracks/1",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPath().EqualTo("/tracks/2"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have path equal to "/tracks/2",
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedPath_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPath().EqualTo("/tracks/2"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedPath_ShouldFail()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPath().EqualTo("/tracks/1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have path equal to "/tracks/1",
					             but it had path "/tracks/1"
					             """);
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedPath_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPath().NotEqualTo("/tracks/1"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedPath_ShouldSucceed()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPath("/tracks/2"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedPath_ShouldFail()
			{
				Uri subject = new("https://music.example.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPath("/tracks/1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have path equal to "/tracks/1",
					             but it had path "/tracks/1"
					             """);
			}
		}
	}
}
