namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsBaseOf
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("https://awexpect.com/api/"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.IsBaseOf("https://example.com/api/tracks")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Uris are base URIs of https://example.com/api/tracks for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenExpectedStringIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsBaseOf((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedUriIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsBaseOf((Uri)null!);

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
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsBaseOf(expected);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' must be a valid absolute URI.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedUriIsRelative_ShouldThrowArgumentException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsBaseOf(new Uri("/api/tracks", UriKind.Relative));

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' must be a valid absolute URI.").AsPrefix()
					.Because("a relative URI would be resolved against the subject and thus always be based on it");
			}

			[Test]
			public async Task WhenSubjectIsBaseOfExpected_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).IsBaseOf(new Uri("https://awexpect.com/api/tracks/1"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsBaseOfExpectedString_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).IsBaseOf("https://awexpect.com/api/tracks/1");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasNoTrailingSlash_ShouldIgnoreItsLastSegment()
			{
				Uri subject = new("https://awexpect.com/api");

				async Task Act()
					=> await That(subject).IsBaseOf("https://awexpect.com/tracks/1");

				await That(Act).DoesNotThrow()
					.Because("Uri.IsBaseOf only considers the subject up to its last slash");
			}

			[Test]
			public async Task WhenSubjectIsNotBaseOfExpected_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).IsBaseOf("https://example.com/api/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a base URI of https://example.com/api/tracks/1,
					             but it was https://awexpect.com/api/
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsBaseOf("https://awexpect.com/api/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a base URI of https://awexpect.com/api/tracks/1,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/api/", UriKind.Relative);

				async Task Act()
					=> await That(subject).IsBaseOf("https://awexpect.com/api/tracks/1");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a base URI of https://awexpect.com/api/tracks/1,
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsBaseOfExpected_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBaseOf("https://awexpect.com/api/tracks/1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a base URI of https://awexpect.com/api/tracks/1,
					             but it was https://awexpect.com/api/
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNotBaseOfExpected_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBaseOf("https://example.com/api/tracks/1"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBaseOf("https://awexpect.com/api/tracks/1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a base URI of https://awexpect.com/api/tracks/1,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/api/", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsBaseOf("https://awexpect.com/api/tracks/1"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a base URI of https://awexpect.com/api/tracks/1,
					             but it was relative
					             """);
			}
		}
	}
}
