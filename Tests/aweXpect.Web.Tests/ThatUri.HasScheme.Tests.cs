namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasScheme
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Containing_WhenSubjectSchemeContainsTheExpectedValue_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().Containing("http");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().EqualTo("http");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "http",
					             but it had scheme "https"
					             """);
			}

			[Fact]
			public async Task EqualTo_WhenSubjectEqualsExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().EqualTo("https");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().NotEqualTo("https");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has scheme not equal to "https",
					             but it had scheme "https"
					             """);
			}

			[Fact]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().NotEqualTo("http");

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task Containing_WhenSubjectSchemeContainsTheExpectedValue_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().Containing("http"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has scheme not containing "http",
					             but it had scheme "https"
					             """);
			}

			[Fact]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().EqualTo("http"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EqualTo_WhenSubjectEqualsExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().EqualTo("https"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has scheme not equal to "https",
					             but it had scheme "https"
					             """);
			}

			[Fact]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().NotEqualTo("https"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().NotEqualTo("http"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "http",
					             but it had scheme "https"
					             """);
			}
		}
	}
}
