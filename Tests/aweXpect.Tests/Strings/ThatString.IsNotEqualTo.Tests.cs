namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualAndUnexpectedAreNull_ShouldFail()
			{
				string? subject = null;
				string? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to <null>,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldSucceed()
			{
				string? subject = null;
				string unexpected = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCustomMatchTypeComparesByValue_AndBothAreNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(null).AsCaseFolded();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not case-folded equal to <null>,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenCustomMatchTypeComparesByValue_AndOnlySubjectIsNull_ShouldSucceed()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo("abc").AsCaseFolded();

				await That(Act).DoesNotThrow()
					.Because("a custom match type that does not inspect the subject compares a null subject as a value");
			}

			[Test]
			public async Task WhenStringHasMissingLeadingWhitespace_ShouldSucceed()
			{
				string subject = "some text";
				string unexpected = " \t some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringHasMissingTrailingWhitespace_ShouldSucceed()
			{
				string subject = "some text";
				string unexpected = "some text \t ";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringHasUnexpectedLeadingWhitespace_ShouldSucceed()
			{
				string subject = " \t some text";
				string unexpected = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringHasUnexpectedTrailingWhitespace_ShouldSucceed()
			{
				string subject = "some text \t ";
				string unexpected = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringIsLonger_ShouldSucceed()
			{
				string subject = "some text without out";
				string unexpected = "some text with";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringIsShorter_ShouldSucceed()
			{
				string subject = "some text with";
				string unexpected = "some text without out";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringsAreTheSame_ShouldFail()
			{
				string subject = "foo";
				string unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to "foo",
					             but it was "foo"
					             """);
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldSucceed()
			{
				string subject = "actual text";
				string unexpected = "unexpected other text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				string subject = "some text";
				string? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringLeadingWhiteSpaceTests
		{
			[Test]
			[AutoArguments(" foo", "foo")]
			[AutoArguments("foo", " foo")]
			[AutoArguments("\tfoo", "\nfoo")]
			[AutoArguments("\r\nfoo", "foo")]
			[AutoArguments("foo", "\tfoo")]
			public async Task WhenStringsDifferOnlyInLeadingWhiteSpace_ShouldFail(
				string subject, string unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to "{unexpected.DisplayWhitespace()}" ignoring leading whitespace,
					              but it was "{subject.DisplayWhitespace()}"
					              """);
			}
		}

		public sealed class IgnoringNewlineStyleTests
		{
			[Test]
			[AutoArguments("foo\nbar", "foo\rbar")]
			[AutoArguments("foo\rbar", "foo\nbar")]
			[AutoArguments("foo\nbar", "foo\r\nbar")]
			[AutoArguments("foo\rbar", "foo\r\nbar")]
			[AutoArguments("foo\r\nbar", "foo\nbar")]
			[AutoArguments("foo\r\nbar", "foo\rbar")]
			public async Task WhenStringsDifferOnlyInNewlineStyle_ShouldFail(
				string subject, string unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringNewlineStyle();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to "{unexpected.DisplayWhitespace()}" ignoring newline style,
					              but it was "{subject.DisplayWhitespace()}"
					              """);
			}
		}

		public sealed class IgnoringTrailingWhiteSpaceTests
		{
			[Test]
			[AutoArguments("foo ", "foo")]
			[AutoArguments("foo", "foo ")]
			[AutoArguments("foo\t", "foo\n")]
			[AutoArguments("foo\r\n", "foo")]
			[AutoArguments("foo", "foo\t")]
			public async Task WhenStringsDifferOnlyInTrailingWhiteSpace_ShouldFail(
				string subject, string unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to "{unexpected.DisplayWhitespace()}" ignoring trailing whitespace,
					              but it was "{subject.DisplayWhitespace()}"
					              """);
			}
		}
	}
}
