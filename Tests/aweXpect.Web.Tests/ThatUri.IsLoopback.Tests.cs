namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsLoopback
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("127.0.0.1")]
			[InlineData("loopback")]
			[InlineData("localhost")]
			public async Task WhenSubjectIsLoopback_ShouldSucceed(string uriString)
			{
				Uri subject = new UriBuilder(uriString).Uri;

				async Task Act()
					=> await That(subject).IsLoopback();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotLoopback_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).IsLoopback();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             references the local host,
					             but it was https://www.awexpect.com/
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("127.0.0.1")]
			[InlineData("loopback")]
			[InlineData("localhost")]
			public async Task WhenSubjectIsLoopback_ShouldFail(string uriString)
			{
				Uri subject = new UriBuilder(uriString).Uri;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLoopback());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not reference the local host,
					              but it was {subject}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsNotLoopback_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLoopback());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
