namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsNotBaseOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("https://awexpect.com/api/"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.IsNotBaseOf("https://awexpect.com/api/tracks")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Uris are not base URIs of https://awexpect.com/api/tracks for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenUnexpectedStringIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotBaseOf((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedUriIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotBaseOf((Uri)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData("")]
			[InlineData("not a uri")]
			[InlineData("https://")]
			public async Task WhenUnexpectedStringIsNoValidAbsoluteUri_ShouldThrowArgumentException(string unexpected)
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotBaseOf(unexpected);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' must be a valid absolute URI.").AsPrefix();
			}

			[Fact]
			public async Task WhenUnexpectedUriIsRelative_ShouldThrowArgumentException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotBaseOf(new Uri("/api/tracks", UriKind.Relative));

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' must be a valid absolute URI.").AsPrefix()
					.Because("a relative URI would be resolved against the subject and thus always be based on it");
			}

			[Fact]
			public async Task WhenSubjectIsBaseOfUnexpected_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).IsNotBaseOf(new Uri("https://awexpect.com/api/tracks/1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a base URI of https://awexpect.com/api/tracks/1,
					             but it was https://awexpect.com/api/
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotBaseOfUnexpected_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).IsNotBaseOf(new Uri("https://example.com/api/tracks/1"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotBaseOfUnexpectedString_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).IsNotBaseOf("https://example.com/api/tracks/1");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotBaseOf("https://awexpect.com/api/tracks/1");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a base URI of https://awexpect.com/api/tracks/1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/api/", UriKind.Relative);

				async Task Act()
					=> await That(subject).IsNotBaseOf("https://awexpect.com/api/tracks/1");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a base URI of https://awexpect.com/api/tracks/1,
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsBaseOfUnexpected_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotBaseOf("https://awexpect.com/api/tracks/1"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotBaseOfUnexpected_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/api/");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotBaseOf("https://example.com/api/tracks/1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a base URI of https://example.com/api/tracks/1,
					             but it was https://awexpect.com/api/
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotBaseOf("https://awexpect.com/api/tracks/1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a base URI of https://awexpect.com/api/tracks/1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/api/", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotBaseOf("https://awexpect.com/api/tracks/1"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a base URI of https://awexpect.com/api/tracks/1,
					             but it was relative
					             """);
			}
		}
	}
}
