namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasDefaultPort
	{
		public sealed class Tests
		{
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
