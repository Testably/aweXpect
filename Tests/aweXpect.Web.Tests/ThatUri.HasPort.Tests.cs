namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasPort
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasPort().EqualTo(443);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has port equal to 443,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasPort().EqualTo(443);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has port equal to 443,
					             but it was relative
					             """);
			}

			[Test]
			public async Task Between_WhenSubjectPortIsInRange_ShouldSucceed()
			{
				Uri subject = new("https://localhost:5001/health");

				async Task Act()
					=> await That(subject).HasPort().Between(5000).And(5999);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedPort_ShouldFail()
			{
				Uri subject = new("https://localhost:5001/health");

				async Task Act()
					=> await That(subject).HasPort().EqualTo(5000);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has port equal to 5000,
					             but it had port 5001
					             """);
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedPort_ShouldSucceed()
			{
				Uri subject = new("https://localhost:5001/health");

				async Task Act()
					=> await That(subject).HasPort().EqualTo(5001);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectUsesTheDefaultPort_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com");

				async Task Act()
					=> await That(subject).HasPort().EqualTo(443);

				await That(Act).DoesNotThrow()
					.Because("the port of a URI without an explicit port is the default port of its scheme");
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedPort_ShouldFail()
			{
				Uri subject = new("https://localhost:5001/health");

				async Task Act()
					=> await That(subject).HasPort(4000);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has port equal to 4000,
					             but it had port 5001
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedPort_ShouldSucceed()
			{
				Uri subject = new("https://localhost:4000/health");

				async Task Act()
					=> await That(subject).HasPort(4000);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasPort(4000);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has port equal to 4000,
					             but it was relative
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
					=> await That(subject).DoesNotComplyWith(it => it.HasPort().EqualTo(443));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have port equal to 443,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPort().EqualTo(443));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have port equal to 443,
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedPort_ShouldSucceed()
			{
				Uri subject = new("https://localhost:5001/health");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPort().EqualTo(5000));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedPort_ShouldFail()
			{
				Uri subject = new("https://localhost:5001/health");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPort().EqualTo(5001));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have port equal to 5001,
					             but it had port 5001
					             """);
			}

			[Test]
			public async Task Shorthand_WhenSubjectDoesNotEqualTheExpectedPort_ShouldSucceed()
			{
				Uri subject = new("https://localhost:5001/health");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPort(4000));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Shorthand_WhenSubjectEqualsExpectedPort_ShouldFail()
			{
				Uri subject = new("https://localhost:4000/health");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasPort(4000));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have port equal to 4000,
					             but it had port 4000
					             """);
			}
		}
	}
}
