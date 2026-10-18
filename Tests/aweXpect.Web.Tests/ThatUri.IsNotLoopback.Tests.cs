namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsNotLoopback
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("http://localhost/"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.IsNotLoopback()));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Uris do not reference the local host for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not reference the local host,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not reference the local host,
					             but it was relative
					             """);
			}

			[Test]
			[Arguments("127.0.0.1")]
			[Arguments("loopback")]
			[Arguments("localhost")]
			public async Task WhenSubjectIsNotLoopback_ShouldFail(string uriString)
			{
				Uri subject = new UriBuilder(uriString).Uri;

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not reference the local host,
					              but it was {subject}
					              """);
			}

			[Test]
			public async Task WhenSubjectIsNotLoopback_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).IsNotLoopback();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             references the local host,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             references the local host,
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Test]
			public async Task WhenSubjectIsNotLoopback_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             references the local host,
					             but it was https://www.awexpect.com/
					             """);
			}

			[Test]
			[Arguments("127.0.0.1")]
			[Arguments("loopback")]
			[Arguments("localhost")]
			public async Task WhenSubjectIsNotLoopback_ShouldSucceed(string uriString)
			{
				Uri subject = new UriBuilder(uriString).Uri;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotLoopback());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
