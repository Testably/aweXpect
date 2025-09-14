namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsAbsolute
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsAbsolute_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).IsAbsolute();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotAbsolute_ShouldFail()
			{
				Uri subject = new Uri("https://www.awexpect.com")
					.MakeRelativeUri(new Uri("https://www.awexpect.com/foo/bar"));

				async Task Act()
					=> await That(subject).IsAbsolute();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an absolute URI,
					             but it was foo/bar
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsAbsolute_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAbsolute());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an absolute URI,
					             but it was https://www.awexpect.com/
					             """);
			}

			[Fact]
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
