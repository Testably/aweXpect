namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class EndsWith
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task
				IgnoringCase_WhenSubjectEndsWithDifferentCase_ShouldFailUnlessCaseIsIgnored(
					bool ignoreCase)
			{
				string subject = "some arbitrary text";
				string expected = "Text";

				async Task Act()
					=> await That(subject).EndsWith(expected).IgnoringCase(ignoreCase);

				await That(Act).Throws<FailException>()
					.OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             ends with "Text",
					             but it was "some arbitrary text", which differs at index 15:
					                               ↓ (actual)
					               "some arbitrary text"
					                              "Text"
					                               ↑ (expected suffix)
					             """);
			}

			[Test]
			public async Task
				IgnoringCase_WhenSubjectEndsWithDifferentString_ShouldIncludeIgnoringCaseInMessage()
			{
				string subject = "some arbitrary text";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).EndsWith(expected).IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with "SOME" ignoring case,
					             but it was "some arbitrary text", which differs at index 18:
					                                  ↓ (actual)
					               "some arbitrary text"
					                              "SOME"
					                                  ↑ (expected suffix)
					             """);
			}

			[Test]
			public async Task
				IgnoringIndentation_WhenSubjectEndsWithDifferentlyIndentedFooter_ShouldSucceed()
			{
				string subject = "some arbitrary\n    text";
				string expected = "arbitrary\ntext";

				async Task Act()
					=> await That(subject).EndsWith(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task
				IgnoringLeadingWhiteSpace_WhenSuffixReachesTheStartOfTheSubject_ShouldIgnoreItsLeadingWhiteSpace()
			{
				string subject = "Abbey";

				async Task Act()
					=> await That(subject).EndsWith("\t Abbey").IgnoringLeadingWhiteSpace();

				await That(Act).DoesNotThrow()
					.Because("the whitespace of the suffix lies at the start of the subject, where it is ignored");
			}

			[Test]
			public async Task
				IgnoringLeadingWhiteSpace_WhenSuffixStartsWithWhiteSpaceInsideTheSubject_ShouldFail()
			{
				string subject = "RoadAbbey";

				async Task Act()
					=> await That(subject).EndsWith(" Abbey").IgnoringLeadingWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with " Abbey" ignoring leading whitespace,
					             but it was "RoadAbbey", which differs at index 3:
					                   ↓ (actual)
					               "RoadAbbey"
					                  " Abbey"
					                   ↑ (expected suffix)
					             """)
					.Because("only whitespace at the start of the subject is ignored, not the space the suffix requires inside it");
			}

			[Test]
			public async Task
				Using_WhenSubjectEndsWithIncorrectMatchAccordingToComparer_ShouldIncludeComparerInMessage()
			{
				string subject = "some arbitrary text";
				string expected = "Text";

				async Task Act()
					=> await That(subject).EndsWith(expected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with "Text" using IgnoreCaseForVocalsComparer,
					             but it was "some arbitrary text", which differs at index 15:
					                               ↓ (actual)
					               "some arbitrary text"
					                              "Text"
					                               ↑ (expected suffix)
					             """);
			}

			[Test]
			public async Task
				Using_WhenSubjectEndsWithMatchAccordingToComparer_ShouldSucceed()
			{
				string subject = "some arbitrary text";
				string expected = "tExt";

				async Task Act()
					=> await That(subject).EndsWith(expected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with "SOME",
					             but it was ""
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "text";
				string? expected = null;

				async Task Act()
					=> await That(subject).EndsWith(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsWhiteSpaceAndTrailingWhiteSpaceIsIgnored_ShouldThrowArgumentException()
			{
				string subject = "text ";

				async Task Act()
					=> await That(subject).EndsWith(" ").IgnoringTrailingWhiteSpace();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the suffix is empty once the trailing whitespace is ignored");
			}

			[Test]
			public async Task WhenSubjectDoesNotEndWithExpected_ShouldFail()
			{
				string subject = "some arbitrary text";
				string expected = "some";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with "some",
					             but it was "some arbitrary text", which differs at index 18:
					                                  ↓ (actual)
					               "some arbitrary text"
					                              "some"
					                                  ↑ (expected suffix)
					             """);
			}

			[Test]
			public async Task WhenSubjectEndsWithExpected_ShouldSucceed()
			{
				string subject = "some arbitrary text";
				string expected = "text";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsEqualToExpected_ShouldSucceed()
			{
				string subject = "some text";
				string expected = subject;

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "text";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with "text",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsShorterThanExpected_ShouldFail()
			{
				string subject = "text";
				string expected = "more than text";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with "more than text",
					             but it was "text" with a length of 4, which is shorter than the expected length of 14 and misses the prefix:
					               "more than "
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectDoesNotEndWithExpected_ShouldSucceed()
			{
				string subject = "Some arbitrary text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.EndsWith("Some"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectEndsWithExpected_ShouldFail()
			{
				string subject = "Some arbitrary text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.EndsWith("text"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "text",
					             but it was "Some arbitrary text"
					             """);
			}

			[Test]
			public async Task WhenSubjectEndsWithExpectedIgnoringCase_ShouldFail()
			{
				string subject = "Some arbitrary text";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.EndsWith("TEXT").IgnoringCase());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "TEXT" ignoring case,
					             but it was "Some arbitrary text"
					             """);
			}
		}
	}
}
