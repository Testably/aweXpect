namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenActualAndUnexpectedAreNull_ShouldSucceed()
			{
				string? subject = null;
				string? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldSucceed()
			{
				string? subject = null;
				string unexpected = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				string subject = "some text";
				string? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasMissingLeadingWhitespace_ShouldSucceed()
			{
				string subject = "some text";
				string unexpected = " \t some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasMissingTrailingWhitespace_ShouldSucceed()
			{
				string subject = "some text";
				string unexpected = "some text \t ";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasUnexpectedLeadingWhitespace_ShouldSucceed()
			{
				string subject = " \t some text";
				string unexpected = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasUnexpectedTrailingWhitespace_ShouldSucceed()
			{
				string subject = "some text \t ";
				string unexpected = "some text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringIsLonger_ShouldSucceed()
			{
				string subject = "some text without out";
				string unexpected = "some text with";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringIsShorter_ShouldSucceed()
			{
				string subject = "some text with";
				string unexpected = "some text without out";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringsAreTheSame_ShouldFail()
			{
				string subject = "foo";
				string unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to "foo",
					             but it was "foo"
					             """);
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldSucceed()
			{
				string subject = "actual text";
				string unexpected = "unexpected other text";

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class IgnoringLeadingWhiteSpaceTests
		{
			[Theory]
			[InlineAutoData(" foo", "foo")]
			[InlineAutoData("foo", " foo")]
			[InlineAutoData("\tfoo", "\nfoo")]
			[InlineAutoData("\r\nfoo", "foo")]
			[InlineAutoData("foo", "\tfoo")]
			public async Task WhenStringsDifferOnlyInLeadingWhiteSpace_ShouldSucceed(
				string subject, string unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringLeadingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to "{unexpected.DisplayWhitespace()}" ignoring leading whitespace,
					              but it was "{subject.DisplayWhitespace()}"
					              """);
			}
		}

		public sealed class IgnoringNewlineStyleTests
		{
			[Theory]
			[InlineAutoData("foo\nbar", "foo\rbar")]
			[InlineAutoData("foo\rbar", "foo\nbar")]
			[InlineAutoData("foo\nbar", "foo\r\nbar")]
			[InlineAutoData("foo\rbar", "foo\r\nbar")]
			[InlineAutoData("foo\r\nbar", "foo\nbar")]
			[InlineAutoData("foo\r\nbar", "foo\rbar")]
			public async Task WhenStringsDifferOnlyInNewlineStyle_ShouldSucceed(
				string subject, string unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringNewlineStyle();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to "{unexpected.DisplayWhitespace()}" ignoring newline style,
					              but it was "{subject.DisplayWhitespace()}"
					              """);
			}
		}

		public sealed class IgnoringTrailingWhiteSpaceTests
		{
			[Theory]
			[InlineAutoData("foo ", "foo")]
			[InlineAutoData("foo", "foo ")]
			[InlineAutoData("foo\t", "foo\n")]
			[InlineAutoData("foo\r\n", "foo")]
			[InlineAutoData("foo", "foo\t")]
			public async Task WhenStringsDifferOnlyInTrailingWhiteSpace_ShouldFail(
				string subject, string unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected).IgnoringTrailingWhiteSpace();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to "{unexpected.DisplayWhitespace()}" ignoring trailing whitespace,
					              but it was "{subject.DisplayWhitespace()}"
					              """);
			}
		}
	}
}
