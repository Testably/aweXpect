namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class StartsWith
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public async Task
				IgnoringCase_WhenSubjectStartsWithDifferentCase_ShouldFailUnlessCaseIsIgnored(
					bool ignoreCase)
			{
				string subject = "Some arbitrary text";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).StartsWith(expected)
						.IgnoringCase(ignoreCase);

				await That(Act).Throws<XunitException>()
					.OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             starts with "SOME",
					             but it was "Some arbitrary text" which differs at index 1:
					                 ↓ (actual)
					               "Some arbitrary text"
					               "SOME"
					                 ↑ (expected prefix)
					             """);
			}

			[Fact]
			public async Task
				IgnoringCase_WhenSubjectStartsWithDifferentString_ShouldIncludeIgnoringCaseInMessage()
			{
				string subject = "some arbitrary text";
				string expected = "TEXT";

				async Task Act()
					=> await That(subject).StartsWith(expected).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "TEXT" ignoring case,
					             but it was "some arbitrary text" which differs at index 0:
					                ↓ (actual)
					               "some arbitrary text"
					               "TEXT"
					                ↑ (expected prefix)
					             """);
			}

			[Fact]
			public async Task
				IgnoringIndentation_WhenSubjectStartsWithDifferentlyIndentedHeader_ShouldSucceed()
			{
				string subject = "    some arbitrary\n        text";
				string expected = "some arbitrary\ntext";

				async Task Act()
					=> await That(subject).StartsWith(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task
				IgnoringIndentation_WhenSubjectStartsWithDifferentString_ShouldReportOriginalPosition()
			{
				string subject = "    some arbitrary text";
				string expected = "other";

				async Task Act()
					=> await That(subject).StartsWith(expected).IgnoringIndentation();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "other" ignoring indentation,
					             but it was "some arbitrary text" which differs at index 4:
					                ↓ (actual)
					               "some arbitrary text"
					               "other"
					                ↑ (expected prefix)

					             Actual:
					                 some arbitrary text
					             """);
			}

			[Fact]
			public async Task
				Using_WhenSubjectStartsWithIncorrectMatchAccordingToComparer_ShouldIncludeComparerInMessage()
			{
				string subject = "some arbitrary text";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).StartsWith(expected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "SOME" using IgnoreCaseForVocalsComparer,
					             but it was "some arbitrary text" which differs at index 0:
					                ↓ (actual)
					               "some arbitrary text"
					               "SOME"
					                ↑ (expected prefix)
					             """);
			}

			[Fact]
			public async Task
				Using_WhenSubjectStartsWithMatchAccordingToComparer_ShouldSucceed()
			{
				string subject = "some arbitrary text";
				string expected = "sOmE";

				async Task Act()
					=> await That(subject).StartsWith(expected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "SOME",
					             but it was ""
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "text";
				string? expected = null;

				async Task Act()
					=> await That(subject).StartsWith(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedIsWhiteSpaceAndLeadingWhiteSpaceIsIgnored_ShouldThrowArgumentException()
			{
				string subject = " text";

				async Task Act()
					=> await That(subject).StartsWith(" ").IgnoringLeadingWhiteSpace();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
					.WithParamName("expected")
					.Because("the prefix is empty once the leading white-space is ignored");
			}

			[Fact]
			public async Task WhenSubjectDoesNotStartWithExpected_ShouldFail()
			{
				string subject = "some arbitrary text";
				string expected = "text";

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "text",
					             but it was "some arbitrary text" which differs at index 0:
					                ↓ (actual)
					               "some arbitrary text"
					               "text"
					                ↑ (expected prefix)
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsEqualToExpected_ShouldSucceed()
			{
				string subject = "some text";
				string expected = subject;

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "text";

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "text",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsShorterThanExpected_ShouldFail()
			{
				string subject = "text";
				string expected = "text and more";

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "text and more",
					             but it was "text" with a length of 4 which is shorter than the expected length of 13 and misses:
					               " and more"
					             """);
			}

			[Fact]
			public async Task WhenSubjectStartsWithExpected_ShouldSucceed()
			{
				string subject = "some arbitrary text";
				string expected = "some";

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
