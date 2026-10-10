namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasDefaultPort
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("https://awexpect.com:8080/"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.HasDefaultPort()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Uris have the default port for the used scheme for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasDefaultPort();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has the default port for the used scheme,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData("https://www.awexpect.com:80")]
			[InlineData("http://www.example.com:443")]
			public async Task WhenSubjectDoesNotHaveTheDefaultPort_ShouldFail(string uriString)
			{
				Uri subject = new(uriString);

				async Task Act()
					=> await That(subject).HasDefaultPort();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has the default port for the used scheme,
					              but it was {subject}
					              """);
			}

			[Theory]
			[InlineData("https://www.awexpect.com")]
			[InlineData("https://www.awexpect.com:443")]
			[InlineData("http://www.example.com:80")]
			public async Task WhenSubjectHasTheDefaultPort_ShouldSucceed(string uriString)
			{
				Uri subject = new(uriString);

				async Task Act()
					=> await That(subject).HasDefaultPort();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasDefaultPort());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have the default port for the used scheme,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData("https://www.awexpect.com:80")]
			[InlineData("http://www.example.com:443")]
			public async Task WhenSubjectDoesNotHaveTheDefaultPort_ShouldSucceed(string uriString)
			{
				Uri subject = new(uriString);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasDefaultPort());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("https://www.awexpect.com")]
			[InlineData("https://www.awexpect.com:443")]
			[InlineData("http://www.example.com:80")]
			public async Task WhenSubjectHasTheDefaultPort_ShouldFail(string uriString)
			{
				Uri subject = new(uriString);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasDefaultPort());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have the default port for the used scheme,
					              but it was {subject}
					              """);
			}
		}
	}
}
