namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasHost
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasHost().EqualTo("awexpect.com");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has host equal to "awexpect.com",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasHost().EqualTo("awexpect.com");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has host equal to "awexpect.com",
					             but it was relative
					             """);
			}

			[Fact]
			public async Task EndingWith_WhenSubjectHostEndsWithTheExpectedValue_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().EndingWith(".com");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().EqualTo("awexpect.com");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has host equal to "awexpect.com",
					             but it had host "www.awexpect.com", which differs at index 0:
					                ↓ (actual)
					               "www.awexpect.com"
					               "awexpect.com"
					                ↑ (expected)
					             """);
			}

			[Fact]
			public async Task EqualTo_WhenSubjectEqualsExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:8080/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().EqualTo("www.awexpect.com");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().NotEqualTo("awexpect.com");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().NotEqualTo("www.awexpect.com");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "www.awexpect.com",
					             but it had host "www.awexpect.com"
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("awexpect.com"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "awexpect.com",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("awexpect.com"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "awexpect.com",
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Fact]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("awexpect.com"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task EqualTo_WhenSubjectEqualsExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("www.awexpect.com"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "www.awexpect.com",
					             but it had host "www.awexpect.com"
					             """);
			}
		}
	}
}
