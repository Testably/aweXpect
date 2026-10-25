namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasScheme
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasScheme().EqualTo("https");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "https",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasScheme().EqualTo("https");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "https",
					             but it was relative
					             """);
			}

			[Test]
			public async Task Containing_AsWildcard_WhenSchemeDoesNotContainThePattern_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().Containing("h?p").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme containing "h?p" as wildcard at least once,
					             but scheme did not contain "h?p" in "https"
					             """);
			}

			[Test]
			public async Task Containing_IgnoringCase_WhenSchemeContainsExpectedTooRarely_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().Containing("T").IgnoringCase().AtLeast(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme containing "T" ignoring case at least 3 times,
					             but scheme contained "T" twice in "https"
					             """);
			}

			[Test]
			public async Task Containing_WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasScheme().Containing("http");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme containing "http" at least once,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task Containing_WhenSubjectSchemeContainsTheExpectedValue_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().Containing("http");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().EqualTo("http");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "http",
					             but it had scheme "https" with a length of 5, which is longer than the expected length of 4 and has superfluous:
					               "s"
					             """);
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().EqualTo("https");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().NotEqualTo("https");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have scheme equal to "https",
					             but it had scheme "https"
					             """);
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme().NotEqualTo("http");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_AsPrefix_WhenSubjectStartsWithTheExpectedValue_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme("http").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_IgnoringCase_WhenSubjectDiffersOnlyInCase_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme("HTTPS").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme("http");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "http",
					             but it had scheme "https" with a length of 5, which is longer than the expected length of 4 and has superfluous:
					               "s"
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme("https");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasScheme("https");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "https",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectSchemeHasUpperCaseLetters_ShouldCompareTheLowerCaseScheme()
			{
				Uri subject = new("HTTPS://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).HasScheme("https");

				await That(Act).DoesNotThrow()
					.Because("Uri.Scheme is always lower case");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().EqualTo("https"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have scheme equal to "https",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().EqualTo("https"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have scheme equal to "https",
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Test]
			public async Task Containing_Never_WhenSchemeDoesNotContainTheExpectedValue_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().Containing("ftp").Never());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme containing "ftp" at least once,
					             but scheme did not contain "ftp" in "https"
					             """)
					.Because("the negation of never is at least once");
			}

			[Test]
			public async Task Containing_WhenSubjectSchemeContainsTheExpectedValue_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().Containing("http"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have scheme containing "http",
					             but scheme contained "http" at least once in "https"
					             """);
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().EqualTo("http"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().EqualTo("https"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have scheme equal to "https",
					             but it had scheme "https"
					             """);
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().NotEqualTo("https"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme().NotEqualTo("http"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has scheme equal to "http",
					             but it had scheme "https" with a length of 5, which is longer than the expected length of 4 and has superfluous:
					               "s"
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedScheme_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme("http"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedScheme_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com:80");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasScheme("https"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have scheme equal to "https",
					             but it had scheme "https"
					             """);
			}
		}
	}
}
