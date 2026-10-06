namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public sealed partial class IsEqualTo
	{
		public sealed class AsPrefixTests
		{
			[Test]
			public async Task WhenActualAndExpectedAreNull_ShouldThrowArgumentNullException()
			{
				string? subject = null;
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' prefix cannot be null.").AsPrefix()
					.Because("'is null' is never expressed through a prefix");
			}

			[Test]
			public async Task WhenActualIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo("").AsPrefix();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("every subject starts with the empty string, so the expectation says nothing");
			}

			[Test]
			public async Task WhenExpectedIsEmptyAfterTheLeadingWhiteSpaceIsIgnored_ShouldThrowArgumentException()
			{
				string subject = "some text";
				string expected = " \t ";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix().IgnoringLeadingWhiteSpace();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the compared prefix is the normalized one, which every subject starts with");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some text";
				string? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenStringHasAdditionalTrailingWhitespace_ShouldSucceed()
			{
				string subject = "some text \t ";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringHasMissingLeadingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = " \t some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with " \t some text",
					             but it was "some text", which misses some whitespace (" \t " at the beginning)
					             """);
			}

			[Test]
			public async Task WhenStringHasMissingTrailingWhitespace_ShouldFail()
			{
				string subject = "some text";
				string expected = "some text \t ";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text \t ",
					             but it was "some text" with a length of 9, which is shorter than the expected length of 12 and misses:
					               " \t "
					             """);
			}

			[Test]
			public async Task WhenStringHasUnexpectedLeadingWhitespace_ShouldFail()
			{
				string subject = " \t some text and more";
				string expected = "some text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text",
					             but it was " \t some text and more", which has unexpected whitespace (" \t " at the beginning)
					             """);
			}

			[Test]
			public async Task WhenStringIsShorter_ShouldFail()
			{
				string subject = "some text with";
				string expected = "some text without out";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with "some text without out",
					             but it was "some text with" with a length of 14, which is shorter than the expected length of 21 and misses:
					               "out out"
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenStringsAreTheSame_ShouldSucceed(string subject)
			{
				string expected = subject;

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenStringsDiffer_ShouldFail()
			{
				string subject = "actual text";
				string expected = "expected other text";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with "expected other text",
					             but it was "actual text", which differs at index 0:
					                ↓ (actual)
					               "actual text"
					               "expected other text"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenStringStartsWithExpected_ShouldSucceed()
			{
				string subject = "some text without out";
				string expected = "some text with";

				async Task Act()
					=> await That(subject).IsEqualTo(expected).AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTrailingWhitespaceIsIgnored_ShouldStillRequireTheWhitespaceOfThePrefixInsideTheString()
			{
				string subject = "AbbeyRoad";

				async Task Act()
					=> await That(subject).IsEqualTo("Abbey ").AsPrefix().IgnoringTrailingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             starts with "Abbey " ignoring trailing whitespace,
					             but it was "AbbeyRoad", which differs at index 5:
					                     ↓ (actual)
					               "AbbeyRoad"
					               "Abbey "
					                     ↑ (expected prefix)
					             """)
					.Because("only whitespace at the end of the subject is ignored, not the space the prefix requires inside it");
			}

			[Test]
			[Arguments(" a")]
			[Arguments("  ")]
			[Arguments("  ab")]
			public async Task WhenTrimmedStringIsShorter_ShouldNotReportWhitespace(string subject)
			{
				async Task Act()
					=> await That(subject).IsEqualTo("abc").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              starts with "abc",
					              but it was {Formatter.Format(subject)}, which differs at index 0:
					                 ↓ (actual)
					                {Formatter.Format(subject)}
					                "abc"
					                 ↑ (expected prefix)
					              """)
					.Because("removing the whitespace would not make the subject start with \"abc\"");
			}
		}

		public sealed class AsPrefixNegatedTests
		{
			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo("").AsPrefix());

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the negated expectation is just as meaningless as the positive one");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "some text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(null).AsPrefix());

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' prefix cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenStringEndsWithExpected_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "some text without";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(expected).AsPrefix().IgnoringCase());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "some text without" ignoring case,
					             but it was "some text without out"
					             """);
			}

			[Test]
			public async Task WhenStringEndsWithExpected_UsingCustomComparer_ShouldFail()
			{
				string subject = "some text without out";
				string expected = "sOmE text wIthOUt";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsEqualTo(expected).AsPrefix().Using(new IgnoreCaseForVocalsComparer()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "sOmE text wIthOUt" using IgnoreCaseForVocalsComparer,
					             but it was "some text without out"
					             """);
			}
		}
	}
}
