namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsSuffixTests
		{
			[Fact]
			public async Task WhenActualAndExpectedAreNull_ShouldThrowArgumentNullException()
			{
				string? subject = null;
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' suffix cannot be null.").AsPrefix()
					.Because("'is null' is never expressed through a suffix");
			}

			[Fact]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "some text",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo("").AsSuffix();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("every subject ends with the empty string, so the expectation says nothing");
			}

			[Fact]
			public async Task WhenExpectedIsEmptyAfterTheIndentationIsIgnored_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string expected = "  ";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix().IgnoringIndentation();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the compared suffix is the normalized one, which every subject ends with");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some text";
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' suffix cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenStringEndsWithExpected_ShouldSucceed()
			{
				string subject = "some text without out";
				string expected = "text without out";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasAdditionalLeadingWhitespace_ShouldSucceed()
			{
				string subject = " \t some text";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringHasMissingLeadingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = " \t some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with " \t some text",
					             but it was "some text" which is shorter than the expected length of 12 and misses the prefix:
					               " \t "
					             """);
			}

			[Fact]
			public async Task WhenStringHasMissingTrailingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = "some text \t ";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "some text \t ",
					             but it was "some text" which differs before index 8:
					                            ↓ (actual)
					                   "some text"
					               "some text \t "
					                            ↑ (expected suffix)
					             """);
			}

			[Fact]
			public async Task WhenStringHasUnexpectedTrailingWhitespace_ShouldFail()
			{
				string subject = "and some text \t ";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "some text",
					             but it was "and some text \t " which differs before index 15:
					                                ↓ (actual)
					               "and some text \t "
					                       "some text"
					                                ↑ (expected suffix)
					             """);
			}

			[Fact]
			public async Task WhenStringIsShorter_ShouldFail()
			{
				string subject = "text without out";
				string expected = "some text without out";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "some text without out",
					             but it was "text without out" which is shorter than the expected length of 21 and misses the prefix:
					               "some "
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenStringsAreTheSame_ShouldSucceed(string subject)
			{
				string expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string subject = "actual text";
				string expected = "other text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsSuffix();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "other text",
					             but it was "actual text" which differs before index 5:
					                     ↓ (actual)
					               "actual text"
					                "other text"
					                     ↑ (expected suffix)
					             """);
			}
		}

		public sealed class AsSuffixNegatedTests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo("").AsSuffix());

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the negated expectation is just as meaningless as the positive one");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(null).AsSuffix());

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' suffix cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenStringEndsWithExpected_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "text without out";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(expected).AsSuffix().IgnoringCase());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "text without out" ignoring case,
					             but it was "some text without out"
					             """);
			}

			[Fact]
			public async Task WhenStringEndsWithExpected_UsingCustomComparer_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "text wIthOUt OUt";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsEqualTo(expected).AsSuffix().Using(new IgnoreCaseForVocalsComparer()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "text wIthOUt OUt" using IgnoreCaseForVocalsComparer,
					             but it was "some text without out"
					             """);
			}
		}
	}
}
