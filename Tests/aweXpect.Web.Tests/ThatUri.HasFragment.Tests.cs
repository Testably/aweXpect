namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class HasFragment
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("lyrics");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has fragment equal to "lyrics",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1#lyrics", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("lyrics");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has fragment equal to "lyrics",
					             but it was relative
					             """);
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedFragment_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#lyrics");

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("credits");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has fragment equal to "credits",
					             but it had fragment "lyrics", which differs at index 0:
					                ↓ (actual)
					               "lyrics"
					               "credits"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedFragment_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#lyrics");

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("lyrics");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenExpectedContainsTheDelimiter_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#lyrics");

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("#lyrics");

				await That(Act).Throws<FailException>()
					.Because("the leading '#' is not part of the fragment");
			}

			[Test]
			public async Task EqualTo_WhenFragmentStartsWithTheDelimiter_ShouldOnlyStripTheFirstOne()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1##lyrics");

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("#lyrics");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectHasNoFragment_ShouldBeEmpty()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1");

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectHasEscapedCharacters_ShouldCompareTheDecodedFragment()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#caf%C3%A9");

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("café");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectHasPlus_ShouldKeepThePlus()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#rock+roll");

				async Task Act()
					=> await That(subject).HasFragment().EqualTo("rock+roll");

				await That(Act).DoesNotThrow()
					.Because("'+' only means a space in form-encoded query strings, not in fragments");
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectDoesNotEqualTheExpectedFragment_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#lyrics");

				async Task Act()
					=> await That(subject).HasFragment().NotEqualTo("credits");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task NotEqualTo_WhenSubjectEqualsExpectedFragment_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#lyrics");

				async Task Act()
					=> await That(subject).HasFragment().NotEqualTo("lyrics");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have fragment equal to "lyrics",
					             but it had fragment "lyrics"
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
					=> await That(subject).DoesNotComplyWith(it => it.HasFragment().EqualTo("lyrics"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have fragment equal to "lyrics",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("/tracks/1#lyrics", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasFragment().EqualTo("credits"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have fragment equal to "credits",
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Test]
			public async Task EqualTo_WhenSubjectDoesNotEqualTheExpectedFragment_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#lyrics");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasFragment().EqualTo("credits"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task EqualTo_WhenSubjectEqualsExpectedFragment_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com/tracks/1#lyrics");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasFragment().EqualTo("lyrics"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have fragment equal to "lyrics",
					             but it had fragment "lyrics"
					             """);
			}
		}
	}
}
