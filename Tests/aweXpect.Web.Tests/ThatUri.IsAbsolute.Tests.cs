namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsAbsolute
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("/foo", UriKind.Relative),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.IsAbsolute()));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Uris are absolute URIs for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsAbsolute();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an absolute URI,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsAbsolute_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).IsAbsolute();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNotAbsolute_ShouldFail()
			{
				Uri subject = new Uri("https://www.awexpect.com")
					.MakeRelativeUri(new Uri("https://www.awexpect.com/foo/bar"));

				async Task Act()
					=> await That(subject).IsAbsolute();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is an absolute URI,
					             but it was foo/bar
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
					=> await That(subject).DoesNotComplyWith(it => it.IsAbsolute());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an absolute URI,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsAbsolute_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAbsolute());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not an absolute URI,
					             but it was https://www.awexpect.com/
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNotAbsolute_ShouldSucceed()
			{
				Uri subject = new Uri("https://www.awexpect.com")
					.MakeRelativeUri(new Uri("https://www.awexpect.com/foo/bar"));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAbsolute());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
