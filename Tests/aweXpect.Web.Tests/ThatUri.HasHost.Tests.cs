namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasHost
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasHost().EqualTo("awexpect.com");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has host equal to "awexpect.com",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasHost().EqualTo("awexpect.com");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has host equal to "awexpect.com",
					             but it was relative
					             """);
			}

			[Test]
			public async Task EndingWith_WhenSubjectHostEndsWithTheExpectedValue_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().EndingWith(".com");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().EqualTo("awexpect.com");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:8080/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().EqualTo("www.awexpect.com");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().NotEqualTo("awexpect.com");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost().NotEqualTo("www.awexpect.com");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "www.awexpect.com",
					             but it had host "www.awexpect.com"
					             """);
			}

			[Test]
			public async Task Shorthand_AsWildcard_WhenSubjectDoesNotMatchThePattern_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost("*.example.com").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has host matching "*.example.com",
					             but it had host "www.awexpect.com", which did not match:
					               ↓ (actual)
					               "www.awexpect.com"
					               "*.example.com"
					               ↑ (wildcard pattern)
					             """);
			}

			[Test]
			public async Task Shorthand_AsWildcard_WhenSubjectMatchesThePattern_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost("*.awexpect.com").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_IgnoringCase_WhenSubjectDiffersOnlyInCase_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost("WWW.aweXpect.COM").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasHost("awexpect.com");

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:8080/tracks/1");

				async Task Act()
					=> await That(subject).HasHost("www.awexpect.com");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectHostHasUpperCaseLetters_ShouldCompareTheLowerCaseHost()
			{
				Uri subject = new("https://WWW.aweXpect.COM/tracks/1");

				async Task Act()
					=> await That(subject).HasHost("www.awexpect.com");

				await That(Act).DoesNotThrow()
					.Because("Uri.Host is lower case for registered names");
			}

			[Test]
			public async Task Shorthand_WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasHost("awexpect.com");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has host equal to "awexpect.com",
					             but it was <null>
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
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "awexpect.com",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "awexpect.com",
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("awexpect.com"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost().EqualTo("www.awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "www.awexpect.com",
					             but it had host "www.awexpect.com"
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedHost_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost("awexpect.com"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedHost_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost("www.awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "www.awexpect.com",
					             but it had host "www.awexpect.com"
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHost("awexpect.com"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have host equal to "awexpect.com",
					             but it was <null>
					             """);
			}
		}
	}
}
