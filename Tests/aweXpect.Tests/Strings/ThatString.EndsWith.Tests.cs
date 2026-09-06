namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class EndsWith
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public async Task
				IgnoringCase_WhenSubjectEndsWithDifferentCase_ShouldFailUnlessCaseIsIgnored(
					bool ignoreCase)
			{
				string subject = "some arbitrary text";
				string expected = "Text";

				async Task Act()
					=> await That(subject).EndsWith(expected).IgnoringCase(ignoreCase);

				await That(Act).Throws<XunitException>()
					.OnlyIf(!ignoreCase)
					.WithMessage("""
					             Expected that subject
					             ends with "Text",
					             but it was "some arbitrary text" which differs before index 15:
					                               ↓ (actual)
					               "some arbitrary text"
					                              "Text"
					                               ↑ (expected suffix)

					             Actual:
					             some arbitrary text
					             
					             Expected:
					             Text
					             """);
			}

			[Fact]
			public async Task
				IgnoringCase_WhenSubjectEndsWithDifferentString_ShouldIncludeIgnoringCaseInMessage()
			{
				string subject = "some arbitrary text";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).EndsWith(expected).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "SOME" ignoring case,
					             but it was "some arbitrary text" which differs before index 18:
					                                  ↓ (actual)
					               "some arbitrary text"
					                              "SOME"
					                                  ↑ (expected suffix)

					             Actual:
					             some arbitrary text
					             
					             Expected:
					             SOME
					             """);
			}

			[Fact]
			public async Task
				IgnoringIndentation_WhenSubjectEndsWithDifferentlyIndentedFooter_ShouldSucceed()
			{
				string subject = "some arbitrary\n    text";
				string expected = "arbitrary\ntext";

				async Task Act()
					=> await That(subject).EndsWith(expected).IgnoringIndentation();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task
				Using_WhenSubjectEndsWithIncorrectMatchAccordingToComparer_ShouldIncludeComparerInMessage()
			{
				string subject = "some arbitrary text";
				string expected = "Text";

				async Task Act()
					=> await That(subject).EndsWith(expected)
						.Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "Text" using IgnoreCaseForVocalsComparer,
					             but it was "some arbitrary text" which differs before index 15:
					                               ↓ (actual)
					               "some arbitrary text"
					                              "Text"
					                               ↑ (expected suffix)

					             Actual:
					             some arbitrary text
					             
					             Expected:
					             Text
					             """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";
				string expected = "SOME";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "SOME",
					             but it was ""
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				string subject = "text";
				string? expected = null;

				async Task Act()
					=> await That(subject).EndsWith(expected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with <null>,
					             but it was "text"

					             Actual:
					             text
					             """);
			}

			[Fact]
			public async Task WhenSubjectDoesNotEndWithExpected_ShouldFail()
			{
				string subject = "some arbitrary text";
				string expected = "some";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "some",
					             but it was "some arbitrary text" which differs before index 18:
					                                  ↓ (actual)
					               "some arbitrary text"
					                              "some"
					                                  ↑ (expected suffix)

					             Actual:
					             some arbitrary text
					             
					             Expected:
					             some
					             """);
			}

			[Fact]
			public async Task WhenSubjectEndsWithExpected_ShouldSucceed()
			{
				string subject = "some arbitrary text";
				string expected = "text";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsEqualToExpected_ShouldSucceed()
			{
				string subject = "some text";
				string expected = subject;

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				string? subject = null;
				string expected = "text";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "text",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsShorterThanExpected_ShouldFail()
			{
				string subject = "text";
				string expected = "more than text";

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with "more than text",
					             but it was "text" which is shorter than the expected length of 14 and misses the prefix:
					               "more than "

					             Actual:
					             text
					             
					             Expected:
					             more than text
					             """);
			}
		}
	}
}
